using UnityEngine;

namespace RandomCharacterData
{
    public static class RandomNameGenerator
    {
        static readonly string[] FirstName = { "김", "이", "박", "최", "정", "강", "조", "윤", "장", "한", "오", "서", "신", "허", "공" };
        static readonly string[] LastName = { "서준", "민준", "도윤", "시우", "예준", "하준", "지호", "주원", "지후", "도현", "서윤", "서연", "지우", "하윤", "서현", "하은", "민서", "지유", "윤서", "채원" };

        public static string GenerateRandomName()
        {
            int FirstNameIndex = Random.Range(0, FirstName.Length);
            int LastNameIndex = Random.Range(0, LastName.Length);

            return FirstName[FirstNameIndex] + LastName[LastNameIndex];
        }
    }
}
