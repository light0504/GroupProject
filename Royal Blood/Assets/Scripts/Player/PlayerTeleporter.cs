using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerState))]
public class PlayerTeleporter : MonoBehaviour
{
    public static PlayerTeleporter Instance { get; private set; }
    public static GameData dataToLoad = null;
    public string TargetEntryPointNameOnNextSceneLoad { get; set; }

    public static string StartingCheckpointName = null;

    private bool hasBeenPositionedThisScene = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Bắt buộc cho mô hình này
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasBeenPositionedThisScene = false;
        PositionPlayerOnSceneLoad();
    }

    private void PositionPlayerOnSceneLoad()
    {
        if (hasBeenPositionedThisScene) return;

        if (dataToLoad != null)
        {
            Debug.Log(1);
            ApplyLoadedData();
        }
        else if (!string.IsNullOrEmpty(StartingCheckpointName))
        {
            Debug.Log(2);
            Debug.Log($"Starting checkpoint {StartingCheckpointName}");
            MoveToStartingCheckpoint();
        }
        else if (!string.IsNullOrEmpty(TargetEntryPointNameOnNextSceneLoad))
        {
            Debug.Log(3);
            TryPositionPlayerAtEntryPoint();
        }

        if (TryGetComponent<Rigidbody2D>(out var rb)) { rb.linearVelocity = Vector2.zero; }

        dataToLoad = null;
        TargetEntryPointNameOnNextSceneLoad = null;
        StartingCheckpointName = null;
    }

    private void ApplyLoadedData()
    {
        transform.position = new Vector2(dataToLoad.playerPosition[0], dataToLoad.playerPosition[1]);
        GetComponent<PlayerState>().ApplyLoadedData(dataToLoad);
    }

    private void MoveToStartingCheckpoint()
    {
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (var checkpoint in allCheckpoints)
        {
            if (checkpoint.checkpointName == StartingCheckpointName)
            {
                transform.position = checkpoint.transform.position;
                GetComponent<PlayerState>().SetNewCheckpoint(transform.position);
                return;
            }
        }
    }
    void TryPositionPlayerAtEntryPoint()
    {
        if (hasBeenPositionedThisScene) return;

        hasBeenPositionedThisScene = false;

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

    public static void KillInstance()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }
}