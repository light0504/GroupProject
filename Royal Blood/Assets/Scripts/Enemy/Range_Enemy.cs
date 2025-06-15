using UnityEngine;

public class Range_Enemy : MonoBehaviour
{
    [Header("AI References")]
    public Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("AI Behavior Ranges")]
    [Tooltip("T?m nh�n t?i ?a ?? ph�t hi?n ng??i ch?i")]
    public float detectionRange = 15f;
    [Tooltip("T?m b?n t?i ?a. Cung th? s? ti?n t?i n?u xa h?n t?m n�y")]
    public float shootingRange = 10f;
    [Tooltip("T?m qu� g?n. Cung th? s? l�i l?i n?u g?n h?n t?m n�y")]
    public float tooCloseRange = 3f;

    [Header("Movement")]
    public float moveSpeed = 1.5f;

    [Header("Attacking")]
    public GameObject arrowPrefab; // K�o Prefab M?i t�n v�o ?�y
    public Transform arrowSpawnPoint; // ?i?m m� m?i t�n s? ???c t?o ra
    public float attackRate = 1f; // T?n c�ng 1 l?n m?i gi�y
    private float nextAttackTime = 0f;

    [Header("Health")]
    public int maxHealth = 60;
    private int currentHealth;

    private bool isFacingRight = true;
    private bool isDead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
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
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ----- LOGIC AI C?A CUNG TH? -----
        if (distanceToPlayer <= shootingRange && distanceToPlayer > tooCloseRange)
        {
            // 1. ? trong t?m b?n l� t??ng -> D?ng l?i v� T?n c�ng
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);
            FlipTowardsPlayer();

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // 2. Th?y ng??i ch?i nh?ng ? sai v? tr� -> Di chuy?n ?? gi? kho?ng c�ch
            if (distanceToPlayer > shootingRange)
            {
                // Qu� xa -> ?i t?i
                MoveTowardsPlayer(1);
            }
            else if (distanceToPlayer <= tooCloseRange)
            {
                // Qu� g?n -> L�i l?i
                MoveTowardsPlayer(-1);
            }
            FlipTowardsPlayer();
        }
        else
        {
            // 3. Ng??i ch?i ? qu� xa -> ??ng y�n
            animator.SetBool("isWalking", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // direction = 1 ?? ?i t?i, -1 ?? l�i l?i
    void MoveTowardsPlayer(int direction)
    {
        animator.SetBool("isWalking", true);
        Vector2 targetDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(targetDirection.x * moveSpeed * direction, rb.linearVelocity.y);
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        // Vi?c b?n t�n s? ???c g?i b?ng Animation Event
    }

    // H�M N�Y S? ???C G?I B?NG ANIMATION EVENT
    public void ShootArrow()
    {
        if (player == null) return;

        // T?o ra m?i t�n
        GameObject arrowGO = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        Arrow arrowScript = arrowGO.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            // T�nh to�n h??ng bay v� b?n
            Vector2 direction = (player.position - arrowSpawnPoint.position).normalized;
            arrowScript.Launch(direction);
        }
    }

    // C�c h�m c�n l?i gi? nguy�n nh? SkeletonAI
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
        isDead = true;
        animator.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // T?m ph�t hi?n
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange); // T?m b?n
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tooCloseRange); // T?m c?n chi?n
    }
}
