using UnityEngine;
using TMPro;

[RequireComponent(typeof(CircleCollider2D))]
public class TextBlock : MonoBehaviour
{

    [Header("Hiển thị Thông tin Portal")]
    public string textDisplayed;
    public TextMeshProUGUI destinationText;
    //public bool canReview;
    public float trackRadius;

    private CircleCollider2D infoCollider;

    private void Awake()
    {
        infoCollider = GetComponent<CircleCollider2D>();
        infoCollider.radius = trackRadius;
        infoCollider.isTrigger = true;

        if (destinationText != null)
        {
            destinationText.text = textDisplayed;
            destinationText.gameObject.SetActive(false);
        }
    }

    // --- Xử lý va chạm ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.IsTouching(infoCollider))
        {
            if (destinationText != null) 
            {
                destinationText.gameObject.SetActive(true); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && destinationText != null)
        {
            //if (!canReview)
            //{
            //    Destroy(this.gameObject);
            //}
            
            destinationText.gameObject.SetActive(false);
        }
    }
}