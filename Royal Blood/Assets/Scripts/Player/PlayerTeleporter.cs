using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý việc giữ Player tồn tại giữa các scene và định vị Player
/// tại một EntryPoint cụ thể dựa trên tên được cung cấp bởi Portal.
/// </summary>
public class PlayerTeleporter : MonoBehaviour
{
    public static PlayerTeleporter Instance { get; private set; }

    /// <summary>
    /// Tên của EntryPoint ở scene đích.
    /// Được thiết lập bởi Portal trước khi tải scene.
    /// </summary>
    public string TargetEntryPointNameOnNextSceneLoad { get; set; }

    private bool hasBeenPositionedThisScene = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Được gọi tự động mỗi khi một scene mới hoàn tất việc tải.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasBeenPositionedThisScene = false;

        TryPositionPlayerAtEntryPoint();
    }

    /// <summary>
    /// Tìm kiếm và di chuyển Player đến EntryPoint được chỉ định.
    /// </summary>
    void TryPositionPlayerAtEntryPoint()
    {
        if (hasBeenPositionedThisScene) return;

        hasBeenPositionedThisScene = true;

        Debug.Log($"PlayerTeleporter: Đang tìm kiếm EntryPoint có tên '{TargetEntryPointNameOnNextSceneLoad}'...");

        EntryPoint[] allEntryPoints = FindObjectsOfType<EntryPoint>();
        bool foundTarget = false;

        foreach (var entryPoint in allEntryPoints)
        {
            if (entryPoint.entryName == TargetEntryPointNameOnNextSceneLoad)
            {
                this.transform.position = entryPoint.transform.position;

                if (TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.linearVelocity = Vector2.zero;
                }

                Debug.Log($"Thành công! Player đã được dịch chuyển đến EntryPoint '{entryPoint.entryName}'.");
                foundTarget = true;
                break;
            }
        }

        if (!foundTarget)
        {
            Debug.LogWarning($"LỖI: Không tìm thấy EntryPoint nào có tên '{TargetEntryPointNameOnNextSceneLoad}' trong scene '{SceneManager.GetActiveScene().name}'. Player sẽ giữ nguyên vị trí.");
        }

        TargetEntryPointNameOnNextSceneLoad = null;
    }

    /// <summary>
    /// Hủy instance Player đang tồn tại.
    /// Dùng khi quay về Main Menu hoặc reset game.
    /// </summary>
    public static void KillInstance()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }
}