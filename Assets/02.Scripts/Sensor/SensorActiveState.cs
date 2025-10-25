using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorActiveState : MonoBehaviour
{
    public static SensorActiveState instance;

    public bool[] SensorState; //센서의 연결상태 기록

    //싱글톤
    private void Awake()
    {
        SensorState = new bool[System.Enum.GetValues(typeof(SensorEnum)).Length];

        if (instance == null) //존재하고 있지 않을때
        {
            instance = this; //다시 최신화
            DontDestroyOnLoad(gameObject); //씬변경시 파괴 x
        }
        else
        {
            if (instance != this) //중복으로 존재할시에는 파괴! 
                Destroy(this.gameObject);
        }
    }

    //초기화
    void Start()
    {
        for (int i = 0; i < SensorState.Length; i++)
            SensorState[i] = false;
    }
}
