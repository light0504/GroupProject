using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("Configuration")]
    public string mainMenuSceneName = "Main menu";

    [Header("UI References")]
    [SerializeField] private GameObject endCreditsPanel;

    [Header("Animation")]
    [SerializeField] private Animator creditsAnimator;
    private const string REPLAY_TRIGGER_NAME = "Replay";

    void Start()
    {
        if (endCreditsPanel != null)
        {
            endCreditsPanel.SetActive(false);
        }

        // Bắt đầu chạy credits lần đầu tiên
        ReplayCredits();
    }

    // Được gọi bởi Animation Event khi animation kết thúc
    public void OnCreditsFinished()
    {
        if (endCreditsPanel != null)
        {
            endCreditsPanel.SetActive(true);
        }
    }

    // Được gán cho nút "Replay"
    public void ReplayCredits()
    {
        if (endCreditsPanel != null)
        {
            endCreditsPanel.SetActive(false);
        }

        if (creditsAnimator != null)
        {
            creditsAnimator.SetTrigger(REPLAY_TRIGGER_NAME);
        }
    }

    // Được gán cho nút "Main Menu"
    public void ReturnToMainMenu()
    {
        SaveSystem.DeleteSaveFile();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}