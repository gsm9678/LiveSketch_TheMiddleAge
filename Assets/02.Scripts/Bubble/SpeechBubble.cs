using System.Collections;
using UnityEngine;
using Febucci.TextAnimatorForUnity; // 3.x 네임스페이스
                                    // (TypewriterComponent 여기에 있음)

public class SpeechBubble : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.2f, 0f);

    [Header("Text Animator")]
    [SerializeField] private TypewriterComponent typewriter;  // 인스펙터에 할당
    [SerializeField] private float minDisplayTime = 0.5f;      // 문장 한 줄 유지 시간

    private void Awake()
    {
        // 혹시라도 동적으로 붙일 경우를 대비한 안전장치
        if (typewriter == null)
            typewriter = GetComponentInChildren<TypewriterComponent>();
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            // 필요하면 카메라 방향으로 LookAt도 가능
            // transform.forward = (transform.position - Camera.main.transform.position).normalized;
        }
    }

    /// <summary>
    /// 한 줄 말하기 (타이핑 + 잠깐 유지)
    /// </summary>
    public IEnumerator PlayText(string message, float extraWait = 0.5f)
    {
        if (typewriter == null)
        {
            Debug.LogWarning("TypewriterComponent is missing on SpeechBubble.", this);
            yield break;
        }

        // Text Animator 3.x 방식: ShowText 사용
        typewriter.ShowText(message);

        // 여기서는 단순히 "문장 길이에 비례한 시간 + 최소 시간" 정도로 대기
        float estimatedTime = Mathf.Max(minDisplayTime, message.Length * 0.04f);
        yield return new WaitForSeconds(estimatedTime + extraWait);
    }
}