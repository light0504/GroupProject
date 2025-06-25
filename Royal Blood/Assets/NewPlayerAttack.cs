using UnityEngine;

public class NewPlayerAttack : MonoBehaviour
{
    private bool isActive = false;
    private int currentDamage = 40;

    [SerializeField] private Collider2D attackCollider;
    [SerializeField] private float activeDuration = 0.2f; // Thời gian hitbox tồn tại

    private void Start()
    {
        attackCollider.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Nhấn chuột phải
        {
            ActivateHitbox();
            Invoke(nameof(DeactivateHitbox), activeDuration); // Tắt sau 0.2s
        }
    }

    public void SetDamage(int damage)
    {
        currentDamage = damage;
    }

    public void ActivateHitbox()
    {
        isActive = true;
        attackCollider.enabled = true;
        //Debug.Log("Hitbox Activated!");
    }

    public void DeactivateHitbox()
    {
        isActive = false;
        attackCollider.enabled = false;
        //Debug.Log("Hitbox Deactivated!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy: " + other.name);

            //IDamageable damageable = other.GetComponent<IDamageable>();
            //if (damageable != null)
            //{
            //    damageable.TakeDamage(currentDamage);
            //}
            Enemy enemy = other.GetComponentInParent<Enemy>();
            Debug.Log("Enemy component found: " + enemy);
            if (enemy != null)
            {
                enemy.TakeDamage(currentDamage);
                Debug.Log("Dealt " + currentDamage + " damage to " + other.name);
                Debug.Log("Máu của enemy = " + enemy.GetHealth());
            }
            Range_Enemy range_Enemy = other.GetComponentInParent<Range_Enemy>();
            Debug.Log("Enemy component found: " + enemy);
            if (range_Enemy != null)
            {
                range_Enemy.TakeDamage(currentDamage);
                Debug.Log("Dealt " + currentDamage + " damage to " + other.name);
                Debug.Log("Máu của enemy = " + range_Enemy.GetHealth());
            }
        }
    }
}
