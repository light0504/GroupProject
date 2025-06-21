using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
        }
    }

    public void StartNewGame(string firstSceneName, string startingCheckpointName)
    {
        PlayerTeleporter.dataToLoad = null;
        if (PlayerTeleporter.Instance != null)
        {
            PlayerTeleporter.Instance.TargetEntryPointNameOnNextSceneLoad = null;
        }
        PlayerTeleporter.StartingCheckpointName = startingCheckpointName;

        SaveSystem.DeleteSaveFile();
        sceneLoader.LoadSceneByName(firstSceneName);
    }

    public void ContinueGame()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            PlayerTeleporter.dataToLoad = data;
            sceneLoader.LoadSceneByName(data.lastSceneName);
        }
    }
}