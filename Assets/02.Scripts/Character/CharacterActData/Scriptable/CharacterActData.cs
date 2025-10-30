using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "CharacterActData", menuName = "Scriptable Objects/CharacterActData")]
public class CharacterActData : ScriptableObject
{
    [SerializeField][SerializedDictionary("Situation", "Stripts")]
    private SerializedDictionary<Situation, string[]> _dialogueDatas;
    public SerializedDictionary<Situation, string[]> DialogueDatas { get { return _dialogueDatas; } }

}
