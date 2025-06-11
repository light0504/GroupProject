using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private float moveInput;
    private bool isFacingRight = true;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Dashing")]
    public float dashSpeed = 20f;         
    public float dashDuration = 0.2f;     
    public float dashCooldown = 1f;       

    private bool isDashing;               
    private float dashCooldownTimer;

    [Header("Attacking")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public LayerMask enemyLayers;

    private bool isAttacking;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing || isAttacking)
        {
            return;
        }

        if (isDashing)
        {
            return;
        }

        if (animator.GetBool("isDeath"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);

        UpdateAnimatorParameters();

        HandlePlayerInput();

        FlipCharacter();
    }

    void FixedUpdate()
    {
        if (!isDashing && !animator.GetBool("isDeath"))
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("isRunning", moveInput != 0 && isGrounded);

        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);
    }

    private void HandlePlayerInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        if (Input.GetButtonDown("Dash") && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetButtonDown("Fire1")) // "Fire1" thường là nút chuột trái hoặc Left Ctrl
        {
            Attack();
        }
    }

    private System.Collections.IEnumerator Dash()
    {
        // 1. Thiết lập các trạng thái khi bắt đầu Dash
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        animator.SetTrigger("Dash");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = isFacingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        // (Tùy chọn) Bật chế độ bất tử ở đây nếu muốn
        // ví dụ: gameObject.layer = LayerMask.NameToLayer("InvinciblePlayer");

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        // Nếu bạn muốn nhân vật dừng lại đột ngột sau khi dash, thêm dòng này
        // rb.velocity = Vector2.zero;

        // (Tùy chọn) Tắt chế độ bất tử ở đây
        // ví dụ: gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void Attack()
    {
        // 1. Kích hoạt animation và trạng thái tấn công
        isAttacking = true;
        animator.SetTrigger("Attack");
        // Chúng ta sẽ gọi hàm gây sát thương từ Animation Event
    }

    public void TakeHit()
    {
        animator.SetTrigger("TakeHit");
    }

    public void Die()
    {
        animator.SetBool("isDeath", true);
    }

    private void FlipCharacter()
    {
        if ((isFacingRight && moveInput < 0) || (!isFacingRight && moveInput > 0))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }


    public void DealDamageToEnemies()
    {
        // Phát hiện tất cả kẻ thù trong một vòng tròn tại attackPoint
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Gây sát thương cho tất cả kẻ thù tìm thấy
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);

            /*
            // Giả sử kẻ thù có một script tên là "EnemyHealth" với hàm TakeDamage(int damage)
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
            */
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }

    void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
