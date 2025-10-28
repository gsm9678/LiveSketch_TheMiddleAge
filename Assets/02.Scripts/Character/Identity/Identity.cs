using System;
using UnityEngine;
using RandomCharacterData;

abstract class Identity : MonoBehaviour
{
    [SerializeField] string _name = null;
    [SerializeField] PersonalityDialogue _personality = null;

    private void Start()
    {
        if (_name == "")
        {
            _name = RandomNameGenerator.GenerateRandomName();
        }
        if(_personality == null)
        {
            _personality = RandomPersonality.GetRandomPersonality();
        }
    }

}
