using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private string firstLevelSceneName;
    [SerializeField] private string startingCheckpointName = "StartPoint";

    // Các tham chiếu gốc của bạn
    [SerializeField] private NotificationController notificationPopup;
    [SerializeField] private SceneLoader sceneLoader;

    void Start()
    {
        if (continueButton != null)
            continueButton.interactable = (SaveSystem.LoadGame() != null);
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
                "Starting a new game will delete your progress. Are you sure?",
                () => { GameManager.Instance.StartNewGame(firstLevelSceneName, startingCheckpointName); ; }
            );
        }
        else
        {
            GameManager.Instance.StartNewGame(firstLevelSceneName, startingCheckpointName); ;
        }
    }
}