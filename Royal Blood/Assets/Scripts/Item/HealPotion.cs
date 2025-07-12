using UnityEngine;

public class HealthPotion : BaseItem
{
    [Header("% máu hồi khi nhặt")]
    [Range(0f, 1f)]
    [SerializeField] private float heal;
    protected override void OnCollected(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerHealth>().HealPercent(heal);
    }
}
