using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Quản lý việc hiển thị và tự động ẩn một đối tượng TextMeshProUGUI để tạo thông báo.
/// </summary>
public class Noti : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Thời gian (giây) mà thông báo sẽ hiển thị trước khi tự động ẩn đi.")]
    public float displayDuration = 3f; // Đổi tên từ hideDelay để rõ nghĩa hơn

    [Header("References")]
    [Tooltip("Kéo đối tượng TextMeshProUGUI từ Hierarchy vào đây.")]
    public TextMeshProUGUI notificationText; // Đổi tên để rõ ràng hơn

    // Biến để lưu trữ coroutine đang chạy, giúp chúng ta quản lý nó
    private Coroutine hideCoroutine;

    void Awake()
    {
        Hide();
    }

    /// <summary>
    /// Hiển thị một chuỗi văn bản thông báo và tự động ẩn sau một khoảng thời gian.
    /// </summary>
    /// <param name="textToShow">Nội dung của thông báo.</param>
    public void PrintText(string textToShow)
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        notificationText.text = textToShow;
        notificationText.enabled = true;

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// Coroutine chờ một khoảng thời gian rồi gọi hàm Hide().
    /// </summary>
    private IEnumerator HideAfterDelay()
    {
        // Chờ trong 'displayDuration' giây
        yield return new WaitForSeconds(displayDuration);

        // Sau khi chờ xong, ẩn thông báo
        Hide();
        // Đặt lại coroutine về null để báo hiệu nó đã kết thúc
        hideCoroutine = null;
    }

    /// <summary>
    /// Ẩn đối tượng TextMeshPro.
    /// </summary>
    private void Hide()
    {
        if (notificationText != null)
        {
            notificationText.enabled = false;
        }
    }
}