using Unity.VisualScripting;
using UnityEngine;

public class HiddenMapOpener : MonoBehaviour
{
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
            if (SceneDataManager.Instance.IsUnlocked())
            {
                Destroy(gameObject);
            }
        }
    }
}
