using UnityEngine;

public class TouchCharacterParticle : MonoBehaviour
{
    [Header("Ray")]
    [SerializeField] private Camera rayCamera;
    [SerializeField] private float maxDistance = 200f;
    [SerializeField] private LayerMask characterLayer = ~0; // 필요하면 Character만 있는 레이어로 제한

    private void Awake()
    {
        if (rayCamera == null)
            rayCamera = Camera.main;
    }

    private void Update()
    {
        // 모바일 터치
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryHitCharacter(Input.GetTouch(0).position);
        }

        // PC 클릭 (에디터 테스트용)
        if (Input.GetMouseButtonDown(0))
        {
            TryHitCharacter(Input.mousePosition);
        }
    }

    private void TryHitCharacter(Vector2 screenPos)
    {
        if (rayCamera == null) return;

        Ray ray = rayCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, characterLayer))
        {
            // Character가 Collider가 자식에 있을 수도 있으니 부모까지 탐색
            Character ch = hit.collider.GetComponentInParent<Character>();
            if (ch != null)
            {
                ch.PlayArriveParticlePublic();
            }
        }
    }
}