using RandomCharacterData;
using System.Collections.Generic;
using UnityEngine;

public class UseCharacterActData : MonoBehaviour
{
    public List<CharacterActData> CharacterActDataDatas;

    private void Awake()
    {
        RandomCharacterActData.SetCharacterActDatas(CharacterActDataDatas);
    }
}
