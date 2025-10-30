using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "CharacterActData", menuName = "Scriptable Objects/CharacterActData")]
public class CharacterActData : ScriptableObject
{
    [SerializeField][SerializedDictionary("Situation", "Stripts")]
    private SerializedDictionary<Situation, string[]> _talkDatas;
    public SerializedDictionary<Situation, string[]> TalkDatas { get { return _talkDatas; } }

}
