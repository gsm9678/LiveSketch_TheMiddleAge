using System.Collections;
using UnityEngine;
using DG.Tweening;
using Febucci.TextAnimatorForUnity;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    [Header("References")]
    public RectTransform bubbleRoot;
    public RectTransform bodyRect;
    public RectTransform tailRect;
    public RectTransform textContainer;
    public TextMeshProUGUI textMesh;
    public CanvasGroup canvasGroup;
    public TypewriterComponent typewriter;

    [Header("Target")]
    private Transform target;
    public Vector3 offset = new Vector3(0, 2.2f, 0);

    [Header("Padding")]
    public Vector2 padding = new Vector2(40, 30); // 말풍선 안쪽 여백

    [Header("Tween")]
    public float showTime = 0.25f;
    public float hideTime = 0.2f;

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;

            // Billboard
            if (Camera.main != null)
            {
                transform.forward = -(Camera.main.transform.position - transform.position).normalized;
            }
        }

        // 자동 크기 조절
        UpdateBubbleSize();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // 텍스트 길이에 맞게 말풍선 크기 조절
    void UpdateBubbleSize()
    {
        if (textContainer == null || bubbleRoot == null) return;

        // TextMeshPro가 계산한 Preferred Size 가져오기
        Vector2 preferred = new Vector2(textMesh.preferredWidth, textMesh.preferredHeight);

        // TextContainer는 ContentSizeFitter에 의해 자동 조절됨 → bubbleRoot가 패딩만 더해 확장
        bubbleRoot.sizeDelta = preferred + padding;
    }

    // 말풍선 등장
    public IEnumerator ShowBubble()
    {
        canvasGroup.alpha = 0;
        bubbleRoot.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Join(canvasGroup.DOFade(1f, showTime));
        seq.Join(bubbleRoot.DOScale(0.01f, showTime).SetEase(Ease.OutBack));

        yield return seq.WaitForCompletion();
    }

    // 말풍선 숨김
    public IEnumerator HideBubble()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(canvasGroup.DOFade(0f, hideTime));
        seq.Join(bubbleRoot.DOScale(0.005f, hideTime).SetEase(Ease.InBack));

        yield return seq.WaitForCompletion();
    }

    // 텍스트 출력
    public IEnumerator PlayLine(string message)
    {
        textMesh.text = message;
        typewriter.ShowText(message);

        float waitTime = Mathf.Max(0.5f, message.Length * 0.04f);
        yield return new WaitForSeconds(waitTime + 1f);
    }
}