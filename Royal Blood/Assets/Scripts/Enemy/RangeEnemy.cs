using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RangeEnemy : BaseEnemy
{
    [Header("Range Specific Settings")]
    [Tooltip("Khoảng cách tối đa để phát hiện người chơi.")]
    public float detectionRange = 15f;
    [Tooltip("Khoảng cách bắn tối ưu. Sẽ tiến tới nếu xa hơn.")]
    public float shootingRange = 10f;
    [Tooltip("Khoảng cách an toàn tối thiểu. Sẽ lùi lại nếu gần hơn.")]
    public float tooCloseRange = 3f;

    [Header("Range Attack Details")]
    public GameObject arrowPrefab;
    public UnityEngine.Transform arrowSpawnPoint;

    // Triển khai logic AI riêng cho kẻ thù tầm xa
    protected override void DecideAction()
    {
        FlipTowardsPlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootingRange && distanceToPlayer > tooCloseRange)
        {
            StopAndAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            Reposition(distanceToPlayer);
        }
        else
        {
            Idle();
        }
    }

    private void Idle()
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void Reposition(float distanceToPlayer)
    {
        if (distanceToPlayer > shootingRange)
        {
            // Người chơi quá xa, tiến lên
            Move(1);
        }
        else if (distanceToPlayer <= tooCloseRange)
        {
            // Người chơi quá gần, lùi lại
            Move(-1);
        }
    }

    // direction: 1 để tiến tới, -1 để lùi lại
    private void Move(int direction)
    {
        if (direction > 0 && IsObstacleAhead() && isGrounded)
        {
            Jump();
        }

        animator.SetBool("isWalking", true);
        float moveDirection = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveSpeed * moveDirection * direction, rb.linearVelocity.y);
    }

    private void StopAndAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    // Được gọi bởi Animation Event
    public void ShootArrow()
    {
        if (player == null || arrowPrefab == null || arrowSpawnPoint == null) return;

        GameObject arrowGO = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        if (arrowGO.TryGetComponent<Arrow>(out Arrow arrowScript))
        {
            Vector2 direction = (player.position - arrowSpawnPoint.position).normalized;
            arrowScript.Launch(direction);
        }
    }

    // Vẽ thêm các gizmo đặc thù
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected(); // Vẽ gizmo chung từ lớp cha

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tooCloseRange);
    }
}