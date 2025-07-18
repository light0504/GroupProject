using UnityEngine;

public class HiddenMap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("Player has entered the cover area. Revealing map...");
    //        //Destroy(this.gameObject); // Assuming the cover is a GameObject that should be destroyed
    //        gameObject.SetActive(false); // Vẫn còn trong scene, chỉ là tắt đi

    //    }
    //}
    private BoxCollider2D m_Collider;
    void Start()
    {
        m_Collider = GetComponent<BoxCollider2D>();
        m_Collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player has entered the hidden map area. Revealing map...");
            if (SceneDataManager.Instance.IsUnlocked())
            {
                Destroy(gameObject);
            }
        }
    }
}
