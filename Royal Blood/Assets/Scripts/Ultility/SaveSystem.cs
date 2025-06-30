using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    private static readonly string saveFileName = "gamedata.json";

    private static string GetPath() => Path.Combine(Application.persistentDataPath, saveFileName);

    public static void SaveGame(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(GetPath(), json);
            Debug.Log("Game saved successfully to: " + GetPath());
        }
        catch (Exception ex)
        {
            Debug.LogError("SaveGame failed: " + ex.Message);
        }
    }

    public static GameData LoadGame()
    {
        string path = GetPath();
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            if (data == null)
            {
                Debug.LogError("Failed to parse save file, returning new GameData.");
                return new GameData();
            }

            Debug.Log("Game loaded successfully from: " + path);
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError("LoadGame failed: " + ex.Message);
            return null;
        }
    }

    public static void SaveFullGameData(Vector3 playerPosition, int currentHealth, int maxHealth, int attackPower, int totalDiamonds)
    {
        var oldData = SaveSystem.LoadGame();

        var newData = new GameData(
            SceneDataManager.Instance.GetCurrentSceneName(),
            playerPosition,
            currentHealth,
            maxHealth,
            attackPower,
            totalDiamonds,
            SceneDataManager.Instance.GetCurrentSceneData().lastSceneUnlocked,
            SceneDataManager.Instance.GetCurrentSceneData().nextScene,
            SceneDataManager.Instance.GetCurrentSceneData().collectedDiamonds,
            SceneDataManager.Instance.GetCurrentSceneData().collectedKeys
        );

        if (oldData != null)
        {
            newData.unlockedScenes.AddRange(oldData.unlockedScenes);
        }
        SaveSystem.SaveGame(newData);
        Debug.Log("Full game data saved.");
    }

    public static void DeleteSaveFile()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogWarning("No save file found to delete at: " + path);
        }
    }
}
