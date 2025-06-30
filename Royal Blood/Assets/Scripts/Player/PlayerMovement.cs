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

    [Header("Jump Settings")]
    public int maxJumpCount = 2;
    private int jumpCount;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool isGrounded = false;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashCooldownTimer;

    private bool isAttacking = false;

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

        // Reset jump count khi chạm đất
        if (isGrounded)
        {
            jumpCount = 0;
        }

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
        // Double Jump logic
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            animator.SetTrigger("Jump");
        }

        if (Input.GetButtonDown("Dash") && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        animator.SetTrigger("Dash");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    private void FlipCharacter()
    {
        if ((isFacingRight && moveInput < 0) || (!isFacingRight && moveInput > 0))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
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

    public bool GetDash() => isDashing;
}
