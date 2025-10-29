using System.Collections.Generic;
using UnityEngine;

public class RayCastPointGenerate : MonoBehaviour
{
    public SensorEnum sensorEnum;
    private Vector3[] origins;
    private Vector3[] directions;

    [SerializeField] private SensorManager sensorManager;

    [SerializeField] private Transform Tr_Origin;//생성한 프리팹 트렌스폼
    [SerializeField] private Transform Tr_Direction;//생성한 프리팹 트렌스폼

    [SerializeField] private float length = 100f;

    void Update()
    {
        if (SensorActiveState.instance.SensorState[((int)sensorEnum)])//호쿠요 메니저가 연결되었으면
        {
            List<Vector3> points = sensorManager.GetSensorVector();
            origins = new Vector3[points.Count];
            directions = new Vector3[points.Count];

            for (int i = 0; i < points.Count; i++)//오브젝트 풀링, 센서 위치에 이동
            {
                origins[i] = (Tr_Origin.TransformPoint(points[i]));
                directions[i] = (Tr_Direction.TransformPoint(points[i]));
                Vector3 dir = directions[i] - origins[i];
                dir.Normalize();
                Physics.Raycast(origins[i], dir, length);
                Debug.DrawRay(origins[i], dir * length);
            }
        }
    }
}

