using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private Movement3D movement3D;
    private Transform targetPos;

    [SerializeField] private float range = 40;
    private Vector3 point;

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        targetPos = GetComponent<Transform>();
    }

    private void Start()
    {
        StartMoveTo();
    }

    private void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            if (RandomPoint(targetPos.position, range, out point))
            {
                movement3D.MoveTo(point);
                //targetPos.position = point;
            }
        }
    }

    public void StartMoveTo()
    {
        if (RandomPoint(targetPos.position, range, out point))
        {
            movement3D.MoveTo(point);
            //targetPos.position = point;
        }
    }

    public void StartEvent()
    {
        movement3D.ResetMoveTo();
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
                return true;
            }
        }
    }
}