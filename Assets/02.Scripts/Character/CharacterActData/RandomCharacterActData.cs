using System.Collections.Generic;
using UnityEngine;

namespace RandomCharacterData
{
    public static class RandomCharacterActData
    {
        static List<CharacterActData> CharacterActDatas;

        public static CharacterActData GetRandomCharacterActData()
        {
            return CharacterActDatas[Random.Range(0, CharacterActDatas.Count)];
        }

        public static void SetCharacterActDatas(List<CharacterActData> pd)
        {
            CharacterActDatas = pd;
        }
    }
}