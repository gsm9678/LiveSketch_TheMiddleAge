using UnityEngine;

public class CharacterSelecter : MonoBehaviour
{
    [Header("생성할 캐릭터 설정")]
    [SerializeField] Character[] Characters;

    private void Start()
    {
        GameManager.Instance.OnQRDataDetected += GenerateCharacter;
    }

    public void GenerateCharacter(string id)
    {
        GameObject go = null;

        foreach (Character character in Characters)
        {
            if(character.id == id)
            {
                go = character.Prefab;
                break;
            }
        }

        Instantiate(go, this.transform);
    }
}
