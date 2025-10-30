using System;
using UnityEngine;
using RandomCharacterData;

abstract public class CharacterBase : MonoBehaviour
{
    [SerializeField] string _name = null;
    [SerializeField] CharacterActData _characterActData = null;

    private void Start()
    {
        if (_name == "")
        {
            _name = RandomNameGenerator.GenerateRandomName();
        }
        if(_characterActData == null)
        {
            _characterActData = RandomCharacterActData.GetRandomCharacterActData();
        }
    }
}
