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
    public bool isNextSceneGate;

    [Header("Cấu hình Dịch Chuyển Player")]
    [Tooltip("Tên của EntryPoint trong scene đích mà người chơi sẽ đến. Phải khớp chính xác!")]
    public string targetEntryPointName;

    [Header("Hiển thị Thông tin Portal")]
    public string destinationDisplayName;
    public TextMeshProUGUI destinationText;

    [Header("Vị trí Xuất hiện")]
    [Tooltip("Hướng người chơi sẽ xuất hiện nếu không thể vượt.")]
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

    private Vector3 GetTeleDirection()
    {
        Vector3 direction = gameObject.transform.position;
        switch (entryDirection)
        {
            case EntryPointDirection.Top:
                return new Vector3(direction.x, direction.y + 4, direction.z);//y + 4
            case EntryPointDirection.Bottom:
                return new Vector3(direction.x, direction.y - 4, direction.z);//y - 4
            case EntryPointDirection.Left:
                return new Vector3(direction.x - 4, direction.y, direction.z);//x - 4
            case EntryPointDirection.Right:
                return new Vector3(direction.x + 4, direction.y, direction.z);//x + 4
        }
        return direction;
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
            if (isNextSceneGate)
            {
                if (SceneDataManager.Instance.CanMoveNextScene())
                {
                    HandleTeleport();
                }
                else
                {
                    GameObject player = AutoTrackPlayer.TrackPlayer();
                    player.transform.position = GetTeleDirection();
                    player.GetComponent<Noti>().PrintText("Không thể dịch chuyển, hãy tiêu diệt quái vật!");
                }
            }
            else
            {
                HandleTeleport();
            }
        }
    }

    private void HandleTeleport()
    {
        if (sceneLoader == null || PlayerTeleporter.Instance == null) return;

        PlayerTeleporter.Instance.TargetEntryPointNameOnNextSceneLoad = this.targetEntryPointName;

        sceneLoader.LoadConfiguredSceneFromArray(sceneIndexToLoad);
    }
}