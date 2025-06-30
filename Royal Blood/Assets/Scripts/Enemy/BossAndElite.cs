using UnityEngine;

public class BossAndElite : MonoBehaviour
{
    private BaseEnemy baseEnemy;
    void Start()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        baseEnemy.canRespawn = false;
        if(SceneDataManager.Instance.IsUnlocked())
            Destroy(gameObject);
    }
}
