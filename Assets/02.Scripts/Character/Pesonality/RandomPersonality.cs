using System.Collections.Generic;
using UnityEngine;

namespace RandomCharacterData
{
    public static class RandomPersonality
    {
        static List<PersonalityData> peronalityDatas;

        public static PersonalityData GetRandomPersonality()
        {
            return peronalityDatas[Random.Range(0, peronalityDatas.Count)];
        }

        public static void SetPersonalityDatas(List<PersonalityData> pd)
        {
            peronalityDatas = pd;
        }
    }
}