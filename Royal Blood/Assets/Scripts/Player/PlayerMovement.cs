using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Player Stats")]
    public int maxHealth = 100;
    private int currentHealth;

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
        currentHealth = maxHealth;
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

    public void TakeDamage(int damage)
    {
        // Check if player is already dead or maybe invincible (e.g., during a dash).
        if (currentHealth <= 0 || isDashing)
        {
            return;
        }

        currentHealth -= damage;
        animator.SetTrigger("TakeHit"); // Play the player's "get hit" animation.
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
        Debug.Log("Player has died!");
        animator.SetBool("isDeath", true);

        // Disable player movement script and physics.
        this.enabled = false; // This disables the Update() loop of this script.
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;

        // Optional: Add logic for a "Game Over" screen here after a delay.
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
        // 1. Detect all colliders within the attack range that are on the "Enemy" layer.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 2. Loop through every enemy that was hit.
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // 3. Try to get the AI script from the enemy object.
            // You might need to check for multiple types of enemies here in the future.
            Range_Enemy enemyAI = enemyCollider.GetComponent<Range_Enemy>();
            if (enemyAI != null)
            {
                // 4. Call the enemy's public TakeDamage function.
                enemyAI.TakeDamage(attackDamage);
                continue; // Move to the next enemy in the list.
            }

            Enemy skeletonAI = enemyCollider.GetComponent<Enemy>();
            if (skeletonAI != null)
            {
                skeletonAI.TakeDamage(attackDamage);
            }
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
