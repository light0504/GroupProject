using UnityEngine;

public class AtkBuffer : BaseItem
{
    [Header("ATK Tăng khi nhặt")]
    [SerializeField] private int atk;
    protected override void OnCollected(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerAttack>().IncreaseAttack(atk);
    }
}
