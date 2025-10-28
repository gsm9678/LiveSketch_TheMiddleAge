using RandomCharacterData;
using UnityEngine;

[CreateAssetMenu(fileName = "PeronalityDialogue", menuName = "Scriptable Objects/PeronalityDialogue")]
public class PersonalityDialogue : ScriptableObject
{
    [SerializeField]
    private Personality _personality;
    public Personality personality { get { return _personality; } }
    [SerializeField]
    private Dialogue[] _dialogues = new Dialogue[System.Enum.GetValues(typeof(Situation)).Length];
    public Dialogue[] dialogues { get { return _dialogues; } }
}
