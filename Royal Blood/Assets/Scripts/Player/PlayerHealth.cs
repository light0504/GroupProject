using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    private bool isDead = false;

    [Header("Dependencies")]
    private PlayerState playerState;
    private PlayerMovement playerMovement;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D playerCollider;

    public event Action<int, int> OnHealthChanged;
    private void Awake()
    {
        // Lấy tất cả các component một lần để tối ưu
        playerState = GetComponent<PlayerState>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // Khởi tạo máu
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        // Check if player is already dead or maybe invincible (e.g., during a dash).
        if (isDead || playerMovement.GetDash())
        {
            return;
        }
        Debug.Log("Player takes damage: " + damage);
        currentHealth -= damage;
        animator.SetTrigger("TakeHit"); // Play the player's "get hit" animation.
        OnHealthChanged(currentHealth, maxHealth);
        Debug.Log("Player health: " + currentHealth);

        // Optional: Add a knockback effect here.
        // rb.AddForce(new Vector2(knockbackX, knockbackY), ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Tránh gọi hàm Die nhiều lần

        isDead = true;
        Debug.Log("Player has died!");
        animator.SetBool("isDeath", true);
        // Vô hiệu hóa điều khiển và vật lý TẠM THỜI
        //playerMovement.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true; // Ngăn các lực khác tác động
        playerCollider.enabled = false;

        // Bắt đầu quá trình hồi sinh sau một khoảng trễ
        StartCoroutine(RespawnRoutine(2f)); // Chờ 2 giây để animation chết chạy
    }
    private IEnumerator RespawnRoutine(float delay)
    {
        // Chờ animation chết hoặc một khoảng thời gian
        yield return new WaitForSeconds(delay);

        // 1. Dịch chuyển người chơi
        playerState.Respawn(); // playerState.Respawn() chỉ nên chứa logic di chuyển vị trí

        // 2. Hồi phục trạng thái
        currentHealth = maxHealth;
        isDead = false;
        animator.SetBool("isDeath", false);
        OnHealthChanged?.Invoke(currentHealth, maxHealth); // Cập nhật lại UI máu đầy

        // 3. Kích hoạt lại điều khiển và vật lý
        playerMovement.enabled = true;
        playerCollider.enabled = true;
        rb.isKinematic = false;

        Debug.Log("Player has respawned!");
    }
    public int[] GetHealth()
    {
        return new int[] { currentHealth, maxHealth };
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    public void SetHealthData(int[] loadedHealth)
    {
        maxHealth = loadedHealth[0];
        currentHealth = loadedHealth[1] > 0 ? loadedHealth[1] : maxHealth;
    }

    public void UpdateHealth(int heal)
    {
        currentHealth += heal;
    }

    public void SetMaxHealth()
    {
        currentHealth = maxHealth;
    }
}