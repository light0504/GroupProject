using UnityEngine;
using UnityEngine.Tilemaps;

public class Trap : MonoBehaviour
{
    private TilemapCollider2D TilemapCollider2D;
    private bool isWarning = false;

    [Header("Sát thương gây ra")]
    [Tooltip("Sát thương gây ra cho người chơi và quái vật")]
    [SerializeField]private int playerDamage = 50;
    [SerializeField]private int enemyDamage = 200;

    private void Start()
    {
        TilemapCollider2D = GetComponent<TilemapCollider2D>();
        TilemapCollider2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(playerDamage);
            if (!isWarning)
            {
                collision.gameObject.GetComponent<Noti>().PrintText("Cẩn thận cạm bẫy, hãy tận dụng để tiêu diệt quái vật");
                isWarning = true;
            } 
        }
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BaseEnemy>().TakeDamage(enemyDamage);
        }
    }
}
