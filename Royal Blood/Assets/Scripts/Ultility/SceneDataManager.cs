using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager Instance { get; private set; }

    [Header("Cấu hình Scene")]
    public string nextScene;

    [Header("Scene Configuration")]
    [SerializeField] private int diamondsRequired = 5;
    [SerializeField] private int keysRequired = 1;
    [SerializeField] private UIManager uIManager;
    [SerializeField] public bool IsBossScene = false;
    private string currentSceneName;
    private int diamondCollected;
    private int keysCollected;

    public bool isBossKilled = true;
    private SceneData currentSceneData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        currentSceneName = SceneManager.GetActiveScene().name;

        var gameData = SaveSystem.LoadGame();
        //Nếu là màn boss, chỉnh thành chưa tiêu diệt boss
        if (IsBossScene) isBossKilled = false; else isBossKilled = true;
        //Tạo data tạm thời
        LoadSceneData();
        //Nếu có data, và màn hiện tại cần mở khóa, gán cho data tạm thời. 
        //Nếu không, coi như đã hoàn thành nhiệm vụ.
        if (gameData != null)
        {
            if (gameData.unlockedScenes.Contains(currentSceneName))
            {
                diamondCollected = diamondsRequired;
                keysCollected = keysRequired;
                isBossKilled = true;
                
            }
            else
            {
                diamondCollected = gameData.lastSceneDiamonds;
                keysCollected = gameData.lastSceneKey;
                currentSceneData.collectedDiamonds = gameData.lastSceneDiamonds;
                currentSceneData.collectedKeys = gameData.lastSceneKey;
                currentSceneData.lastSceneUnlocked = null;
                currentSceneData.nextScene = gameData.nextScene;
            }
        }
        uIManager.UpdateCollectiblesUI();
    }

    public void BossKilled()
    {
      
        isBossKilled = true;
        SaveTemporary();
    }
    private void LoadSceneData()
    {
        currentSceneData = new SceneData("", currentSceneName, 0, 0);
    }

    public bool CanMoveNextScene()
    {
        return IsUnlocked() || IsReachRequirement() ;
    }

    // Kiểm tra đã đạt yêu cầu chưa
    public bool IsReachRequirement()
    {
        return (diamondCollected >= diamondsRequired && keysCollected >= keysRequired) && isBossKilled;
    }
    //Kiểm tra đã từng lưu chưa
    public bool IsUnlocked()
    {
        var gameData = SaveSystem.LoadGame();
        return gameData != null && gameData.unlockedScenes.Contains(currentSceneName);
    }
    
    public void CollectDiamond()
    {
        if (!IsUnlocked()) 
        {
            diamondCollected++;
            currentSceneData.collectedDiamonds = diamondCollected;
            SaveTemporary();
            uIManager.UpdateCollectiblesUI();
        }
        
    }

    public void CollectKey()
    {
        if (!IsUnlocked())
        {
            keysCollected++;
            currentSceneData.collectedKeys = keysCollected;
            SaveTemporary();
            uIManager.UpdateCollectiblesUI();
        }
    }

    private void SaveTemporary()
    {
        if (IsReachRequirement() && !IsUnlocked())
        {
            //Nếu đã đạt yêu cầu nhưng chưa lưu => Lưu
            currentSceneData = new(currentSceneName, nextScene, 0, 0);
            AutoTrackPlayer.TrackPlayer().GetComponent<PlayerState>().TriggerSaveGame();
        }
    }
    public SceneData GetCurrentSceneData() => currentSceneData;
    public string GetCurrentSceneName() => currentSceneName;
    public int GetCurrentDiamonds() => diamondCollected;
    public int GetCurrentKeys() => keysCollected;

    public int GetRequiredDiamonds() => diamondsRequired;
    public int GetRequiredKey() => keysRequired;
}