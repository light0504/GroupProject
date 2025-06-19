using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attacking")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public LayerMask enemyLayers;

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

    public int GetAttack()
    {
        return attackDamage;
    }

    public void SetAttackData(int loadedAttackPower)
    {
        attackDamage = loadedAttackPower;
    }
}
