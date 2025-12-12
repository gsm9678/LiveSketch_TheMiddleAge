using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Wander")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float range = 40f;

    [Header("Start Option")]
    [SerializeField] private bool autoWanderOnStart = true; // 기본 true (기존 동작 유지)

    private NavMeshAgent navMeshAgent;
    private Transform targetPos;
    private Vector3 point;

    // 기존: 도착하면 Character가 받는 콜백
    public Action action = null;

    // 지금 배회 중인지 여부
    public bool IsWandering { get; private set; }

    private Coroutine moveRoutine;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetPos = transform;
    }

    private void Start()
    {
        if (autoWanderOnStart)
            StartWander();
    }

    // 배회 시작 (랜덤 목적지 반복)
    public void StartWander()
    {
        IsWandering = true;
        SetCanUseStairs(false);   // 평소엔 계단 사용 금지
        MoveToRandom();
    }

    // 배회 정지
    public void StopWander()
    {
        IsWandering = false;
        ResetMoveTo();
    }

    // 랜덤 목적지로 이동(배회용)
    public void MoveToRandom()
    {
        if (!IsWandering) return;

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.isStopped = false;

        if (RandomPoint(targetPos.position, range, out point))
        {
            MoveToPoint(point);
        }
    }

    // 특정 지점으로 단발 이동(연출용/지정 이동용)
    public void MoveToPoint(Vector3 destination)
    {
        ResetMoveTo();

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);

        moveRoutine = StartCoroutine(OnMove());
    }

    public void ResetMoveTo()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        if (navMeshAgent != null)
            navMeshAgent.ResetPath();
    }

    private IEnumerator OnMove()
    {
        while (true)
        {
            if (!navMeshAgent.pathPending)
            {
                // NavMeshAgent.remainingDistance가 더 안정적
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.05f)
                {
                    navMeshAgent.ResetPath();

                    action?.Invoke(); // Character가 “도착” 이벤트 받음

                    // 배회 모드일 때만 다음 랜덤 목적지로 반복
                    if (IsWandering)
                        MoveToRandom();

                    moveRoutine = null;
                    yield break;
                }
            }

            yield return null;
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }

    public void SetCanUseStairs(bool canUse)
    {
        if (canUse)
        {
            // Walkable + Stairs
            navMeshAgent.areaMask =
                NavMesh.GetAreaFromName("Walkable") >= 0
                ? (1 << NavMesh.GetAreaFromName("Walkable")) |
                  (1 << NavMesh.GetAreaFromName("Stairs"))
                : NavMesh.AllAreas;
        }
        else
        {
            // Walkable만
            navMeshAgent.areaMask =
                1 << NavMesh.GetAreaFromName("Walkable");
        }
    }
}