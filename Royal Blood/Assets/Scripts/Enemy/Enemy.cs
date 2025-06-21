using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("AI References")]
    public Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("AI Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;

    [Header("Jumping & Navigation")]
    public float jumpForce = 8f; // The force applied for a jump.
    public Transform groundCheckPoint; // A point below the enemy to check for ground.
    public Transform obstacleCheckPoint; // A point in front of the enemy to check for obstacles.
    public float groundCheckRadius = 0.2f; // Radius of the ground check circle.
    public float obstacleCheckDistance = 0.5f; // How far the obstacle check raycast looks.
    public LayerMask groundLayer; // Defines what layer is considered ground/obstacles.

    [Header("Attack Settings")]
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Melee Attack")]
    public Transform attackPoint;
    public int attackDamage = 20;
    public LayerMask playerLayer; // Set this to the "Player" layer in the Inspector.

    private bool isFacingRight = true;
    private bool isDead = false;
    private bool isGrounded; // To store the ground status.

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Corrected from previous error.
        currentHealth = maxHealth;
        player = AutoTrackPlayer.TrackPlayer().transform;
    }

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null)
        {
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // The AI logic remains the same.
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackBehavior();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChaseBehavior();
        }
        else
        {
            IdleBehavior();
        }

        FlipTowardsPlayer();
    }

    // FixedUpdate is used for physics checks.
    void FixedUpdate()
    {
        if (isDead) return;
        CheckGroundStatus();
    }

    private void CheckGroundStatus()
    {
        // Check if the groundCheckPoint is colliding with anything on the groundLayer.
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private bool IsObstacleAhead()
    {
        // Shoots a raycast forward from the obstacleCheckPoint.
        // `transform.right` points in the local 'right' direction, which is forward for a 2D character.
        RaycastHit2D hit = Physics2D.Raycast(obstacleCheckPoint.position, transform.right, obstacleCheckDistance, groundLayer);

        // Return true if the raycast hits something.
        return hit.collider != null;
    }

    private void Jump()
    {
        if (!isGrounded) return; // Can only jump if grounded.

        // Apply upward force and trigger animation.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("Jump");
    }

    private void IdleBehavior()
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement but allow falling.
    }

    private void ChaseBehavior()
    {
        // Check for obstacles before moving.
        if (IsObstacleAhead() && isGrounded)
        {
            Jump();
        }

        // Standard chase logic.
        animator.SetBool("isWalking", true);
        Vector2 targetDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(targetDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    private void AttackBehavior()
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    // --- This is the function for the Animation Event ---
    public void DealMeleeDamage()
    {
        // Create a circle to detect the player.
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        // If the player was detected...
        if (hitPlayer != null)
        {
            // ...get their controller script and call TakeDamage.
            PlayerMovement player = hitPlayer.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.GetComponentInChildren<PlayerHealth>().TakeDamage(attackDamage);
            }
        }
    }

    // --- The rest of the functions (Flip, TakeDamage, Die) remain the same ---
    void FlipTowardsPlayer()
    {
        float playerDirection = player.position.x - transform.position.x;
        if (playerDirection > 0 && !isFacingRight) Flip();
        else if (playerDirection < 0 && isFacingRight) Flip();
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        animator.SetTrigger("Hit");
        if (currentHealth <= 0) Die();
    }
    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        isDead = true;

        // 1. Trigger the animation.
        animator.SetTrigger("Dead");

        // 2. Disable the object's ability to collide or be a physical object.
        // This prevents the dead body from blocking the player.
        GetComponent<Collider2D>().enabled = false;
        if (rb != null)
        {
            rb.simulated = false; // A better way to disable physics than setting to Static.
            rb.linearVelocity = Vector2.zero;
        }

        // 3. REMOVE the Destroy(gameObject, 2f); line from here.
    }

    public void OnDeathAnimationFinished()
    {
        // This function will be called by the animation event.
        // Now it's safe to destroy the object.
        Destroy(gameObject);
    }

    // Gizmos for easy setup in the editor.
    void OnDrawGizmosSelected()
    {
        // Draw the attack and detection ranges.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw the ground and obstacle check gizmos.
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
        if (obstacleCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Vector3 rayDirection = transform.right * obstacleCheckDistance;
            Gizmos.DrawLine(obstacleCheckPoint.position, obstacleCheckPoint.position + rayDirection);
        }
    }

}
