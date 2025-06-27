using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý trạng thái tổng thể của người chơi, bao gồm hồi sinh, checkpoint và điều phối lưu/tải game.
/// Đây là trung tâm điều phối cấp cao nhất cho các hành động liên quan đến trạng thái sống/chết.
/// </summary>
public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    [Header("Respawn Settings")]
    [Tooltip("Thời gian chờ trước khi hồi sinh sau khi chết.")]
    public float respawnDelay = 2f;
    public Vector3 CurrentCheckpointPosition { get; private set; }

    // Tham chiếu đến các component quan trọng để điều phối
    private PlayerHealth playerHealth;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
   private ItemPicker itemPicker;

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); return; }
        playerHealth = GetComponent<PlayerHealth>();
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        itemPicker = GetComponent<ItemPicker>();
    }

    /// <summary>
    /// Được gọi bởi PlayerHealth khi máu về 0. Bắt đầu chu trình chết.
    /// </summary>
    public void OnPlayerDied()
    {
        // 1. Vô hiệu hóa vật lý và điều khiển để tránh các hành vi không mong muốn
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // Ngăn nhân vật bị đẩy đi khi đang chết
        }
        playerCollider.enabled = false;
        playerMovement.enabled = false;

        // 2. Kích hoạt animation chết
        animator.SetBool("isDeath", true);

        // 3. Bắt đầu đếm ngược để hồi sinh
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // Chờ một khoảng thời gian để animation chết được thực hiện
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    /// <summary>
    /// Thực hiện quá trình hồi sinh người chơi một cách toàn diện.
    /// </summary>
    private void Respawn()
    {
        animator.SetBool("isDeath", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("isRunning", false);
        // 1. Di chuyển người chơi về vị trí checkpoint cuối cùng
        transform.position = CurrentCheckpointPosition;

        // 2. Khôi phục lại vật lý và điều khiển
        playerCollider.enabled = true;
        playerMovement.enabled = true;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // TRẢ LẠI TRẠNG THÁI DYNAMIC
        }
        // 3. Gọi các hàm ResetState của từng component con để chúng tự reset trạng thái nội bộ
        playerHealth.ResetState();
    }

    /// <summary>
    /// Cập nhật vị trí checkpoint mới.
    /// </summary>
    public void SetNewCheckpoint(Vector3 newPosition) => CurrentCheckpointPosition = newPosition;

    /// <summary>
    /// Kích hoạt việc lưu dữ liệu game hiện tại.
    /// </summary>
    public void TriggerSaveGame()
    {
        var gameData = SaveSystem.LoadGame();
        SaveSystem.SaveGame(new GameData(
            SceneManager.GetActiveScene().name,
            transform.position,
            playerHealth.GetCurrentHealth(),
            playerHealth.GetMaxHealth(),
            playerAttack.GetAttack(),
            itemPicker.GetTotalDiamonds(),
            gameData.lastSceneName,
            gameData.lastSceneDiamonds,
            gameData.lastSceneKey
        ));
    }

    /// <summary>
    /// Áp dụng dữ liệu đã được tải từ file save.
    /// </summary>
    public void ApplyLoadedData(GameData data)
    {
        playerHealth.SetHealthData(data.playerCurrentHealth, data.playerMaxHealth);
        playerAttack.SetAttackData(data.playerAttackPower);
    }
}