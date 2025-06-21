using System;
using UnityEngine;

public class Range_Enemy : MonoBehaviour
{
    // SECTION: References & Components
    // Public references are assigned in the Unity Inspector.
    // Private references are fetched by the script itself.
    [Header("AI References")]
    public Transform player;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    private Animator animator;
    private Rigidbody2D rb;

    // SECTION: AI Behavior Settings
    // These values control how the AI perceives its environment and makes decisions.
    [Header("AI Behavior Ranges")]
    [Tooltip("The maximum distance at which the AI can see the player.")]
    public float detectionRange = 15f;
    [Tooltip("The maximum distance for shooting. The archer will advance if the player is further than this.")]
    public float shootingRange = 10f;
    [Tooltip("The minimum safe distance. The archer will retreat if the player is closer than this.")]
    public float tooCloseRange = 3f;

    // SECTION: Movement & Navigation
    // Controls the physical movement and navigation abilities of the AI.
    [Header("Movement & Navigation")]
    public float moveSpeed = 1.5f;
    public float jumpForce = 8f;
    public Transform groundCheckPoint;
    public Transform obstacleCheckPoint;
    public float groundCheckRadius = 0.2f;
    public float obstacleCheckDistance = 0.5f;
    public LayerMask groundLayer; // Defines what is considered 'ground' for jumping and landing.

    // SECTION: Attacking
    // Parameters for the archer's combat abilities.
    [Header("Attacking")]
    public float attackRate = 1f; // How many times the archer can attack per second.
    private float nextAttackTime = 0f; // A timer to manage the attack cooldown.

    // SECTION: Health
    // Manages the archer's health and life state.
    [Header("Health")]
    public int maxHealth = 60;
    private int currentHealth;

    // SECTION: State Variables
    // Private variables to track the AI's current state.
    private bool isFacingRight = true;
    private bool isDead = false;
    private bool isGrounded;

    // --- UNITY LIFECYCLE METHODS ---

    void Awake()
    {
        // Fetch components once at the start to improve performance.
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        player = AutoTrackPlayer.TrackPlayer().transform;
    }

    void Start()
    {
        // If the player is not assigned manually, try to find them by tag.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Update()
    {
        // If the AI is dead or has no target, do nothing.
        if (isDead || player == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement.
            return;
        }

        // The core AI decision-making loop.
        DecideAction();
    }

    void FixedUpdate()
    {
        // Physics-related checks should be in FixedUpdate.
        if (!isDead)
        {
            CheckGroundStatus();
        }
    }

    // --- AI DECISION & ACTION METHODS ---

    private void DecideAction()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Always face the player, regardless of the action.
        FlipTowardsPlayer();

        if (distanceToPlayer <= shootingRange && distanceToPlayer > tooCloseRange)
        {
            // State 1: In optimal shooting range. Stop and attack.
            StopAndAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // State 2: Player is detected but not in the optimal range. Reposition.
            Reposition(distanceToPlayer);
        }
        else
        {
            // State 3: Player is out of sight. Go idle.
            Idle();
        }
    }

    private void StopAndAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement.
        animator.SetBool("isWalking", false);

        // Check if enough time has passed to attack again.
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    private void Reposition(float distanceToPlayer)
    {
        if (distanceToPlayer > shootingRange)
        {
            // Player is too far, move forward.
            Move(1);
        }
        else if (distanceToPlayer <= tooCloseRange)
        {
            // Player is too close, retreat.
            Move(-1);
        }
    }

    private void Idle()
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // --- MOVEMENT & NAVIGATION METHODS ---

    // `direction` is 1 for forward, -1 for backward.
    private void Move(int direction)
    {
        // Check for obstacles and jump only when moving forward.
        if (direction > 0 && IsObstacleAhead() && isGrounded)
        {
            Jump();
        }

        animator.SetBool("isWalking", true);
        Vector2 targetDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(targetDirection.x * moveSpeed * direction, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (!isGrounded) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("Jump");
    }

    // --- HELPER & UTILITY METHODS ---

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private bool IsObstacleAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(obstacleCheckPoint.position, transform.right, obstacleCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void FlipTowardsPlayer()
    {
        float playerDirection = player.position.x - transform.position.x;
        if (playerDirection > 0 && !isFacingRight) Flip();
        else if (playerDirection < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // --- COMBAT METHODS (PUBLIC & ANIMATION EVENTS) ---

    // This is called by an Animation Event during the "Attack" animation.
    public void ShootArrow()
    {
        if (player == null || arrowPrefab == null || arrowSpawnPoint == null) return;

        GameObject arrowGO = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        Arrow arrowScript = arrowGO.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            Vector2 direction = (player.position - arrowSpawnPoint.position).normalized;
            arrowScript.Launch(direction);
        }
    }

    // This function must be public so the player's attack script can call it.
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
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

    // --- EDITOR-ONLY METHODS ---

    // This draws visual aids in the Scene view to make setup easier.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; // Detection range
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;  // Shooting range
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.color = Color.red;   // Too close range
        Gizmos.DrawWireSphere(transform.position, tooCloseRange);

        // Ground and Obstacle check gizmos
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
        if (obstacleCheckPoint != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 rayDirection = transform.right * obstacleCheckDistance;
            Gizmos.DrawLine(obstacleCheckPoint.position, obstacleCheckPoint.position + rayDirection);
        }
    }
}
