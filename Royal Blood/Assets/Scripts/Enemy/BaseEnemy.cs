using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lớp abstract không thể được kéo thả trực tiếp vào GameObject.
// Nó đóng vai trò là một khuôn mẫu cho các lớp con.
public abstract class BaseEnemy : MonoBehaviour
{
    // SECTION: References & Components (chung)
    [Header("Base References")]
    public Transform player;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected EnemyHealthUI healthUI;


    public float respawnDelay = 10f;

    // SECTION: Movement & Navigation (chung)
    [Header("Base Movement & Navigation")]
    public float moveSpeed = 2f;
    public float jumpForce = 8f;
    public Transform groundCheckPoint;
    public Transform obstacleCheckPoint;
    public float groundCheckRadius = 0.2f;
    public float obstacleCheckDistance = 0.5f;
    public LayerMask groundLayer;

    // SECTION: Attacking (chung)
    [Header("Base Attacking")]
    public float attackRate = 1f;
    protected float nextAttackTime = 0f;

    // SECTION: Health (chung)
    [Header("Base Health")]
    public int maxHealth = 100;
    protected int currentHealth;

    [Header("Item Drop Settings")]
    [Tooltip("Danh sách các vật phẩm có thể rơi ra khi kẻ thù này bị tiêu diệt.")]
    public List<BaseItem> droppableItems = new List<BaseItem>();

    public Vector3 CurrentCheckpointPosition { get; private set; }

    // SECTION: State Variables (chung)
    protected bool isFacingRight = true;
    protected bool isDead = false;
    protected bool isGrounded;

    public event Action<int, int> OnHealthChanged;

    // --- UNITY LIFECYCLE METHODS ---

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthUI = GetComponentInChildren<EnemyHealthUI>();

        currentHealth = maxHealth;

        if (player == null)
        {
            var playerObject = AutoTrackPlayer.TrackPlayer();
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
        CurrentCheckpointPosition = transform.position;
    }

    protected virtual void Start()
    {
        // Nếu vẫn không tìm thấy người chơi, tìm bằng tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Update()
    {
        if (isDead || player == null)
        {
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (healthUI != null)
        {
            healthUI.gameObject.SetActive(!(maxHealth == currentHealth));
        }

        // Mỗi lớp con sẽ tự quyết định hành động của mình
        DecideAction();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        CheckGroundStatus();
    }

    // --- AI DECISION & ACTION (ABSTRACT) ---

    // Phương thức trừu tượng này BẮT BUỘC các lớp con phải triển khai.
    // Đây là nơi chứa logic AI chính.

    protected abstract void DecideAction();

    // --- MOVEMENT & NAVIGATION (chung) ---

    protected void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    protected bool IsObstacleAhead()
    {
        // Sử dụng transform.right vì nó sẽ tự động lật theo hướng của nhân vật
        RaycastHit2D hit = Physics2D.Raycast(obstacleCheckPoint.position, transform.right, obstacleCheckDistance, groundLayer);
        return hit.collider != null;
    }

    #region Action
    protected void Jump()
    {
        if (!isGrounded) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("Jump");
    }

    protected void FlipTowardsPlayer()
    {
        if (player == null) return;
        float playerDirection = player.position.x - transform.position.x;
        if (playerDirection > 0 && !isFacingRight) Flip();
        else if (playerDirection < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    #endregion
    #region HEALTH

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        animator.SetTrigger("Hit");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    #endregion
    #region DieAndRespawn
    protected void Die()
    {
        Debug.Log(gameObject.name + " died!");
        isDead = true;
        animator.SetTrigger("Dead"); // Kích hoạt animation chết

        // Vô hiệu hóa vật lý và va chạm
        GetComponent<Collider2D>().enabled = false;
        healthUI.gameObject.SetActive(false);
        if (rb != null)
        {
            rb.simulated = false;
        }
        HandleItemDrop();
        StartCoroutine(RespawnRoutine());
    }

    // Được gọi bởi Animation Event ở cuối clip animation "Dead"
    public void OnDeathAnimationFinished()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator RespawnRoutine()
    {
        // Chờ một khoảng thời gian để animation chết được thực hiện
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }
    private void Respawn()
    {
        Debug.Log(gameObject.name + " is respawning!");

        // 1. Di chuyển kẻ thù về vị trí ban đầu
        transform.position = CurrentCheckpointPosition;
        // Có thể thêm: transform.rotation = initialRotation;

        // 2. Reset lại trạng thái logic
        isDead = false;
        currentHealth = maxHealth;
        // Có thể thêm: OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // 3. Kích hoạt lại vật lý và va chạm
        GetComponent<Collider2D>().enabled = true;
        if (rb != null)
        {
            rb.simulated = true;
        }

        // 4. Kích hoạt lại hình ảnh
        GetComponent<SpriteRenderer>().enabled = true;

        // 5. Kích hoạt lại Animator để nó quay về trạng thái Idle
        // (Nếu bạn có transition từ "Dead" về "Any State" hoặc "Idle")
        animator.Rebind(); // Rebind() là một cách mạnh mẽ để reset hoàn toàn animator
        animator.Update(0f);
    }

    #endregion
    #region Drop
    private void HandleItemDrop()
    {
        if (droppableItems == null || droppableItems.Count == 0) return;

        foreach (var dropItem in droppableItems)
        {

            if (UnityEngine.Random.Range(0f, 1f) <= dropItem.dropChance)
            {
                GameObject itemInstance = Instantiate(dropItem.itemPrefab, transform.position, Quaternion.identity);

                if (itemInstance.TryGetComponent<BaseItem>(out var itemScript))
                {
                    itemScript.Initialize(itemInstance.transform.position);
                }
            }
        }
    }

    #endregion
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (groundCheckPoint != null) Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);

        Gizmos.color = Color.magenta;
        if (obstacleCheckPoint != null) Gizmos.DrawLine(obstacleCheckPoint.position, obstacleCheckPoint.position + transform.right * obstacleCheckDistance);
    }

    public int GetMaxHealth() => maxHealth;
    public int GetHealth() => currentHealth;
}