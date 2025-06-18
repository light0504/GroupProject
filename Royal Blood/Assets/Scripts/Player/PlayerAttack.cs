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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            // EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     enemyHealth.TakeDamage(attackDamage);
            // }
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
