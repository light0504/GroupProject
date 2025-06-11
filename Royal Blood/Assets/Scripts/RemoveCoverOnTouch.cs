using UnityEngine;

public class MapCover : MonoBehaviour
{

    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the cover area. Revealing map...");
            Destroy(this.gameObject); // Assuming the cover is a GameObject that should be destroyed
        }
    }
}