using UnityEngine;
using RandomCharacterData;

public abstract class CharacterBase : MonoBehaviour
{
    public string characterName = null;
    public CharacterActData characterActData = null;
    public float CharacterPriority = 0;

    private void Start()
    {
        if (characterName == "")
        {
            characterName = RandomNameGenerator.GenerateRandomName();
        }
        if(characterActData == null)
        {
            characterActData = RandomCharacterActData.GetRandomCharacterActData();
        }
        Debug.Log(GetDialogue(Situation.Hello, 0));

        CharacterPriority = Random.Range(0f, 100f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Character"))
        {
            if (CharacterPriority > other.GetComponent<CharacterBase>().CharacterPriority)
            {
                StartTalking(other.GetComponent<CharacterBase>());
            }
        }
    }

    protected abstract void TriggerEvent();

    public void StartTalking(CharacterBase partner)
    {
        var values = System.Enum.GetValues(typeof(TalkSituation));
        int random = Random.Range(0, values.Length);

        Situation situation = (Situation)(int)values.GetValue(random);

        Talking(situation, 0, partner, partner.characterName);
    }

    private void Talking(Situation situation, int DialogueIndex, CharacterBase partner, string partnerName)
    {
        var lines = characterActData.DialogueDatas[situation];
        if (DialogueIndex >= lines.Length) return;

        Debug.Log(characterName + "\n" + GetDialogue(situation, DialogueIndex, partnerName));

        DialogueIndex++;
        
        partner.Talking(situation, DialogueIndex, this, characterName);
    }

    protected string GetDialogue(Situation situation, int DialogueIndex, string PartnerName = null)
    {
        string returnString = characterActData.DialogueDatas[situation][DialogueIndex];

        if (returnString.Contains("(MyName)"))
            returnString = returnString.Replace("(MyName)", characterName);
        else if(returnString.Contains("(TargetName)"))
            returnString = returnString.Replace("(TargetName)", PartnerName);

        return returnString;
    }
}
