using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public SensorEnum sensorEnum;

    private OSCManager m_senserData;
    private SensorDataFormat SensorData;
    List<Vector3> vector3 = new List<Vector3>();

    [SerializeField]
    RectTransform SensorPosi;//맵핑할 대상

    void Start()
    {
        m_senserData = GameObject.Find("OSCManager").GetComponent<OSCManager>();
        StartCoroutine(GetSendsorData());
    }

    IEnumerator GetSendsorData()
    {
        while (true)
        {
            yield return new WaitUntil(() => SensorActiveState.instance.SensorState[(int)sensorEnum]); //센서가 연결될때까지 대기
            yield return new WaitForFixedUpdate();
            SensorData = m_senserData.SensorData[((int)sensorEnum)];//호쿠요 센서 로우 데이터 받기
            vector3.Clear();

            for (int i = 0; i < SensorData.Position.Count; i++)//호쿠요 센서 로우 데이터를 컨텐츠에서 사용할 수 있게 맵핑
            {
                vector3.Add(new Vector3(scale(-SensorData.RectSize.x / 2, SensorData.RectSize.x / 2, SensorPosi.position.x - SensorPosi.rect.width / 2, SensorPosi.position.x + SensorPosi.rect.width / 2, SensorData.Position[i].x),
                                        scale(-SensorData.RectSize.y / 2, SensorData.RectSize.y / 2, SensorPosi.position.y - SensorPosi.rect.height / 2, SensorPosi.position.y + SensorPosi.rect.height / 2, SensorData.Position[i].y),
                                        0));
            }
        }
    }
    
    #if UNITY_EDITOR
    Vector3 MousePosition;
    [SerializeField] Camera camera;

    private void Update()
    {
        if (camera != null)
        {
            SensorActiveState.instance.SensorState[((int)sensorEnum)] = true;
            if (Input.GetMouseButton(0))
            {
                vector3.Clear();
                MousePosition = Input.mousePosition;

                vector3.Add(new Vector3(scale(-camera.pixelWidth / 2, camera.pixelWidth / 2, SensorPosi.position.x - SensorPosi.rect.width / 2, SensorPosi.position.x + SensorPosi.rect.width / 2, MousePosition.x - camera.pixelWidth / 2),
                                            scale(-camera.pixelHeight / 2, camera.pixelHeight / 2, SensorPosi.position.y - SensorPosi.rect.height / 2, SensorPosi.position.y + SensorPosi.rect.height / 2, MousePosition.y - camera.pixelHeight / 2),
                                            0));
            }
        }
    }
    #endif

    //외부에서 vector3을 받기
    public List<Vector3> getSensorVector()
    {
        return vector3;
    }

    //호쿠요 메니저에서 받은 위치 데이터를 컨텐츠 위치에 맵핑
    private float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}
