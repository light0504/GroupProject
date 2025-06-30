using UnityEngine;

public class AtkBuffer : BaseItem
{
    protected override void OnCollected(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerAttack>().IncreaseAttack(1);
    }
}
