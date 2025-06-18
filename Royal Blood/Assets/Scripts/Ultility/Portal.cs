using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Portal : MonoBehaviour
{
    public enum EntryPointDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    [Header("Cấu hình Chuyển Scene")]
    public SceneLoader sceneLoader;
    public int sceneIndexToLoad;

    [Header("Cấu hình Dịch Chuyển Player")]
    [Tooltip("Tên của EntryPoint trong scene đích mà người chơi sẽ đến. Phải khớp chính xác!")]
    public string targetEntryPointName;

    [Header("Hiển thị Thông tin Portal")]
    public string destinationDisplayName;
    public TextMeshProUGUI destinationText;

    [Header("Vị trí Xuất hiện")]
    [Tooltip("Kéo GameObject EntryPoint là con của Portal này vào đây.")]
    public Transform entryPoint;

    [Tooltip("Hướng người chơi sẽ xuất hiện so với tâm của portal.")]
    public EntryPointDirection entryDirection;

    private const float ENTRY_DISTANCE = 4.0f;

    private Collider2D teleportCollider;
    private Collider2D infoCollider;

    private void Awake()
    {
        teleportCollider = GetComponent<BoxCollider2D>();
        infoCollider = GetComponent<CircleCollider2D>();
        teleportCollider.isTrigger = true;
        infoCollider.isTrigger = true;

        if (destinationText != null)
        {
            destinationText.text = destinationDisplayName;
            destinationText.gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        if (entryPoint == null) return;

        switch (entryDirection)
        {
            case EntryPointDirection.Top:
                entryPoint.localPosition = new Vector3(0, ENTRY_DISTANCE, 0);
                break;
            case EntryPointDirection.Bottom:
                entryPoint.localPosition = new Vector3(0, -ENTRY_DISTANCE, 0);
                break;
            case EntryPointDirection.Left:
                entryPoint.localPosition = new Vector3(-ENTRY_DISTANCE, 0, 0);
                break;
            case EntryPointDirection.Right:
                entryPoint.localPosition = new Vector3(ENTRY_DISTANCE, 0, 0);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject) return;

        if (entryPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(entryPoint.position, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, entryPoint.position);
        }
    }

    // --- Xử lý va chạm ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.IsTouching(infoCollider))
        {
            if (destinationText != null) { destinationText.gameObject.SetActive(true); }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && destinationText != null)
        {
            destinationText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.IsTouching(teleportCollider))
        {
            HandleTeleport();
        }
    }

    private void HandleTeleport()
    {
        if (sceneLoader == null || PlayerTeleporter.Instance == null) return;

        PlayerTeleporter.Instance.TargetEntryPointNameOnNextSceneLoad = this.targetEntryPointName;

        sceneLoader.LoadConfiguredSceneFromArray(sceneIndexToLoad);
    }
}