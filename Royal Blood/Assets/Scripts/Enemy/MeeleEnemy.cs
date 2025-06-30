using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MeleeEnemy : BaseEnemy
{
    [Header("Melee Specific Settings")]
    [Tooltip("Khoảng cách tối đa để phát hiện người chơi.")]
    public float detectionRange = 10f;
    [Tooltip("Khoảng cách để thực hiện tấn công.")]
    public float attackRange = 1.5f;

    [Header("Melee Attack Details")]
    public UnityEngine.Transform attackPoint;
    public int attackDamage = 20;
    public LayerMask playerLayer;

    // Triển khai logic AI riêng cho kẻ thù cận chiến
    protected override void DecideAction()
    {
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

        if (Time.time >= nextAttackTime && !player.GetComponent<PlayerHealth>().IsDead())
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    // Được gọi bởi Animation Event
    public void DealMeleeDamage()
    {
        // Create a circle to detect the player.
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        // If the player was detected...
        if (hitPlayer != null)
        {
            // ...get their controller script and call TakeDamage.
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            PlayerHealth player = playerGO.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }

    // Vẽ thêm các gizmo đặc thù
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected(); // Vẽ gizmo chung từ lớp cha

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}