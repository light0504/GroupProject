using UnityEngine;

public class BossEnemy : BaseEnemy
{
    [Header("Detection & Attack Settings")]
    [Tooltip("Maximum range to detect the player.")]
    public float detectionRange = 50f;
    public float attackRangeA = 15f;
    public float attackRangeB = 10f;
    public float attackRangeC = 15f;

    [Header("Attack Points")]
    public Transform attackPointA;
    public Transform attackPointB;
    public Transform attackPointC;
    public int attackDamage = 50;
    public LayerMask playerLayer;

    private int currentAttackIndex = 0;

    protected override void DecideAction()
    {
        if (player == null) return;

        FlipTowardsPlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float maxAttackRange = Mathf.Max(attackRangeA, attackRangeB, attackRangeC);

        if (distanceToPlayer <= maxAttackRange)
        {
            AttackBehavior(distanceToPlayer);
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChaseBehavior();
        }
        else
        {
            IdleBehavior();
        }

    }

    private void IdleBehavior()
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void ChaseBehavior()
    {
        if (IsObstacleAhead() && isGrounded)
        {
            Jump();
        }

        animator.SetBool("isWalking", true);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }

    private void AttackBehavior(float distanceToPlayer)
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = Vector2.zero;

        if (Time.time < nextAttackTime || player.GetComponent<PlayerHealth>().IsDead()) return;

        // Determine possible attacks based on distance
        var possibleAttacks = new System.Collections.Generic.List<int>();
        if (distanceToPlayer <= attackRangeA) possibleAttacks.Add(0);
        if (distanceToPlayer <= attackRangeB) possibleAttacks.Add(1);
        if (distanceToPlayer <= attackRangeC) possibleAttacks.Add(2);

        if (possibleAttacks.Count == 0) return;

        // Randomly pick one of the valid attacks
        currentAttackIndex = possibleAttacks[Random.Range(0, possibleAttacks.Count)];

        // Trigger appropriate animation
        switch (currentAttackIndex)
        {
            case 0: animator.SetTrigger("AttackA"); break;
            case 1: animator.SetTrigger("AttackB"); break;
            case 2: animator.SetTrigger("AttackC"); break;
        }

        nextAttackTime = Time.time + 1f / attackRate;
    }

    // Called via animation event
    public void DealMeleeDamage()
    {
        Transform[] attackPoints = { attackPointA, attackPointB, attackPointC };
        float[] attackRanges = { attackRangeA, attackRangeB, attackRangeC };

        if (currentAttackIndex < 0 || currentAttackIndex >= attackPoints.Length) return;

        Transform selectedPoint = attackPoints[currentAttackIndex];
        float selectedRange = attackRanges[currentAttackIndex];

        if (selectedPoint == null) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(selectedPoint.position, selectedRange, playerLayer);
        if (hitPlayer != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                // Optional: Debug.Log($"Boss hit player with Attack {(char)('A' + currentAttackIndex)}");
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        if (attackPointA != null) Gizmos.DrawWireSphere(attackPointA.position, attackRangeA);
        if (attackPointB != null) Gizmos.DrawWireSphere(attackPointB.position, attackRangeB);
        if (attackPointC != null) Gizmos.DrawWireSphere(attackPointC.position, attackRangeC);
    }

    public void OnBossDeathAnimationFinished()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        SceneDataManager.Instance.BossKilled();
        Destroy(gameObject);
    }

}
