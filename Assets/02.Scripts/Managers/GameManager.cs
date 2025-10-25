using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{ 
    public Action<string> OnQRDataDetected;

    public override void Awake()    
    {
        base.Awake();
    }
}
