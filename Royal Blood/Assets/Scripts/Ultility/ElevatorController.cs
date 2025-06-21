using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Điểm Đích (Destination)")]
    [Tooltip("Kéo GameObject Destination_A vào đây")]
    public Transform destination_A;
    [Tooltip("Kéo GameObject Destination_B vào đây")]
    public Transform destination_B;

    [Header("Nút Gọi (Call Buttons)")]
    [Tooltip("Kéo GameObject CallButton_A vào đây")]
    public ElevatorCallButton callButton_A;
    [Tooltip("Kéo GameObject CallButton_B vào đây")]
    public ElevatorCallButton callButton_B;

    [Header("Thông số")]
    public float speed = 4f;

    private Transform currentTarget;
    private bool isMoving = false;
    private bool isPlayerOnboard = false;

    void Start()
    {
        // Kiểm tra lỗi
        if (destination_A == null || destination_B == null || callButton_A == null || callButton_B == null)
        {
            Debug.LogError("Vui lòng gán tất cả các tham chiếu cho Elevator Controller!");
            return;
        }

        // Bắt đầu tại vị trí của Destination_B
        transform.position = destination_B.position;
        currentTarget = destination_B;
        UpdateCallButtons();
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, currentTarget.position) < 0.01f)
            {
                isMoving = false;
                transform.position = currentTarget.position;
                UpdateCallButtons();
            }
        }
    }

    public void CallTo(Transform destination)
    {
        if (!isMoving)
        {
            currentTarget = destination;
            isMoving = true;
            UpdateCallButtons();
        }
    }

    private void UpdateCallButtons()
    {
        // Nếu đang ở gần điểm A, chỉ cho phép tương tác với nút B
        if (Vector2.Distance(transform.position, destination_A.position) < 0.1f)
        {
            callButton_A.SetInteractable(false);
            callButton_B.SetInteractable(true);
        }
        // Nếu đang ở gần điểm B, chỉ cho phép tương tác với nút A
        else if (Vector2.Distance(transform.position, destination_B.position) < 0.1f)
        {
            callButton_A.SetInteractable(true);
            callButton_B.SetInteractable(false);
        }
        // Nếu đang di chuyển, tắt cả hai
        else
        {
            callButton_A.SetInteractable(false);
            callButton_B.SetInteractable(false);
        }
    }

    // Phát hiện người chơi bước vào thang máy để tự động di chuyển
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isMoving && !isPlayerOnboard)
        {
            isPlayerOnboard = true;
            Transform destination = (currentTarget == destination_A) ? destination_B : destination_A;
            CallTo(destination);
        }
    }

    // Các hàm va chạm để dính người chơi vào thang máy
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            DontDestroyOnLoad(collision.gameObject);
            isPlayerOnboard = false;
            UpdateCallButtons();
        }
    }
}