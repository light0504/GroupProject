using UnityEngine;

public class ElevatorCallButton : MonoBehaviour
{
    [Header("Kết nối")]
    [Tooltip("Kéo Elevator_Container vào đây")]
    public ElevatorController elevatorController;

    [Tooltip("Kéo ĐIỂM ĐÍCH mà nút này sẽ gọi (Destination_A hoặc Destination_B)")]
    public Transform destinationPoint;

    [Tooltip("Kéo GameObject Text vào đây (tùy chọn)")]
    public GameObject interactionText;

    private bool isPlayerInRange = false;

    void Start()
    {
        if (interactionText != null) interactionText.SetActive(false);
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Kiểm tra để chắc chắn đã gán các tham chiếu
            if (elevatorController != null && destinationPoint != null)
            {
                // Gửi lệnh gọi, kèm theo ĐIỂM ĐÍCH
                elevatorController.CallTo(destinationPoint);
            }
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        GetComponent<Collider2D>().enabled = isInteractable;
        GetComponent<BoxCollider2D>().enabled = isInteractable;
        if (!isInteractable)
        {
            isPlayerInRange = false;
            if (interactionText != null) interactionText.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionText != null) interactionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionText != null) interactionText.SetActive(false);
        }
    }
}