using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

// ZXing
using ZXing;
using ZXing.Common;

public class QrFolderWatcher : MonoBehaviour
{
    [Header("감시할 폴더 전체 경로 (예: C:\\\\QR_Inbox)")]
    [SerializeField] private string folderPath = "";

    [Header("지원 확장자(소문자)")]
    [SerializeField] private string[] allowedExt = new[] { ".png", ".jpg", ".jpeg" };

    [Header("감시 실패 시 폴링(초)")]
    [SerializeField] private float pollingIntervalSeconds = 2f;

    // 내부 상태
    private FileSystemWatcher watcher;
    private readonly ConcurrentQueue<string> pendingFiles = new ConcurrentQueue<string>();
    private readonly HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private float pollingTimer;
    private bool watcherActive;

    // ZXing 리더(메인 스레드에서 생성)
    private IBarcodeReader reader;

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            Debug.LogError("[QrFolderWatcher] Folder Path가 비어있습니다. 절대경로를 입력해주세요.");
            enabled = false;
            return;
        }

        try
        {
            Directory.CreateDirectory(folderPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"[QrFolderWatcher] 폴더 생성/접근 실패: {e.Message}");
            enabled = false;
            return;
        }

        // 시작 시점의 기존 파일은 '이미 처리됨'으로 표시 (새 파일만 처리)
        try
        {
            foreach (var f in Directory.GetFiles(folderPath))
            {
                if (IsAllowed(f)) seen.Add(f);
            }
        }
        catch (Exception) { /* ignore */ }

        // ZXing 리더 준비
        reader = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
            }
        };

        // FileSystemWatcher 시도
        watcherActive = TryStartWatcher();

        if (!watcherActive)
        {
            Debug.LogWarning("[QrFolderWatcher] FileSystemWatcher 시작 실패. 폴링으로 대체합니다.");
        }
        else
        {
            Debug.Log("[QrFolderWatcher] FileSystemWatcher 활성화됨.");
        }
    }

    private bool TryStartWatcher()
    {
        try
        {
            watcher = new FileSystemWatcher(folderPath);
            watcher.IncludeSubdirectories = false;
            watcher.Filter = "*.*"; // 확장자는 핸들러에서 필터링
            watcher.NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.LastWrite |
                NotifyFilters.CreationTime |
                NotifyFilters.Size;

            watcher.Created += OnFsEvent;
            watcher.Changed += OnFsEvent;
            watcher.Renamed += OnFsRenamed;
            watcher.Error += (s, e) =>
            {
                Debug.LogWarning($"[QrFolderWatcher] Watcher 오류 발생: {e.GetException()?.Message}");
                watcherActive = false; // 폴링으로 폴백
            };

            watcher.EnableRaisingEvents = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[QrFolderWatcher] Watcher 시작 실패: {e.Message}");
            watcher = null;
            return false;
        }
    }

    private void OnDestroy()
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Created -= OnFsEvent;
            watcher.Changed -= OnFsEvent;
            watcher.Renamed -= OnFsRenamed;
            watcher.Dispose();
            watcher = null;
        }
    }

    private void OnFsRenamed(object sender, RenamedEventArgs e)
    {
        if (!IsAllowed(e.FullPath)) return;
        EnqueueIfNew(e.FullPath);
    }

    private void OnFsEvent(object sender, FileSystemEventArgs e)
    {
        if (!IsAllowed(e.FullPath)) return;
        EnqueueIfNew(e.FullPath);
    }

    private bool IsAllowed(string path)
    {
        var ext = Path.GetExtension(path)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) return false;
        return allowedExt.Contains(ext);
    }

    private void EnqueueIfNew(string fullPath)
    {
        // 파일이 생성/변경 이벤트가 여러 번 올 수 있으므로 단순 중복 방지
        // (완료 시에는 seen에 추가)
        if (!seen.Contains(fullPath))
        {
            pendingFiles.Enqueue(fullPath);
        }
    }

    private void Update()
    {
        // Watcher가 비활성화되면 주기적 폴링으로 신규 파일 확인
        if (!watcherActive)
        {
            pollingTimer += Time.deltaTime;
            if (pollingTimer >= pollingIntervalSeconds)
            {
                pollingTimer = 0f;
                TryEnqueueNewFilesByPolling();
            }
        }

        // 메인 스레드에서 처리(텍스처 생성 등 Unity API 사용)
        int maxPerFrame = 4; // 한 프레임 과도한 처리 방지
        for (int i = 0; i < maxPerFrame; i++)
        {
            if (!pendingFiles.TryDequeue(out var path)) break;
            ProcessFile(path);
        }
    }

    private void TryEnqueueNewFilesByPolling()
    {
        try
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                if (IsAllowed(file) && !seen.Contains(file))
                {
                    pendingFiles.Enqueue(file);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[QrFolderWatcher] 폴링 중 오류: {e.Message}");
        }
    }

    private void ProcessFile(string path)
    {
        // 파일이 완전히 기록될 때까지 대기 (짧게 재시도)
        if (!WaitForFileReady(path, 2000))
        {
            // 다음 프레임 재시도하도록 다시 큐잉
            pendingFiles.Enqueue(path);
            return;
        }

        try
        {
            byte[] bytes = File.ReadAllBytes(path);

            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tex.LoadImage(bytes, markNonReadable: false))
            {
                Debug.LogWarning($"[QrFolderWatcher] 이미지 로딩 실패: {path}");
                Destroy(tex);
                return;
            }

            // ZXing은 Color32 배열을 선호
            var colors = tex.GetPixels32();
            var result = reader.Decode(colors, tex.width, tex.height);

            if (result != null)
            {
                Debug.Log($"[QR] 파일: {Path.GetFileName(path)} / 값: {result.Text}");
                GameManager.Instance.OnQRDataDetected?.Invoke(result.Text);
            }
            else
            {
                Debug.LogWarning($"[QrFolderWatcher] QR 미인식: {Path.GetFileName(path)}");
            }

            Destroy(tex);

            // 처리 완료로 간주
            seen.Add(path);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[QrFolderWatcher] 처리 중 예외({Path.GetFileName(path)}): {e.Message}");
            // 일시적 오류일 수 있으니 한 번 더 시도하고 싶다면 재큐잉 가능
            // pendingFiles.Enqueue(path);
        }
    }

    /// <summary>
    /// 파일이 다른 프로세스에서 쓰는 중이라 잠겨있을 수 있으므로 짧게 재시도.
    /// </summary>
    private bool WaitForFileReady(string path, int timeoutMs)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            try
            {
                // 공유 읽기 허용으로 열어보고 바로 닫기
                using (var stream = new FileStream(
                    path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (stream.Length > 0)
                        return true;
                }
            }
            catch
            {
                // 잠시 후 재시도
            }
            Thread.Sleep(50);
        }
        return false;
    }
}