using System;
using UnityEngine;
using RandomCharacterData;

public abstract class CharacterBase : MonoBehaviour
{
    public string characterName = null;
    public CharacterActData characterActData = null;

    private void Start()
    {
        if (characterName == "")
        {
            characterName = RandomNameGenerator.GenerateRandomName();
        }
        if(characterActData == null)
        {
            characterActData = RandomCharacterActData.GetRandomCharacterActData();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerEvent();
    }

    protected abstract void TriggerEvent();

    protected string GetDialogue(Situation situation, int DialogueIndex)
    {
        return characterActData.DialogueDatas[situation][DialogueIndex];
    }
}
