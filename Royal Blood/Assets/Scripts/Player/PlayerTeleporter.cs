using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTeleporter : MonoBehaviour
{
    public static PlayerTeleporter Instance { get; private set; }

    // Tên của EntryPoint mà Player sẽ xuất hiện ở scene tiếp theo
    // Được set bởi Portal trước khi load scene
    public string TargetEntryPointNameOnNextSceneLoad { get; set; }

    private bool hasBeenPositionedThisScene = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"PlayerTeleporter '{gameObject.name}' is now DontDestroyOnLoad. Primary instance.");
        }
        else if (Instance != this)
        {
            Debug.LogWarning($"Another PlayerTeleporter instance ('{Instance.gameObject.name}') already exists. Destroying this new Player GameObject ('{gameObject.name}').");
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"PlayerTeleporter '{gameObject.name}' enabled, subscribed to sceneLoaded.");
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log($"PlayerTeleporter '{gameObject.name}' disabled, unsubscribed from sceneLoaded.");
    }

    void Start()
    {
        // Có thể không cần làm gì nhiều ở Start nếu logic chính nằm trong OnSceneLoaded
        // Reset cờ vị trí cho lần đầu tiên (hoặc khi được enable lại trong cùng scene)
        hasBeenPositionedThisScene = false;
        // Thử định vị ngay, phòng trường hợp scene đầu tiên có entry point được set từ trước
        TryPositionPlayerAtEntryPoint();
    }


    // Được gọi mỗi khi một scene mới hoàn tất việc load
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"PlayerTeleporter: Scene '{scene.name}' loaded. Target Entry: '{TargetEntryPointNameOnNextSceneLoad}'");
        // Reset cờ cho scene mới này để Player có thể được định vị lại
        hasBeenPositionedThisScene = false;
        TryPositionPlayerAtEntryPoint();

        // Sau khi đã sử dụng (hoặc cố gắng sử dụng) TargetEntryPointNameOnNextSceneLoad,
        // bạn có thể muốn xóa nó đi để nó không ảnh hưởng đến lần load scene tiếp theo
        // nếu không phải do Portal kích hoạt (ví dụ: reload scene thủ công).
        // TargetEntryPointNameOnNextSceneLoad = null; // Cân nhắc dòng này
    }

    void TryPositionPlayerAtEntryPoint()
    {
        if (hasBeenPositionedThisScene)
        {
            Debug.Log("PlayerTeleporter: Already positioned in this scene.");
            return;
        }

        if (string.IsNullOrEmpty(TargetEntryPointNameOnNextSceneLoad))
        {
            Debug.Log("PlayerTeleporter: No TargetEntryPointName specified for this scene load. Player remains at current/last position.");
            hasBeenPositionedThisScene = true; // Đánh dấu đã "xử lý" để không cố gắng định vị lại
            return;
        }

        EntryPoint[] entryPoints = FindObjectsOfType<EntryPoint>();
        bool foundEntryPoint = false;
        foreach (EntryPoint entry in entryPoints)
        {
            if (entry.entryName == TargetEntryPointNameOnNextSceneLoad)
            {
                Debug.Log($"PlayerTeleporter: Positioning Player at EntryPoint '{entry.entryName}' at {entry.transform.position}");
                transform.position = entry.transform.position; // Di chuyển Player đến vị trí của EntryPoint

                // Nếu Player có Rigidbody, bạn có thể muốn reset vận tốc
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                hasBeenPositionedThisScene = true; // Đánh dấu Player đã được định vị trong scene này
                foundEntryPoint = true;
                break;
            }
        }

        if (!foundEntryPoint)
        {
            Debug.LogWarning($"PlayerTeleporter: EntryPoint '{TargetEntryPointNameOnNextSceneLoad}' NOT FOUND in scene '{SceneManager.GetActiveScene().name}'. Player remains at current/last position.");
            hasBeenPositionedThisScene = true; // Vẫn đánh dấu đã xử lý
        }

        // Cân nhắc xóa TargetEntryPointNameOnNextSceneLoad sau khi đã dùng:
        // Điều này ngăn việc Player bị đặt lại vị trí nếu scene được load lại
        // mà không thông qua portal.
        // TargetEntryPointNameOnNextSceneLoad = null;
    }
}