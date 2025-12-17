using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    public enum MoveState { Idle, Scripted, Wander }
    public MoveState State { get; private set; } = MoveState.Idle;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Wander")]
    [SerializeField] private float range = 40f;
    [SerializeField] private float minWait = 1.5f;
    [SerializeField] private float maxWait = 4.0f;

    [Header("Auto Start (for pre-placed characters)")]
    [SerializeField] private bool autoWanderOnStart = true;
    [SerializeField] private bool allowStairsInWander = false;

    [Header("NavMesh Robust")]
    [SerializeField] private float warpRadius = 3f;
    [SerializeField] private float destProjectRadius = 2.5f;
    [SerializeField] private float arriveEpsilon = 0.10f;
    [SerializeField] private int randomTries = 40;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkBoolName = "isWalking";

    [Header("Animation Control")]
    public bool animationLocked = false;

    private NavMeshAgent agent;
    private Coroutine moveCo;
    private Coroutine waitCo;

    public event Action Arrived;

    private int walkableArea = -1;
    private int stairsArea = -1;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // 간헐 초기화 문제 완화
        agent.enabled = false;
        agent.enabled = true;

        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;

        walkableArea = NavMesh.GetAreaFromName("Walkable");
        stairsArea = NavMesh.GetAreaFromName("Stairs");
    }

    private void Start()
    {
        if (autoWanderOnStart)
            EnterAutoWanderSafely(allowStairsInWander);
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        if (animationLocked) return;

        bool isWalking =
            State != MoveState.Idle &&
            agent.enabled &&
            agent.isOnNavMesh &&
            agent.velocity.sqrMagnitude > 0.01f;

        animator.SetBool(walkBoolName, isWalking);
    }

    /* =========================
     * Public API
     * ========================= */

    public void EnterIdle()
    {
        StopAllRoutines();
        State = MoveState.Idle;
        ResetPath();
    }

    public void EnterScriptedMove(Vector3 rawDestination, bool allowStairs)
    {
        StopAllRoutines();
        State = MoveState.Scripted;

        SetCanUseStairs(allowStairs);

        StartCoroutine(CoEnterWhenReady(() =>
        {
            MoveTo(rawDestination);
        }));
    }

    public void EnterAutoWander(bool allowStairs)
    {
        StopAllRoutines();
        State = MoveState.Wander;

        SetCanUseStairs(allowStairs);

        StartCoroutine(CoEnterWhenReady(() =>
        {
            MoveToRandom();
        }));
    }

    public void EnterAutoWanderSafely(bool allowStairs = false)
    {
        EnterAutoWander(allowStairs);
    }

    public void SetCanUseStairs(bool canUse)
    {
        // Walkable이 없으면 그냥 AllAreas
        if (walkableArea < 0)
        {
            agent.areaMask = NavMesh.AllAreas;
            return;
        }

        int mask = 1 << walkableArea;
        if (canUse && stairsArea >= 0)
            mask |= 1 << stairsArea;

        agent.areaMask = mask;
    }

    /* =========================
     * Core
     * ========================= */

    private IEnumerator CoEnterWhenReady(Action onReady)
    {
        yield return new WaitUntil(() => agent != null && agent.enabled && gameObject.activeInHierarchy);

        // agent.areaMask 기준으로만 NavMesh 보정(중요)
        float t = 0f;
        while (!agent.isOnNavMesh && t < 2.0f)
        {
            t += Time.deltaTime;
            ForceSnapToNavMesh();
            yield return null;
        }

        yield return null;
        onReady?.Invoke();
    }

    private void MoveToRandom()
    {
        if (State != MoveState.Wander) return;

        if (!TryGetRandomPoint(transform.position, range, out var dest))
        {
            if (!TryGetRandomPoint(transform.position, range * 1.5f, out dest))
                return;
        }

        if (Vector3.Distance(transform.position, dest) < 0.6f)
        {
            if (TryGetRandomPoint(transform.position, range, out var dest2) &&
                Vector3.Distance(transform.position, dest2) >= 0.6f)
                dest = dest2;
        }

        MoveTo(dest);
    }

    private void MoveTo(Vector3 rawDestination)
    {
        StopMoveOnly();
        ForceSnapToNavMesh();

        agent.speed = moveSpeed;
        agent.isStopped = false;

        // 핵심: destination을 NavMesh 위로 보정해서 SetDestination
        if (!TrySetDestinationOnNavMesh(rawDestination))
        {
            if (State == MoveState.Wander)
                MoveToRandom();
            else
                ResetPath();
            return;
        }

        moveCo = StartCoroutine(CoMonitorMove());
    }

    private IEnumerator CoMonitorMove()
    {
        while (true)
        {
            if (!agent.enabled)
            {
                yield return null;
                continue;
            }

            if (!agent.isOnNavMesh)
            {
                ForceSnapToNavMesh();
                yield return null;
                continue;
            }

            if (!agent.pathPending)
            {
                if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    if (State == MoveState.Wander)
                        MoveToRandom();
                    else
                        ResetPath();

                    yield break;
                }

                bool arrived =
                    agent.remainingDistance <= agent.stoppingDistance + arriveEpsilon &&
                    agent.velocity.sqrMagnitude < 0.01f;

                if (arrived)
                {
                    ResetPath();
                    moveCo = null;

                    Arrived?.Invoke();

                    if (State == MoveState.Wander)
                        waitCo = StartCoroutine(CoWaitThenNext());

                    yield break;
                }

                // Wander인데 hasPath=false로 풀리면 즉시 복구
                if (State == MoveState.Wander && !agent.hasPath)
                {
                    MoveToRandom();
                    yield break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator CoWaitThenNext()
    {
        float wait = UnityEngine.Random.Range(minWait, maxWait);
        yield return new WaitForSeconds(wait);

        if (State == MoveState.Wander)
            MoveToRandom();

        waitCo = null;
    }

    /* =========================
     * NavMesh helpers
     * ========================= */

    private void ForceSnapToNavMesh()
    {
        if (agent == null) return;
        if (agent.isOnNavMesh) return;

        // NavMesh.AllAreas
        // agent.areaMask (현재 허용 영역 기준으로만 스냅)
        if (NavMesh.SamplePosition(transform.position, out var hit, warpRadius, agent.areaMask))
        {
            transform.position = hit.position;
            agent.Warp(hit.position);
        }
    }

    private bool TrySetDestinationOnNavMesh(Vector3 rawDestination)
    {
        // 목적지도 현재 허용 영역(agent.areaMask) 기준으로 보정
        if (NavMesh.SamplePosition(rawDestination, out NavMeshHit hit, destProjectRadius, agent.areaMask))
        {
            agent.SetDestination(hit.position);
            return true;
        }

        // 보조 시도: y를 내 위치로 맞춰 샘플링
        Vector3 fallback = rawDestination;
        fallback.y = transform.position.y;

        if (NavMesh.SamplePosition(fallback, out hit, destProjectRadius, agent.areaMask))
        {
            agent.SetDestination(hit.position);
            return true;
        }

        return false;
    }

    private bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < randomTries; i++)
        {
            Vector3 p = center + UnityEngine.Random.insideUnitSphere * radius;
            p.y = center.y; // y 튐 방지

            if (NavMesh.SamplePosition(p, out var hit, 1.5f, agent.areaMask))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }

    /* =========================
     * Routine helpers
     * ========================= */

    private void StopAllRoutines()
    {
        StopMoveOnly();

        if (waitCo != null)
        {
            StopCoroutine(waitCo);
            waitCo = null;
        }
    }

    private void StopMoveOnly()
    {
        if (moveCo != null)
        {
            StopCoroutine(moveCo);
            moveCo = null;
        }

        ResetPath();
    }

    private void ResetPath()
    {
        if (agent != null && agent.enabled)
            agent.ResetPath();
    }
}