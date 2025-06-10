using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;          

public class NotificationController : MonoBehaviour
{
    [Header("UI References (Con của GameObject này)")]
    [Tooltip("GameObject chính của UI thông báo, sẽ được ẩn/hiện.")]
    [SerializeField] private GameObject notiUiRoot; // Kéo Noti_Canvas (hoặc Panel chính) vào đây

    [SerializeField] private Text messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Tooltip("Hành động sẽ thực hiện khi ấn yes")]
    private Action onYesAction;

    void Awake()
    {
        if (notiUiRoot == null)
            Debug.LogError("Noti UI Root chưa được gán trong NotificationController!", this);
        if (messageText == null)
            Debug.LogError("Message Text chưa được gán trong NotificationController!", this);
        if (yesButton == null)
            Debug.LogError("Yes Button chưa được gán trong NotificationController!", this);
        // if (noButton == null) Debug.LogWarning("No Button chưa được gán trong NotificationController (tùy chọn).", this);

        // Gán sự kiện cho các nút
        yesButton.onClick.AddListener(OnYesButtonPressed);
        if (noButton != null)
        {
            noButton.onClick.AddListener(OnNoButtonPressed);
        }

        if (notiUiRoot != null)
        {
            notiUiRoot.SetActive(false);
        }
    }

    /// <summary>
    /// Hiển thị thông báo với nội dung và hành động cho nút Yes.
    /// </summary>
    /// <param name="message">Nội dung thông báo.</param>
    /// <param name="yesAction">Hành động sẽ thực thi khi nhấn nút Yes.</param>
    public void ShowNotification(string message, Action yesAction)
    {
        if (notiUiRoot == null || messageText == null || yesButton == null)
        {
            Debug.LogError("Không thể hiển thị thông báo do thiếu tham chiếu UI.");
            return;
        }

        messageText.text = message;
        this.onYesAction = yesAction;

        notiUiRoot.SetActive(true);
    }

    private void OnYesButtonPressed()
    {
        onYesAction?.Invoke();

        HideNotification();
    }

    private void OnNoButtonPressed()
    {
        HideNotification();
    }

    public void HideNotification()
    {
        if (notiUiRoot != null)
        {
            notiUiRoot.SetActive(false);
        }
        onYesAction = null;
    }
}