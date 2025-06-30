using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nên dùng TextMeshPro

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText; // Đổi sang TextMeshPro

    [Header("Target")]
    [Tooltip("Tham chiếu đến script PlayerHealth cần theo dõi. Sẽ tự tìm nếu để trống.")]
    [SerializeField] private PlayerHealth playerHealth;

    // Start được gọi sau Awake, đảm bảo Player đã được tạo
    void Start()
    {
        // Nếu playerHealth chưa được gán, hãy thử tìm nó.
        // cách này an toàn hơn GetComponentInParent
        if (playerHealth == null)
        {
            // PlayerState.Instance là một cách tin cậy để tìm ra Player
            if (PlayerState.Instance != null)
            {
                playerHealth = PlayerState.Instance.GetComponent<PlayerHealth>();
            }
        }

        // Sau khi đã chắc chắn có tham chiếu, mới đăng ký sự kiện
        if (playerHealth != null)
        {
            // Đăng ký hàm UpdateHealthUI vào sự kiện OnHealthChanged
            playerHealth.OnHealthChanged += UpdateHealthUI;

            // Cập nhật UI lần đầu tiên với giá trị hiện tại
            UpdateHealthUI(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
        }
        else
        {
            Debug.LogError("PlayerHealthUI không thể tìm thấy PlayerHealth để theo dõi!", this);
        }
    }

    void OnDestroy()
    {
        // Luôn hủy đăng ký để tránh lỗi
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    /// <summary>
    /// Hàm này được gọi bởi sự kiện OnHealthChanged từ PlayerHealth.
    /// </summary>
    private void UpdateHealthUI(int currentHp, int maxHp)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = currentHp;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHp} / {maxHp}";
        }
    }
}