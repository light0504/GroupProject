using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("AI References")]
    public Transform player; // K�o ??i t??ng Player v�o ?�y
    private Animator animator;
    private Rigidbody2D rb;

    [Header("AI Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 10f; // T?m ph�t hi?n ng??i ch?i
    public float attackRange = 1.5f;   // T?m ?? t?n c�ng
    public LayerMask playerLayer;      // Layer c?a ng??i ch?i

    [Header("Attack Settings")]
    public float attackRate = 2f; // T?n c�ng 1 l?n m?i 2 gi�y
    private float nextAttackTime = 0f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    private bool isFacingRight = true;
    private bool isDead = false;

    void Awake()
    {
        // L?y c�c component c?n thi?t
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // N?u b?n ch?a g�n Player, h�y th? t�m b?ng Tag
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // N?u ?� ch?t, kh�ng l�m g� c?
        if (isDead || player == null)
        {
            // D?ng m?i chuy?n ??ng khi ch?t
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // T�nh kho?ng c�ch t?i ng??i ch?i
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Logic AI
        if (distanceToPlayer <= attackRange)
        {
            // 1. ? trong t?m t?n c�ng -> T?n c�ng
            animator.SetBool("isWalking", false); // D?ng ?i b? ?? t?n c�ng
            rb.linearVelocity = Vector2.zero; // D?ng di chuy?n

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // 2. ? trong t?m ph�t hi?n -> ?u?i theo
            ChasePlayer();
        }
        else
        {
            // 3. ? ngo�i t?m -> ??ng y�n
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
        }

        // L?t m?t ?? lu�n nh�n v? ph�a ng??i ch?i
        FlipTowardsPlayer();
    }

    void ChasePlayer()
    {
        animator.SetBool("isWalking", true);
        Vector2 targetDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(targetDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    void Attack()
    {
        // H�m n�y ch? k�ch ho?t animation
        // Vi?c g�y s�t th??ng n�n ???c x? l� b?ng Animation Event
        animator.SetTrigger("Attack");
    }

    void FlipTowardsPlayer()
    {
        float playerDirection = player.position.x - transform.position.x;

        if (playerDirection > 0 && !isFacingRight)
        {
            // Ng??i ch?i ? b�n ph?i, nh?ng skeleton ?ang nh�n sang tr�i -> l?t
            Flip();
        }
        else if (playerDirection < 0 && isFacingRight)
        {
            // Ng??i ch?i ? b�n tr�i, nh?ng skeleton ?ang nh�n sang ph?i -> l?t
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // H�m n�y ph?i l� public ?? script c?a ng??i ch?i c� th? g?i
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
        Debug.Log("Skeleton died!");
        isDead = true;

        // K�ch ho?t animation ch?t
        animator.SetBool("isDead", true);

        // V� hi?u h�a v?t l� v� collider ?? kh�ng c?n ???ng ng??i ch?i
        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.bodyType = RigidbodyType2D.Static; // Ng?n kh�ng b? ?nh h??ng b?i tr?ng l?c

        // H?y ??i t??ng sau 2 gi�y ?? animation ch?t c� th? di?n ra
        Destroy(gameObject, 2f);
    }

    // V? gizmo trong Editor ?? d? d�ng tinh ch?nh t?m AI
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
