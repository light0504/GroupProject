using UnityEngine;

public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager Instance { get; private set; }

    [Header("Cấu hình Scene")]
    public string sceneName;
    public string nextScene;

    [Header("Scene Configuration")]
    [SerializeField] private int diamondsRequired = 5;
    [SerializeField] private int keysRequired = 1;
    [SerializeField] private UIManager uIManager;

    private string currentSceneName;
    private int diamondCollected;
    private int keysCollected;
    private bool nextSceneUnlocked;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        uIManager.UpdateCollectiblesUI();
    }

    public bool CanMoveNextScene()
    {
        if (IsUnlocked())
        {
            return true;
        }
        return diamondCollected >= diamondsRequired && keysCollected >= keysRequired;
    }

    public bool IsUnlocked()
    {
        var gameData = SaveSystem.LoadGame();
        foreach (string scene in gameData.unlockedScenes)
        {
            if (scene.Equals(currentSceneName))
            {
                return true;
            }
        }
        return false;
    }
    public void CollectDiamond()
    {
        diamondCollected++;
        if (nextSceneUnlocked)
            return;
        if (CanMoveNextScene())
        {
            var gameData = SaveSystem.LoadGame();
            if (gameData == null) return;
            SaveSystem.SaveGame(new GameData(gameData.lastSceneName, new Vector3(gameData.playerPosition[0], gameData.playerPosition[1]), gameData.playerCurrentHealth, gameData.playerMaxHealth, gameData.playerAttackPower, gameData.totalDiamondsCollected, currentSceneName, nextScene, 0, 0));
        }
        uIManager.UpdateCollectiblesUI();
    }

    public void CollectKey()
    {
        keysCollected++;
        if(nextSceneUnlocked)
            return;
        if (CanMoveNextScene())
        {
            var gameData = SaveSystem.LoadGame();
            if (gameData == null) return;
            SaveSystem.SaveGame(new GameData(nextScene, new Vector3(gameData.playerPosition[0], gameData.playerPosition[1]), gameData.playerCurrentHealth, gameData.playerMaxHealth, gameData.playerAttackPower, gameData.totalDiamondsCollected, currentSceneName, nextScene, 0, 0));
        }

        uIManager.UpdateCollectiblesUI();
    }

    public int GetCurrentDiamonds() => diamondCollected;
    public int GetCurrentKeys() => keysCollected;

    public int GetRequiredDiamonds() => diamondsRequired;
    public int GetRequiredKey() => keysRequired;
}