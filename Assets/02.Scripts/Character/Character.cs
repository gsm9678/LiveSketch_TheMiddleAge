using System.Collections;
using UnityEngine;
using RandomCharacterData;

[RequireComponent(typeof(PlayerController))]
public class Character : MonoBehaviour
{
    [Header("Identity")]
    public string characterName;
    public CharacterActData characterActData;
    public float CharacterPriority;

    [Header("Event")]
    [SerializeField] private float EventDelay = 2f;
    private float deltaTime;

    [Header("Look")]
    [SerializeField] private float lookSpeed = 3f;

    [Header("Refs")]
    [SerializeField] private PlayerController playerController;

    [Header("Auto Start Wander")]
    [SerializeField] private bool autoStartWander = true; // EntranceSequence 스폰은 false로 막을 것

    [Header("Greeting Animation (bool)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string helloTriggerName = "Hello";
    [SerializeField] private float helloDuration = 5f;

    [Header("Arrive Particle")]
    [SerializeField] private ParticleSystem[] arriveParticlePrefabs;
    [SerializeField] private Vector3 particleOffset = new Vector3(0f, 2.0f, 0f);

    public bool is_call = false;

    private SpeechBubble dialogueBubble;

    public void SetAutoStartWander(bool enabled) => autoStartWander = enabled;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(characterName))
            characterName = RandomNameGenerator.GenerateRandomName();

        if (characterActData == null)
            characterActData = RandomCharacterActData.GetRandomCharacterActData();

        CharacterPriority = Random.Range(0f, 100f);
        deltaTime = 0;

        playerController.Arrived += OnArrivedToDestination;

        // 미리 배치된 캐릭터는 자동 배회 진입
        if (autoStartWander)
            playerController.EnterAutoWanderSafely(allowStairs: false);
    }

    private void OnDestroy()
    {
        if (playerController != null)
            playerController.Arrived -= OnArrivedToDestination;
    }

    private void Update()
    {
        if (deltaTime < EventDelay)
            deltaTime += Time.deltaTime;
    }

    private void OnArrivedToDestination()
    {
        if (deltaTime >= EventDelay)
        {
            deltaTime = 0;

            PlayArriveParticle();
        }
    }
    public void PlayArriveParticlePublic()
    {
        deltaTime = 0;
        PlayArriveParticle();
    }
    private void PlayArriveParticle()
    {
        if (arriveParticlePrefabs == null || arriveParticlePrefabs.Length == 0)
            return;

        // 랜덤 파티클 선택
        ParticleSystem prefab =
            arriveParticlePrefabs[Random.Range(0, arriveParticlePrefabs.Length)];

        if (prefab == null) return;

        Vector3 spawnPos = transform.position + particleOffset;

        ParticleSystem ps = Instantiate(
            prefab,
            spawnPos,
            Quaternion.identity
        );

        // 캐릭터를 따라가게 하고 싶으면 부모 설정
        ps.transform.SetParent(transform);

        ps.Play();

        // 파티클 종료 후 자동 제거
        float lifeTime =
            ps.main.duration +
            ps.main.startLifetime.constantMax;

        Destroy(ps.gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Character")) return;
        if (is_call) return;
        if (deltaTime < EventDelay) return;

        Character otherChar = other.GetComponent<Character>();
        if (otherChar == null) return;
        if (otherChar.is_call) return;

        if (CharacterPriority <= otherChar.CharacterPriority) return;

        is_call = true;
        otherChar.is_call = true;

        deltaTime = 0;
        otherChar.deltaTime = 0;

        playerController.EnterIdle();
        otherChar.playerController.EnterIdle();

        StartCoroutine(StartTalking(otherChar));
    }

    public IEnumerator PlayHelloEvent()
    {
        playerController.EnterIdle();
        playerController.animationLocked = true;
        is_call = true;

        if (animator != null)
            animator.SetTrigger(helloTriggerName);

        SpeechBubble bubble = BubbleManager.Instance.CreateDialogueBubble();
        bubble.SetTarget(transform);

        yield return bubble.ShowBubble();

        string msg = GetDialogue(Situation.Hello, 0, null);
        yield return bubble.PlayLine(msg);

        yield return new WaitForSeconds(helloDuration);

        yield return bubble.HideBubble();
        Destroy(bubble.gameObject);

        playerController.animationLocked = false;
        is_call = false;
    }

    private IEnumerator StartTalking(Character partner)
    {
        StartCoroutine(SmoothLookAt(partner));
        StartCoroutine(partner.SmoothLookAt(this));

        var values = System.Enum.GetValues(typeof(TalkSituation));
        Situation situation = (Situation)values.GetValue(Random.Range(0, values.Length));

        dialogueBubble = BubbleManager.Instance.CreateDialogueBubble();
        dialogueBubble.SetTarget(transform);
        yield return dialogueBubble.ShowBubble();

        yield return Talking(situation, 0, partner, partner.characterName, dialogueBubble);

        yield return dialogueBubble.HideBubble();

        Destroy(dialogueBubble.gameObject);
        dialogueBubble = null;

        is_call = false;
        partner.is_call = false;

        CharacterPriority = Random.Range(0f, 100f);
        partner.CharacterPriority = Random.Range(0f, 100f);

        playerController.EnterAutoWanderSafely(false);
        partner.playerController.EnterAutoWanderSafely(false);
    }

    private IEnumerator SmoothLookAt(Character target)
    {
        if (target == null) yield break;

        Vector3 dir = (target.transform.position - transform.position);
        dir.y = 0f;

        Quaternion start = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * lookSpeed;
            transform.rotation = Quaternion.Slerp(start, targetRot, t);
            yield return null;
        }
    }

    private IEnumerator Talking(Situation situation, int index, Character partner, string partnerName, SpeechBubble bubble)
    {
        var lines = characterActData.DialogueDatas[situation];
        if (index >= lines.Length) yield break;

        string msg = GetDialogue(situation, index, partnerName);

        bubble.SetTarget(transform);
        yield return bubble.PlayLine(msg);

        index++;

        if (partner != null)
            yield return partner.PartnerTalking(situation, index, this, characterName, bubble);
    }

    public IEnumerator PartnerTalking(Situation situation, int index, Character partner, string partnerName, SpeechBubble bubble)
    {
        var lines = characterActData.DialogueDatas[situation];
        if (index >= lines.Length) yield break;

        string msg = GetDialogue(situation, index, partnerName);

        bubble.SetTarget(transform);
        yield return bubble.PlayLine(msg);

        index++;

        if (partner != null)
            yield return partner.Talking(situation, index, this, characterName, bubble);
    }

    private string GetDialogue(Situation situation, int index, string partnerName)
    {
        string text = characterActData.DialogueDatas[situation][index];

        if (text.Contains("(MyName)"))
            text = text.Replace("(MyName)", characterName);

        if (text.Contains("(TargetName)"))
            text = text.Replace("(TargetName)", partnerName);

        return text;
    }
}