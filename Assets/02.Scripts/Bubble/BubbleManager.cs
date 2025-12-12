using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public static BubbleManager Instance { get; private set; }

    [SerializeField] private GameObject dialogueBubblePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public SpeechBubble CreateDialogueBubble()
    {
        GameObject obj = Instantiate(dialogueBubblePrefab, Vector3.zero, Quaternion.identity);
        return obj.GetComponent<SpeechBubble>();
    }
}