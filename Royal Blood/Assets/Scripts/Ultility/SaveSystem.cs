using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static readonly string saveFileName = "gamedata.sav";

    private static string GetPath() => Path.Combine(Application.persistentDataPath, saveFileName);

    public static void SaveGame(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(GetPath(), FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
        Debug.Log("Game Saved: " + data);
    }

    public static GameData LoadGame()
    {
        string path = GetPath();
        if (!File.Exists(path)) return null;

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                GameData data = formatter.Deserialize(stream) as GameData;
                Debug.Log("Game Loaded: " + data);
                return data;
            }
        }
        catch { return null; }
    }

    public static void DeleteSaveFile()
    {
        string path = GetPath();
        if (File.Exists(path)) File.Delete(path);
    }
}