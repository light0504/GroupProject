using UnityEngine;

public class AutoTrackPlayer : MonoBehaviour
{

    private static string playerTag = "Player";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameObject TrackPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        return player;
    }
}
