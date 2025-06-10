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

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (animator.GetBool("isDeath"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);

        UpdateAnimatorParameters();

        HandlePlayerInput();

        FlipCharacter();
    }

    void FixedUpdate()
    {
        if (!animator.GetBool("isDeath"))
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

        if (Input.GetButtonDown("Dash"))
        {
            animator.SetTrigger("Dash");
        }
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
