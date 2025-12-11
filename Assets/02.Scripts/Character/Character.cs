using System.Collections;
using UnityEngine;
using RandomCharacterData;

public class Character : MonoBehaviour
{
    public string characterName = null;
    public CharacterActData characterActData = null;
    public float CharacterPriority = 0;
    [SerializeField] float EventDelay;
    private float deltaTime = 0;

    [SerializeField] private PlayerController playerController;

    private bool is_call = false;

    // 말풍선 참조
    private SpeechBubble myBubble;
    private SpeechBubble partnerBubble;

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
        if (playerController == null)
        {
            playerController = gameObject.AddComponent<PlayerController>();
        }
        Debug.Log(GetDialogue(Situation.Hello, 0));

        CharacterPriority = Random.Range(0f, 100f);
        deltaTime = EventDelay;

        playerController.action += OnArrivedToDestination;
    }

    private void Update()
    {
        if (deltaTime < EventDelay)
        {
            deltaTime += Time.deltaTime;
        }
    }

    private void OnArrivedToDestination()
    {
        if (deltaTime >= EventDelay)
        {
            Debug.Log($"{characterName} :: EventDelay 이후 랜덤 목적지에 도착했습니다!");
            deltaTime = 0;
            playerController.MoveTo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (!is_call && deltaTime >= EventDelay)
            {
                deltaTime = 0;
                playerController.ResetMoveTo();

                Character otherChar = other.GetComponent<Character>();

                if (otherChar != null &&
                    CharacterPriority > otherChar.CharacterPriority)
                {
                    StartCoroutine(StartTalking(otherChar));
                }
            }
        }
    }

    //  여기서부터 말풍선 + 대화 시작
    public IEnumerator StartTalking(Character partner)
    {
        var values = System.Enum.GetValues(typeof(TalkSituation));
        int random = Random.Range(0, values.Length);

        Situation situation = (Situation)(int)values.GetValue(random);

        is_call = true;

        // 양쪽 캐릭터에 말풍선 생성
        myBubble = BubbleManager.Instance.CreateBubble(transform);
        partnerBubble = BubbleManager.Instance.CreateBubble(partner.transform);

        yield return Talking(situation, 0, partner, partner.characterName);

        // 대화 종료 처리
        is_call = false;
        CharacterPriority = Random.Range(0f, 100f);

        if (myBubble != null) Destroy(myBubble.gameObject);
        if (partnerBubble != null) Destroy(partnerBubble.gameObject);

        playerController.MoveTo();
        partner.playerController.MoveTo();
        yield return null;
    }

    /// <summary>
    /// 재귀적으로 주고받는 대화 코루틴
    /// </summary>
    private IEnumerator Talking(Situation situation, int DialogueIndex, Character partner, string partnerName)
    {
        var lines = characterActData.DialogueDatas[situation];
        if (DialogueIndex >= lines.Length) yield break;

        // 내 대사
        string msg = GetDialogue(situation, DialogueIndex, partnerName);
        Debug.Log(characterName + "\n" + msg);

        if (myBubble != null)
        {
            // Text Animator 3.x 타입라이터로 말풍선 출력
            yield return myBubble.PlayText(msg, 0.3f);
        }
        else
        {
            // 말풍선이 없으면 기존처럼 딜레이만 유지
            yield return new WaitForSeconds(1f);
        }

        DialogueIndex++;

        // 상대 캐릭터에게 턴 넘기기
        if (partner != null)
        {
            yield return partner.PartnerTalking(situation, DialogueIndex, this, characterName);
        }
    }

    /// <summary>
    /// 상대 캐릭터가 말하는 쪽 (같은 패턴, 말풍선만 partnerBubble 사용)
    /// </summary>
    public IEnumerator PartnerTalking(Situation situation, int DialogueIndex, Character partner, string partnerName)
    {
        var lines = characterActData.DialogueDatas[situation];
        if (DialogueIndex >= lines.Length) yield break;

        string msg = GetDialogue(situation, DialogueIndex, partnerName);
        Debug.Log(characterName + "\n" + msg);

        if (partnerBubble != null)
        {
            yield return partnerBubble.PlayText(msg, 0.3f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        DialogueIndex++;

        if (partner != null)
        {
            yield return partner.Talking(situation, DialogueIndex, this, characterName);
        }
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