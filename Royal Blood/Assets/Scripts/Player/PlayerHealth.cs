using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý máu và các tương tác liên quan đến sát thương của người chơi.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats")]
    [Tooltip("Máu tối đa của người chơi.")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Dependencies")]
    private PlayerMovement playerMovement;

    [Tooltip("Kéo script 'Noti' vào đây.")]
    [SerializeField] private Noti notificationManager;
    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>(); // Đổi tên từ PlayerMovement
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // Thông báo cho UI để cập nhật thanh máu lần đầu
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        
    }
    /// <summary>
    /// Xử lý việc người chơi nhận sát thương.
    /// </summary>
    /// <param name="damage">Lượng sát thương nhận vào.</param>
    public void TakeDamage(int damage)
    {
        // Không nhận sát thương nếu đã chết hoặc đang trong trạng thái bất tử (lướt, v.v.)
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void CheckLose()
    {
        if (maxHealth <= 0)
        {
            isDead = true;
            StartCoroutine(Delay(1f));
            StartCoroutine(ReloadSceneAfterDelay(5f));
        }
    }
    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationManager.PrintText("Bạn đã thất bại trên con đường tiêu diệt quỷ vương");
    }
    IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Main menu");
    }
    /// <summary>
    /// Bắt đầu quá trình chết bằng cách thông báo cho PlayerState.
    /// </summary>
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        // Giao toàn bộ trách nhiệm xử lý chết cho PlayerState
        PlayerState.Instance.OnPlayerDied();
    }

    /// <summary>
    /// Được gọi bởi PlayerState để reset lại trạng thái máu khi hồi sinh.
    /// </summary>
    public void ResetState()
    {
        isDead = false;
        playerMovement.ClearInput();
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // --- GETTERS & SETTERS ---

    public bool IsDead() => isDead;

    public void IncreaseHealth(int amount)
    {
        if (amount <= 0) return;

        // Tăng sát thương
        maxHealth += amount;
        currentHealth += amount;

        if (notificationManager != null)
        {
            notificationManager.PrintText($"Tăng +{amount} Máu!");
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void HealPercent(float percent)
    {
        if (percent <= 0) return;
        currentHealth += (int)(maxHealth * percent);
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth() => currentHealth;

    public int GetMaxHealth() => maxHealth; 

    /// <summary>
    /// Thiết lập máu của người chơi từ dữ liệu đã lưu.
    /// </summary>
    public void SetHealthData(int currentTempHealth, int maxTempHealth)
    {
        maxHealth = maxTempHealth;
        currentHealth = currentTempHealth > 0 ? currentTempHealth : maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}