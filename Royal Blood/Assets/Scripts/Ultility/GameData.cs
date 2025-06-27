using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    
    public int lastSceneDiamonds;
    public int lastSceneKey;

    public List<string> unlockedScenes = new List<string>();
    public string nextScene;
    public string lastSceneName;
    public float[] playerPosition;
    public int playerCurrentHealth;
    public int playerMaxHealth;
    public int playerAttackPower;
    public int totalDiamondsCollected;

    public GameData(string savedScene, Vector3 pPos, int currentHealth, int maxHealth, int attack, int totalDiamonds, string lastSceneUnlocked, string nextScene, int nextSceneDiamonds, int nextSceneKey)
    {
        unlockedScenes.Add(lastSceneUnlocked);
        this.nextScene = nextScene;
        this.lastSceneDiamonds = nextSceneDiamonds;
        this.lastSceneKey = nextSceneKey;

        lastSceneName = savedScene;
        playerPosition = new float[2] { pPos.x, pPos.y };
        playerCurrentHealth = currentHealth;
        playerMaxHealth = maxHealth;
        playerAttackPower = attack;
        totalDiamondsCollected = totalDiamonds;
    }

    public override string ToString()
    {
        return $"Scene: {lastSceneName}, Pos:({playerPosition[0]:F1},{playerPosition[1]:F1}), " +
               $"HP: {playerCurrentHealth}, ATK: {playerAttackPower}, " +
               $"Diamonds: {totalDiamondsCollected}";
    }
}
