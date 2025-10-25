using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TouchPointGenerate : MonoBehaviour
{
    public SensorEnum sensorEnum;
    private List<GameObject> TouchPoints = new List<GameObject>();

    [SerializeField] private SensorManager sensorManager;
    [SerializeField] private GameObject TouchPoint;//생성할 프리팹

    [SerializeField] private Transform TouchPointBasket;//생성한 프리팹 트렌스폼

    // Update is called once per frame
    void Update()
    {
        if (SensorActiveState.instance.SensorState[((int)sensorEnum)])//호쿠요 메니저가 연결되었으면
        {
            if(TouchPoint != null)
            {
                if(sensorManager.getSensorVector().Count > TouchPoints.Count)//오브젝트 풀링
                {
                    for (int i = TouchPoints.Count; i < sensorManager.getSensorVector().Count; i++)
                    {
                        TouchPoints.Add(Instantiate(TouchPoint, TouchPointBasket));
                    }
                }
                else if(sensorManager.getSensorVector().Count < TouchPoints.Count)//오브젝트 풀링
                {
                    for(int i = sensorManager.getSensorVector().Count; i < TouchPoints.Count; i++)
                    {
                        TouchPoints[i].SetActive(false);
                    }
                }

                for(int i = 0; i < sensorManager.getSensorVector().Count; i++)//오브젝트 풀링, 센서 위치에 이동
                {
                    TouchPoints[i].SetActive(true);
                    TouchPoints[i].transform.localPosition = sensorManager.getSensorVector()[i];
                }
            }
        }
        else//호쿠요 메니저가 연결이 안되어있으면 모든 오브젝트 False
        {
            for(int i = 0; i < TouchPoints.Count; i++)
            {
                TouchPoints[i].SetActive(false);
            }
        }
    }
}