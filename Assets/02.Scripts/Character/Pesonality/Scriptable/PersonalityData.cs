using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "PersonalityData", menuName = "Scriptable Objects/PersonalityData")]
public class PersonalityData : ScriptableObject
{
    [SerializeField]
    private PersonalityEnum _personality;
    public PersonalityEnum personality { get { return _personality; } }

    [SerializeField][SerializedDictionary("Situation", "Stripts")]
    private SerializedDictionary<Situation, string[]> _talkDatas;
    public SerializedDictionary<Situation, string[]> TalkDatas { get { return _talkDatas; } }

}
