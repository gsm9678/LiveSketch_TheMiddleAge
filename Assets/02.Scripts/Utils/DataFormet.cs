using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CharacterData
{
    public string id;
    public GameObject Prefab;
}

#region Personality
public enum Situation { OneSelf, Hello, Game }

public enum PersonalityEnum { HappyPerson, AngryPerson, SadPerson, TiredPerson, FearPerson }
#endregion

#region Sensor
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
#endregion