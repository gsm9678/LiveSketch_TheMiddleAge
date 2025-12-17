using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentDebugPanel : MonoBehaviour
{
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Vector2 screenOffset = new Vector2(10, 10);

    private NavMeshAgent agent;
    private PlayerController controller;
    private Camera cam;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<PlayerController>();
        cam = Camera.main;
    }

    private void OnGUI()
    {
        if (!showDebug || agent == null) return;

        Vector3 worldPos = transform.position + Vector3.up * 2.2f;
        Vector3 screenPos = cam != null ? cam.WorldToScreenPoint(worldPos) : Vector3.zero;
        if (screenPos.z < 0) return;

        float x = screenPos.x + screenOffset.x;
        float y = Screen.height - screenPos.y + screenOffset.y;

        GUI.color = GetStateColor();
        GUI.Box(new Rect(x, y, 360, 160), GetDebugText());
    }

    private string GetDebugText()
    {
        return
$@"[{gameObject.name}]
State        : {(controller != null ? controller.State.ToString() : "N/A")}
isOnNavMesh  : {agent.isOnNavMesh}
hasPath      : {agent.hasPath}
pathStatus   : {agent.pathStatus}
velocity     : {agent.velocity.magnitude:F3}
remaining    : {agent.remainingDistance:F2}
destination  : {agent.destination}";
    }

    private Color GetStateColor()
    {
        if (!agent.isOnNavMesh) return Color.red;
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid) return new Color(1f, 0.5f, 0.2f);
        if (agent.velocity.magnitude < 0.01f && agent.hasPath) return Color.yellow;
        if (agent.velocity.magnitude > 0.01f) return Color.green;
        return Color.white;
    }
}