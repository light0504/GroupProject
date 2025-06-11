using UnityEngine;

public class HiddenMap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the cover area. Revealing map...");
            Destroy(this.gameObject); // Assuming the cover is a GameObject that should be destroyed
        }
    }
}
