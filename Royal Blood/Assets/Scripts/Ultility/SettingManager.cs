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
    // Biến static để các script khác có thể kiểm tra xem game có đang pause không
    public static bool IsGamePaused = false;

    [Header("Main Panel")]
    [Tooltip("Panel chính chứa toàn bộ giao diện của menu cài đặt.")]
    [SerializeField] private GameObject mainPanel;
    /*
    [Header("Page Panels")]
    [Tooltip("Panel chứa các cài đặt Âm thanh.")]
    [SerializeField] private GameObject soundPage;
    [Tooltip("Panel chứa các cài đặt Điều khiển.")]
    [SerializeField] private GameObject controlsPage;
    */
    [Header("In-Game Only Buttons")]
    [Tooltip("Nút 'Save Game', sẽ bị ẩn trong Main Menu.")]
    [SerializeField] private Button saveGameButton;

    [Tooltip("Nút 'Exit to Menu', sẽ bị ẩn trong Main Menu.")]
    [SerializeField] private Button exitToMenuButton;

    [Header("Scene Configuration")]
    [Tooltip("Tên chính xác của scene Main Menu của bạn.")]
    [SerializeField] private string mainMenuSceneName = "Main menu";

    #region Unity Lifecycle Methods

    void Start()
    {
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

    /// <summary>
    /// Mở menu cài đặt và tạm dừng game.
    /// </summary>
    public void PauseGame()
    {
        mainPanel.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;

        CheckCurrentScene();

        ShowSoundPage();
    }

    /// <summary>
    /// Đóng menu cài đặt và tiếp tục game.
    /// </summary>
    public void ResumeGame()
    {
        mainPanel.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    /// <summary>
    /// Hàm này được gán cho nút "Settings" chính trong game để bật/tắt menu.
    /// </summary>
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

    #region Page Navigation

    private void HideAllPages()
    {
        //if (soundPage != null) soundPage.SetActive(false);
        //if (controlsPage != null) controlsPage.SetActive(false);
    }

    /// <summary>
    /// Gán cho nút bấm để hiển thị trang Âm thanh.
    /// </summary>
    public void ShowSoundPage()
    {
        HideAllPages();
        //if (soundPage != null) soundPage.SetActive(true);
    }

    /// <summary>
    /// Gán cho nút bấm để hiển thị trang Điều khiển.
    /// </summary>
    public void ShowControlsPage()
    {
        HideAllPages();
        //if (controlsPage != null) controlsPage.SetActive(true);
    }

    #endregion

    #region Functionality Buttons

    // --- SOUND PAGE ---
    /// <summary>
    /// Gán cho sự kiện OnValueChanged của Slider âm lượng.
    /// </summary>
    public void OnMasterVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume); // Lưu cài đặt đơn giản
    }

    // --- GAME ACTIONS ---
    /// <summary>
    /// Gán cho nút "Save Game".
    /// </summary>
    public void OnSaveGameButtonPressed()
    {
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.TriggerSaveGame();
        }
    }

    /// <summary>
    /// Gán cho nút "Exit to Menu".
    /// </summary>
    public void OnExitToMenuButtonPressed()
    {
        ResumeGame();
        PlayerTeleporter.KillInstance();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    #endregion
}