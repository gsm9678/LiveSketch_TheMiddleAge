using RandomCharacterData;
using System.Collections.Generic;
using UnityEngine;

public class UsePesrnalityDialogue : MonoBehaviour
{
    public List<PersonalityDialogue> personalityDialogues;

    private void Awake()
    {
        RandomPersonality.SetPersonalityDialogues(personalityDialogues);
    }
}
