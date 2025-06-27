using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nên dùng TextMeshPro

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;

    [Header("Target")]
    [Tooltip("Tham chiếu đến script PlayerHealth cần theo dõi. Sẽ tự tìm nếu để trống.")]
    [SerializeField] private BaseEnemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(enemy.GetHealth(), enemy.GetMaxHealth());
        }
    }

    void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(int currentHp, int maxHp)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = currentHp;
        }
    }
}