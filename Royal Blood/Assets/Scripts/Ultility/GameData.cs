[System.Serializable]
public class GameData
{
    public string lastSceneName;
    public float[] playerPosition;
    public int[] playerCurrentHealth;
    public int playerAttackPower;

    public GameData(string scene, UnityEngine.Vector3 pPos, int[] health, int attack)
    {
        lastSceneName = scene;
        playerPosition = new float[2] { pPos.x, pPos.y };
        playerCurrentHealth = health;
        playerAttackPower = attack;
    }

    public override string ToString()
    {
        return $"Scene: {lastSceneName}, Pos:({playerPosition[0]:F1},{playerPosition[1]:F1}), " +
               $"HP: {playerCurrentHealth}, ATK: {playerAttackPower}";
    }
}