using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Suntail;

public class EntranceSequence : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform spawnPointInside;     // 문 뒤(실내)
    [SerializeField] private Transform stairBottomPoint;     // 계단 아래(바깥)

    [Header("Door")]
    [SerializeField] private Interactive doorInteractive;    // 네 Interactive(문) 넣기
    [SerializeField] private float openDelay = 0.15f;

    [Header("Timing")]
    [SerializeField] private float arriveDistance = 0.5f;
    [SerializeField] private float timeout = 12f;

    private void Start()
    {
        Play();
    }
    public void Play()
    {
        StartCoroutine(CoPlay());
    }

    private IEnumerator CoPlay()
    {
        // 1) 스폰
        GameObject go = Instantiate(characterPrefab, spawnPointInside.position, spawnPointInside.rotation);

        // 2) 컴포넌트 가져오기
        var pc = go.GetComponent<PlayerController>();
        var agent = go.GetComponent<NavMeshAgent>();

        if (pc == null || agent == null)
        {
            Debug.LogError("[EntranceSequence] Spawned character needs PlayerController + NavMeshAgent.");
            yield break;
        }

        // 3) 배회 끄기 (스폰 즉시 돌아다니는 것 방지)
        pc.StopWander();

        pc.SetCanUseStairs(true);

        // 4) 문 열기
        if (doorInteractive != null)
        {
            yield return new WaitForSeconds(openDelay);
            doorInteractive.PlayInteractiveAnimation(); // openAnimationName 재생
        }

        // 5) 계단 아래로 단발 이동
        pc.MoveToPoint(stairBottomPoint.position);

        // 6) 도착 대기 (타임아웃 포함)
        float t = 0f;
        while (t < timeout)
        {
            t += Time.deltaTime;

            float dist = Vector3.Distance(go.transform.position, stairBottomPoint.position);
            if (dist <= arriveDistance)
                break;

            yield return null;
        }
        if (doorInteractive != null)
        {
            doorInteractive.PlayInteractiveAnimation(); // openAnimationName 재생
        }

        pc.SetCanUseStairs(false);
        // 7) 배회 시작
        pc.StartWander();
    }
}