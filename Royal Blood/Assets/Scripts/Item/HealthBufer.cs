using UnityEngine;

public class HealthBuffer : BaseItem
{
    [Header("Máu tăng khi nhặt")]
    [SerializeField] private int heal;
    protected override void OnCollected(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerHealth>().IncreaseHealth(heal);
    }
}
