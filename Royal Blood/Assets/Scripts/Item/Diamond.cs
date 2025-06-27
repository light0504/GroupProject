using UnityEngine;

public class Diamond : BaseItem
{
    protected override void OnCollected(GameObject playerObject)
    {
        SceneDataManager.Instance.CollectDiamond();
        AutoTrackPlayer.TrackPlayer().GetComponent<ItemPicker>()?.LootDiamonds();
    }
}
