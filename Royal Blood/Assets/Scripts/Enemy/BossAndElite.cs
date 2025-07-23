using UnityEngine;

public class BossAndElite : MonoBehaviour
{
    private BaseEnemy baseEnemy;
    void Update()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        baseEnemy.canRespawn = false;
        if(SceneDataManager.Instance.CanMoveNextScene())
            Destroy(gameObject);
    }
}
