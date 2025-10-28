using System.Collections.Generic;
using UnityEngine;

namespace RandomCharacterData
{
    public static class RandomPersonality
    {
        static List<PersonalityDialogue> peronalityDialogues;

        public static PersonalityDialogue GetRandomPersonality()
        {
            Debug.Log(peronalityDialogues.Count);
            return peronalityDialogues[Random.Range(0, peronalityDialogues.Count)];
        }

        public static void SetPersonalityDialogues(List<PersonalityDialogue> pd)
        {
            peronalityDialogues = pd;
        }
    }
}