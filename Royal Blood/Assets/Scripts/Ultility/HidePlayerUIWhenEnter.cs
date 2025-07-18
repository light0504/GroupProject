using UnityEngine;

public class HidePlayerUIWhenEnter : MonoBehaviour
{
    void Start()
    {
        GameObject go = AutoTrackPlayer.TrackPlayer().gameObject;

        // Tìm component Canvas trong các children của đối tượng
        Canvas canvas = go.GetComponentInChildren<Canvas>(true); // true để tìm cả những đối tượng đang bị disable

        if (canvas != null)
        {
            canvas.gameObject.SetActive(false); // Ẩn Canvas
        }
        else
        {
            //Debug.LogWarning("No Canvas found in children of the player object.");
        }
    }
}
