using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool isActive = false;
    private int attackDamage = 40;

    [SerializeField] private Collider2D attackCollider;
    [SerializeField] private float activeDuration = 0.2f; // Thời gian hitbox tồn tại

    [Tooltip("Kéo GameObject chứa script 'Noti' vào đây.")]
    [SerializeField] private Noti notificationManager;
    private void Start()
    {
        attackCollider.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1")) // Nhấn chuột phải
        {
            ActivateHitbox();
            Invoke(nameof(DeactivateHitbox), activeDuration); // Tắt sau 0.2s
        }
    }

    public void SetDamage(int damage)
    {
        attackDamage = damage;
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

    public void IncreaseAttack(int amount)
    {
        if (amount <= 0) return;

        // Tăng sát thương
        attackDamage += amount;

        Debug.Log($"Tăng {amount} damage. Damage hiện tại: {attackDamage}");

        // Kiểm tra xem trình quản lý thông báo có tồn tại không
        if (notificationManager != null)
        {
            // Gọi hàm PrintText để hiển thị thông báo
            notificationManager.PrintText($"Tăng +{amount} Sát thương! (Hiện tại: {attackDamage})");
        }
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
            //    damageable.TakeDamage(attackDamage);
            //}
            BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
            Debug.Log("Enemy component found: " + enemy);
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log("Dealt " + attackDamage + " damage to " + other.name);
                Debug.Log("Máu của enemy = " + enemy.GetHealth());
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
