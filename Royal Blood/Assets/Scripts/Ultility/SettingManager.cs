using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Quản lý toàn bộ chức năng của Menu Cài đặt.
/// Tự động tạm dừng game, điều hướng giữa các trang, và điều chỉnh giao diện
/// dựa trên việc đang ở Main Menu hay trong màn chơi.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static bool IsGamePaused = false;

    [Header("UI Panels & Components")]
    [Tooltip("Panel chính chứa toàn bộ giao diện của menu cài đặt.")]
    [SerializeField] private GameObject mainPanel;

    [Header("In-Game Only Buttons")]
    [Tooltip("Nút 'Save Game', sẽ bị ẩn trong Main Menu.")]
    [SerializeField] private Button saveGameButton;

    [Tooltip("Nút 'Exit to Menu', sẽ bị ẩn trong Main Menu.")]
    [SerializeField] private Button exitToMenuButton;

    // --- SỬA LỖI QUAN TRỌNG ---
    // Phải là một tham chiếu để kéo thả, không dùng new().
    [Header("Dependencies")]
    [Tooltip("Kéo GameObject chứa script NotificationController vào đây.")]
    [SerializeField] private NotificationController notificationPopup;

    [Header("Configuration")]
    [Tooltip("Tên chính xác của scene Main Menu của bạn.")]
    [SerializeField] private string mainMenuSceneName = "Main menu";

    #region Unity Lifecycle Methods

    void Start()
    {
        // Kiểm tra xem các dependency đã được gán chưa để tránh lỗi
        if (notificationPopup == null) Debug.LogError("NotificationPopup chưa được gán trong SettingsManager!", this);

        // Ban đầu, ẩn panel đi
        mainPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainPanel.activeSelf)
                ResumeGame();
            else
                PauseGame();
        }
    }

    #endregion

    #region Main Menu Control (Pause/Resume)

    public void PauseGame()
    {
        mainPanel.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
        CheckCurrentScene();
    }

    public void ResumeGame()
    {
        mainPanel.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public void OnSettingsButtonPressed()
    {
        if (IsGamePaused) ResumeGame();
        else PauseGame();
    }

    private void CheckCurrentScene()
    {
        bool isInMainMenu = SceneManager.GetActiveScene().name == mainMenuSceneName;
        if (saveGameButton != null) saveGameButton.gameObject.SetActive(!isInMainMenu);
        if (exitToMenuButton != null) exitToMenuButton.gameObject.SetActive(!isInMainMenu);
    }

    #endregion

    #region Functionality Buttons

    public void OnMasterVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void OnSaveGameButtonPressed()
    {
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.TriggerSaveGame();
            // (Tùy chọn) Hiển thị thông báo "Game Saved!" qua NotificationPopup
            // notificationPopup.ShowNotification("Game Saved!", null, true); // Ví dụ: thông báo tự tắt
        }
    }

    public void OnExitToMenuButtonPressed()
    {
        if (notificationPopup == null)
        {
            PerformExitToMenu();
            return;
        }

        notificationPopup.ShowNotification(
            "Progress since last save will be lost. Are you sure you want to exit to the main menu?",
            () => {
                PerformExitToMenu();
            }
        );
    }

    private void PerformExitToMenu()
    {
        ResumeGame();
        PlayerTeleporter.KillInstance();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    #endregion
}