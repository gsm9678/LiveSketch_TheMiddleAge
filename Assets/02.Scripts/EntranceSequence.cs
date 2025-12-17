using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Suntail;

public class EntranceSequence : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform spawnPointInside;   // 문 뒤(계단일 수도 있음)
    [SerializeField] private Transform stairBottomPoint;   // 계단 아래

    [Header("Door")]
    [SerializeField] private Interactive doorInteractive;
    [SerializeField] private float doorOpenDelay = 0.2f;

    [Header("After")]
    [SerializeField] private bool allowStairsInWander = false; // 평소 배회: 계단 금지


    public void Play(GameObject go)
    {
        characterPrefab = go;
        StartCoroutine(CoPlay());
    }

    private IEnumerator CoPlay()
    {
        GameObject go = Instantiate(characterPrefab, spawnPointInside.position, spawnPointInside.rotation);

        var pc = go.GetComponent<PlayerController>();
        var ch = go.GetComponent<Character>();

        ch.is_call = true;
        if (pc == null || ch == null)
        {
            Debug.LogError("[EntranceSequence] Prefab needs PlayerController + Character.");
            yield break;
        }

        // (핵심) 스폰 즉시 Character가 자동배회 시작해서 Walkable로 튕기는 것 방지
        ch.SetAutoStartWander(false);

        // 연출 시작: Idle 고정 + 계단 허용
        pc.EnterIdle();
        pc.SetCanUseStairs(true);

        // 한 프레임 기다려 NavMesh/Agent 안정화
        yield return null;

        // 문 열기
        if (doorInteractive != null)
        {
            yield return new WaitForSeconds(doorOpenDelay);
            doorInteractive.PlayInteractiveAnimation();
        }

        // 계단 아래 목적지를 NavMesh 위로 보정
        Vector3 raw = stairBottomPoint.position;
        Vector3 dest = raw;

        // 여기서는 계단 포함 AllAreas로 보정해도 OK (우리는 계단을 허용한 상태)
        if (NavMesh.SamplePosition(raw, out var hit, 2.5f, NavMesh.AllAreas))
            dest = hit.position;

        // 계단 아래까지 Scripted 이동 (계단 허용)
        pc.EnterScriptedMove(dest, allowStairs: true);

        // 도착 대기
        bool arrived = false;
        void OnArrived() => arrived = true;
        pc.Arrived += OnArrived;

        yield return new WaitUntil(() => arrived);

        if (doorInteractive != null)
        {
            doorInteractive.PlayInteractiveAnimation();
        }

        pc.Arrived -= OnArrived;

        // 도착 후 5초 인사(애니 bool + 말풍선)
        yield return StartCoroutine(ch.PlayHelloEvent());

        ch.is_call = false;
        // 인사 끝나면 배회 전환(계단 금지)
        pc.EnterAutoWanderSafely(allowStairsInWander);
    }
}