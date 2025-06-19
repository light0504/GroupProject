using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button continueButton;

    [Header("Dependencies")]
    [SerializeField] private NotificationController notificationPopup;
    [SerializeField] private SceneLoader sceneLoader;

    [Header("New Game Configuration")]
    [SerializeField] private string firstLevelSceneName = "Tutorial_Level";
    [SerializeField] private string startingCheckpointName = "StartPoint";

    void Start()
    {
        if (GameManager.Instance == null)
            Debug.LogError("MainMenuController không tìm thấy GameManager.Instance!", this);
        if (notificationPopup == null)
            Debug.LogWarning("NotificationPopup chưa được gán trong MainMenuController.", this);
        if (sceneLoader == null)
            Debug.LogError("SceneLoader chưa được gán trong MainMenuController!", this);

        UpdateContinueButtonState();
    }

    public void OnContinueButtonPressed()
    {
        GameManager.Instance.ContinueGame();
    }

    public void OnNewGameButtonPressed()
    {
        if (SaveSystem.LoadGame() != null && notificationPopup != null)
        {
            notificationPopup.ShowNotification(
                "Starting a new game will overwrite your progress. Are you sure?",
                () => { GameManager.Instance.StartNewGame(firstLevelSceneName, startingCheckpointName); }
            );
        }
        else
        {
            GameManager.Instance.StartNewGame(firstLevelSceneName, startingCheckpointName);
        }
    }

    public void OnHowToPlayButtonPressed(int howToPlaySceneIndex)
    {
        if (sceneLoader == null)
        {
            Debug.LogError("SceneLoader chưa được gán, không thể tải scene How To Play!");
            return;
        }

        if (notificationPopup != null)
        {
            notificationPopup.ShowNotification(
                "Do you want to view the How To Play instructions?",
                () =>
                {
                    sceneLoader.LoadConfiguredSceneFromArray(howToPlaySceneIndex);
                }
            );
        }
        else
        {
            sceneLoader.LoadConfiguredSceneFromArray(howToPlaySceneIndex);
        }
    }

    public void OnSettingsButtonPressed()
    {
        Debug.Log("Nút Settings đã được nhấn.");
    }

    public void OnExitButtonPressed()
    {
        if (notificationPopup != null)
        {
            notificationPopup.ShowNotification(
                "Are you sure you want to quit?",
                static () =>
                {
                    Application.Quit();

#if UNITY_EDITOR 
                    UnityEditor.EditorApplication.isPlaying = false;
#endif

                }
            );
        }
        else
        {
            Application.Quit();
        }
    }

    private void UpdateContinueButtonState()
    {
        if (continueButton != null)
        {
            continueButton.interactable = (SaveSystem.LoadGame() != null);
        }
    }
}