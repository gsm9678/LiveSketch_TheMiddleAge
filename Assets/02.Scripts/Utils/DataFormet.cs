using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Character
{
    public string id;
    public GameObject Prefab;
}

public class SensorDataFormat
{
    public Vector2 RectSize;
    public List<Vector3> Position = new List<Vector3>();
}

public enum SensorEnum
{
    Front,
    Back,
    Right,
    Left,
    Down
}