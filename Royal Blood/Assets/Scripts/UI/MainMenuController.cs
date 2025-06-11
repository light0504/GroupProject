using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour // Đổi tên class
{
    [Header("Dependencies")]
    [Tooltip("Drag the GameObject containing the NotificationController script here.")]
    [SerializeField] private NotificationController notificationPopup;

    [Tooltip("Drag the GameObject containing the SceneLoader script here.")]
    [SerializeField] private SceneLoader sceneLoader;

    void Start()
    {
        if (notificationPopup == null) Debug.LogError("NotificationPopup is not assigned in MainMenuController!", this);
        if (sceneLoader == null) Debug.LogError("SceneLoader is not assigned in MainMenuController!", this);
    }

    public void OnStartButtonPressed(int index)
    {
        if (sceneLoader != null)
        {
            Debug.Log($"MainMenuController: Requesting to load Gameplay scene with index: {index}");
            sceneLoader.LoadConfiguredSceneFromArray(index);
        }
        else
        {
            Debug.LogError("SceneLoader is not assigned. Cannot load scene!");
        }
    }

    public void OnHowToPlayButtonPressed(int index)
    {
        if (notificationPopup == null)
        {
            Debug.LogError("NotificationPopup is not assigned. Cannot show notification!");
            return;
        }
        if (sceneLoader == null)
        {
            Debug.LogError("SceneLoader is not assigned. Cannot configure notification action!");
            return;
        }

        notificationPopup.ShowNotification(
            "Do you want to view the How To Play instructions?", // Thông báo tiếng Anh
            () => { // YES action
                sceneLoader.LoadConfiguredSceneFromArray(index);
            }
        );
    }

    public void OnExitButtonPressed()
    {
        if (notificationPopup == null)
        {
            Debug.LogError("NotificationPopup is not assigned. Cannot show notification!");
            return;
        }
        if (sceneLoader == null)
        {
            Debug.LogError("SceneLoader is not assigned. Cannot configure notification action!");
            return;
        }

        notificationPopup.ShowNotification(
            "Are you sure you want to quit the game?",
            () => {
                Debug.Log("MainMenuController: User chose to quit the game.");
                sceneLoader.QuitGame();
            }
        );
    }
}