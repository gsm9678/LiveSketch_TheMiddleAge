using System;
using UnityEngine;
using RandomCharacterData;

abstract public class CharacterBase : MonoBehaviour
{
    [SerializeField] string _name = null;
    [SerializeField] PersonalityData _personality = null;

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
