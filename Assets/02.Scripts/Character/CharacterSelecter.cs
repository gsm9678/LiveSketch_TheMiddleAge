using UnityEngine;

public class CharacterSelecter : MonoBehaviour
{
    [Header("생성할 캐릭터 설정")]
    [SerializeField] CharacterData[] Characters;
    [SerializeField] EntranceSequence EntranceSequence;

    private void Start()
    {
        GameManager.Instance.OnQRDataDetected += GenerateCharacter;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnQRDataDetected -= GenerateCharacter;
    }

    public void GenerateCharacter(string id)
    {
        GameObject go = null;

        foreach (CharacterData character in Characters)
        {
            if(character.id == id)
            {
                go = character.Prefab;
                break;
            }
        }
        EntranceSequence.Play(go);

    }
}
