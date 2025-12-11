using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance { get; private set; }

    [SerializeField] private GameObject speechBubblePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);   // 원하면 유지
    }

    public SpeechBubble CreateBubble(Transform target)
    {
        if (speechBubblePrefab == null)
        {
            Debug.LogError("SpeechBubblePrefab is not assigned in BubbleManager.");
            return null;
        }

        GameObject bubbleObj = Instantiate(
            speechBubblePrefab,
            target.position,
            Quaternion.identity
        );

        SpeechBubble bubble = bubbleObj.GetComponent<SpeechBubble>();
        if (bubble == null)
        {
            Debug.LogError("SpeechBubble component missing on prefab.");
            return null;
        }

        bubble.target = target;
        return bubble;
    }
}