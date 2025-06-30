using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public List<string> unlockedScenes = new List<string>();
    public string nextScene;
    public int lastSceneDiamonds;
    public int lastSceneKey;

    public string lastSceneName;
    public float[] playerPosition;
    public int playerCurrentHealth;
    public int playerMaxHealth;
    public int playerAttackPower;
    public int totalDiamondsCollected;

    public GameData() { }

    public GameData(string savedScene, Vector3 pPos, int currentHealth, int maxHealth, int attack, int totalDiamonds, string lastSceneUnlocked, string nextScene, int nextSceneDiamonds, int nextSceneKey)
    {
        if (!string.IsNullOrEmpty(lastSceneUnlocked))
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


[System.Serializable]
public class SceneData
{
    public string lastSceneUnlocked;
    public string nextScene;
    public int collectedDiamonds;
    public int collectedKeys;

    public SceneData(string lastSceneUnlocked, string nextScene, int diamonds, int keys)
    {
        this.lastSceneUnlocked = lastSceneUnlocked;
        this.nextScene = nextScene;
        this.collectedDiamonds = diamonds;
        this.collectedKeys = keys;
    }
}