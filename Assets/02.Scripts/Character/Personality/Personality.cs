using System;
using UnityEngine;
using RandomName;

abstract class Personality : MonoBehaviour
{
    [SerializeField] string _name = null;
    [SerializeField] Identity identity;

    private void Start()
    {
        if (_name == null)
        {
            _name = RandomNameGenerator.GenerateRandomName();
        }
    }

}
