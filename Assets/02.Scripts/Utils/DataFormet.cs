using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CharacterData
{
    public string id;
    public GameObject Prefab;
}

#region Identity
public enum Situation { OneSelf, Hello, Game }

public enum Identity { HappyPerson, AngryPerson, SadPerson, TiredPerson, FearPerson }

[Serializable]
public class Dialogue
{
    public Situation situations;

    public string[] _dialoue;
}

[Serializable]
public class IdentityData
{
    public Identity identity;

    public Dialogue[] Dialogues;
}
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