using UnityEngine;

//OSC 통신을 통해 HokuyoManage으로부터 데이터 받아오는 스크립트
public class OSCManager : Singleton<OSCManager>
{
    public OSC _isOSC;

    public SensorDataFormat[] SensorData; //호쿠요로부터 받은 데이터를 저장

    override public void Awake()
    {
        base.Awake();
    }

    //초기화
    private void Start()
    {
        SetOSC_EventHandler();
        SensorData =  new SensorDataFormat[System.Enum.GetValues(typeof(SensorEnum)).Length];
        for (int i = 0; i < SensorData.Length; i++)
            SensorData[i] = new SensorDataFormat();
    }

    #region FrontSensor
    //센서 연결상태 최신화, 호쿠요 메니저에서 설정한 방 크기 값 받기
    public void GetFrontStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Front)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Front)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Front)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Front)] = true;
            Debug.Log("정면 센서 연결");
        }
    }

    //인식한 물체의 위치값 받기
    public void GetFrontSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Front)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
    }

    public void GetFrontStopMessage(OscMessage message)
    {
    }

    //센서 종료 신호 받기
    public void FrontSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Front)] = false;
        Debug.Log("정면 센서 종료");
    }
    #endregion

    #region BackSensor
    public void GetBackStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Back)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Back)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Back)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Back)] = true;
            Debug.Log("후면 센서 연결");
        }
    }

    public void GetBackSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Back)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
        //Debug.Log(SensorData.Position.Count);
    }

    public void GetBackStopMessage(OscMessage message)
    {
        //Debug.Log(message.GetInt(0));
    }

    public void BackSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Back)] = false;
        Debug.Log("후면 센서 종료");
    }
    #endregion

    #region RightSensor
    public void GetRightStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Right)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Right)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Right)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Right)] = true;
            Debug.Log("우면 센서 연결");
        }
    }

    public void GetRightSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Right)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
        //Debug.Log(SensorData.Position.Count);
    }

    public void GetRightStopMessage(OscMessage message)
    {
        //Debug.Log(message.GetInt(0));
    }

    public void RightSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Right)] = false;
        Debug.Log("우면 센서 종료");
    }
    #endregion

    #region LeftSensor
    public void GetLeftStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Left)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Left)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Left)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Left)] = true;
            Debug.Log("좌면 센서 연결");
        }
    }

    public void GetLeftSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Left)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
        //Debug.Log(SensorData.Position.Count);
    }

    public void GetLeftStopMessage(OscMessage message)
    {
        //Debug.Log(message.GetInt(0));
    }

    public void LeftSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Left)] = false;
        Debug.Log("좌면 센서 종료");
    }
    #endregion

    #region DownSensor
    public void GetDownStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Down)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Down)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Down)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Down)] = true;
            Debug.Log("바닥 센서 연결");
        }
    }

    public void GetDownSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Down)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
    }

    public void GetDownStopMessage(OscMessage message)
    {
    }

    public void DownSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Down)] = false;
        Debug.Log("바닥 센서 종료");
    }
    #endregion

    void SetOSC_EventHandler()
    {
        _isOSC.SetAddressHandler("/Front/Start", GetFrontStartMessage);
        _isOSC.SetAddressHandler("/Front/Data", GetFrontSensorMessage);
        _isOSC.SetAddressHandler("/Front/End", GetFrontStopMessage);
        _isOSC.SetAddressHandler("/Front/Quit", FrontSensorQuit);
        _isOSC.SetAddressHandler("/Back/Start", GetBackStartMessage);
        _isOSC.SetAddressHandler("/Back/Data", GetBackSensorMessage);
        _isOSC.SetAddressHandler("/Back/End", GetBackStopMessage);
        _isOSC.SetAddressHandler("/Back/Quit", BackSensorQuit);
        _isOSC.SetAddressHandler("/Left/Start", GetLeftStartMessage);
        _isOSC.SetAddressHandler("/Left/Data", GetLeftSensorMessage);
        _isOSC.SetAddressHandler("/Left/End", GetLeftStopMessage);
        _isOSC.SetAddressHandler("/Left/Quit", LeftSensorQuit);
        _isOSC.SetAddressHandler("/Right/Start", GetRightStartMessage);
        _isOSC.SetAddressHandler("/Right/Data", GetRightSensorMessage);
        _isOSC.SetAddressHandler("/Right/End", GetRightStopMessage);
        _isOSC.SetAddressHandler("/Right/Quit", RightSensorQuit);
        _isOSC.SetAddressHandler("/Down/Start", GetDownStartMessage);
        _isOSC.SetAddressHandler("/Down/Data", GetDownSensorMessage);
        _isOSC.SetAddressHandler("/Down/End", GetDownStopMessage);
        _isOSC.SetAddressHandler("/Down/Quit", DownSensorQuit);
    }
}
