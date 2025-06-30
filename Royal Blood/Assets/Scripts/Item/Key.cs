using UnityEngine;

public class Key : BaseItem
{
    protected override void OnCollected(GameObject playerObject)
    {
        SceneDataManager.Instance.CollectKey();
    }
}
