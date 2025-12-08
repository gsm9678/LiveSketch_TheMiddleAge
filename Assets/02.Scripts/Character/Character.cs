using System.Collections;
using UnityEngine;
using RandomCharacterData;

public class Character : MonoBehaviour
{
    public string characterName = null;
    public CharacterActData characterActData = null;
    public float CharacterPriority = 0;

    private bool is_call = false;

    private void Start()
    {
        if (characterName == "")
        {
            characterName = RandomNameGenerator.GenerateRandomName();
        }
        if (characterActData == null)
        {
            characterActData = RandomCharacterActData.GetRandomCharacterActData();
        }
        Debug.Log(GetDialogue(Situation.Hello, 0));

        CharacterPriority = Random.Range(0f, 100f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (CharacterPriority > other.GetComponent<Character>().CharacterPriority)
            {
                if (!is_call)
                {
                    StartCoroutine(StartTalking(other.GetComponent<Character>()));
                }
            }
        }
    }


    public IEnumerator StartTalking(Character partner)
    {
        var values = System.Enum.GetValues(typeof(TalkSituation));
        int random = Random.Range(0, values.Length);

        Situation situation = (Situation)(int)values.GetValue(random);

        is_call = true;

        yield return(Talking(situation, 0, partner, partner.characterName));

        is_call = false;

        CharacterPriority = Random.Range(0f, 100f);

        yield return null;
    }

    private IEnumerator Talking(Situation situation, int DialogueIndex, Character partner, string partnerName)
    {
        var lines = characterActData.DialogueDatas[situation];
        if (DialogueIndex >= lines.Length) yield break;

        Debug.Log(characterName + "\n" + GetDialogue(situation, DialogueIndex, partnerName));

        DialogueIndex++;

        yield return(partner.Talking(situation, DialogueIndex, this, characterName));
    }

    private string GetDialogue(Situation situation, int DialogueIndex, string PartnerName = null)
    {
        string returnString = characterActData.DialogueDatas[situation][DialogueIndex];

        if (returnString.Contains("(MyName)"))
            returnString = returnString.Replace("(MyName)", characterName);
        else if (returnString.Contains("(TargetName)"))
            returnString = returnString.Replace("(TargetName)", PartnerName);

        return returnString;
    }
}
