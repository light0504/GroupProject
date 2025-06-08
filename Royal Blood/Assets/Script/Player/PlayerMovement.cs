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

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
        // Chỉ cập nhật vận tốc khi KHÔNG ĐANG DASH và không chết
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
    }

    private System.Collections.IEnumerator Dash()
    {
        // 1. Thiết lập các trạng thái khi bắt đầu Dash
        isDashing = true;                                   // Đánh dấu đang dash
        dashCooldownTimer = dashCooldown;                   // Bắt đầu tính thời gian hồi chiêu
        animator.SetTrigger("Dash");                        // Kích hoạt animation Dash

        float originalGravity = rb.gravityScale;            // Lưu lại trọng lực ban đầu
        rb.gravityScale = 0f;                               // Tắt trọng lực để lướt theo đường thẳng

        // Xác định hướng lướt dựa trên hướng nhân vật đang nhìn
        float dashDirection = isFacingRight ? 1f : -1f;

        // 2. Áp dụng lực Dash
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        // (Tùy chọn) Bật chế độ bất tử ở đây nếu muốn
        // ví dụ: gameObject.layer = LayerMask.NameToLayer("InvinciblePlayer");

        // 3. Chờ cho đến khi thời gian lướt kết thúc
        yield return new WaitForSeconds(dashDuration);

        // 4. Khôi phục lại các trạng thái sau khi Dash xong
        rb.gravityScale = originalGravity;                  // Bật lại trọng lực
        isDashing = false;                                  // Đánh dấu đã hết dash

        // Nếu bạn muốn nhân vật dừng lại đột ngột sau khi dash, thêm dòng này
        // rb.velocity = Vector2.zero;

        // (Tùy chọn) Tắt chế độ bất tử ở đây
        // ví dụ: gameObject.layer = LayerMask.NameToLayer("Player");
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

    void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
