using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Movement3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float range = 40;

    private NavMeshAgent navMeshAgent;
    private Transform targetPos;
    private Vector3 point;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetPos = GetComponent<Transform>();
    }

    public void MoveTo(Vector3 goalPosition)
    {
        // 기존에 이동 행동을 하고 있었다면 코루틴 중지
        StopCoroutine("OnMove");
        // 이동 속도 설정
        navMeshAgent.speed = moveSpeed;
        // 목표지점 설정 (목표까지의 경로 계산 후 알아서 움직인다)
        navMeshAgent.SetDestination(goalPosition);
        // 이동 행동에 대한 코루틴 시작
        StartCoroutine("OnMove");
    }

    public void ResetMoveTo()
    {
        navMeshAgent.ResetPath();
        StopCoroutine("OnMove");
    }

    IEnumerator OnMove()
    {
        while (true)
        {
            // 목표 위치(navMeshAgent.destination)와 내 위치(transform.position)의 거리가 0.1미만일 때
            // 즉, 목표 위치에 거의 도착했을 때
            if (Vector3.Distance(navMeshAgent.destination, transform.position) < 0.1f)
            {
                // 내 위치를 목표 위치로 설정
                transform.position = navMeshAgent.destination;
                // SetDestination()에 의해 설정된 경로를 초기화. 이동을 멈춘다
                navMeshAgent.ResetPath(); 
                if (RandomPoint(targetPos.position, range, out point))
                {
                    MoveTo(point);
                    //targetPos.position = point;
                }
                break;
            }

            yield return null;
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        while (true)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position; 
                MoveTo(result);
                return true;
            }
        }
    }
}