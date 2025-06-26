using UnityEngine;

public class HealthBuffer : BaseItem
{
    protected override void OnCollected(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerHealth>().IncreaseHealth(10);
    }
}
