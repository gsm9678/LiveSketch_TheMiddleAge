using RandomCharacterData;
using System.Collections.Generic;
using UnityEngine;

public class UsePesrnalityData : MonoBehaviour
{
    public List<PersonalityData> personalityDatas;

    private void Awake()
    {
        RandomPersonality.SetPersonalityDatas(personalityDatas);
    }
}
