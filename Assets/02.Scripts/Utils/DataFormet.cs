using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CharacterData
{
    public string id;
    public GameObject Prefab;
}

public enum Situation { OneSelf, Hello, Talk1, Talk2 }
public enum TalkSituation { Talk1 = Situation.Talk1, Talk2 = Situation.Talk2 }
public enum MoveState
{
    Idle,
    Wander,
    Scripted
}
#region Sensor
public class SensorDataFormat
{
    public Vector2 RectSize;
    public List<Vector3> Position = new();
}

public enum SensorEnum
{
    Front,
    Back,
    Right,
    Left,
    Down
}
#endregion