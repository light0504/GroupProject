using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Arrow Stats")]
    public int damage = 15;
    public float speed = 20f;
    public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }
    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if (hitInfo.CompareTag("Player"))
        {
            PlayerHealth player = hitInfo.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // 2. If it does, call the player's TakeDamage function.
                player.GetComponentInChildren<PlayerHealth>().TakeDamage(damage);

                // 3. Destroy the arrow after hitting the player.
                Destroy(gameObject);
                return; // Stop further execution.
            }

            // Destroy the arrow if it hits anything that isn't another enemy.
            if (!hitInfo.CompareTag("Enemy"))
            {
                Destroy(gameObject);
            }
        }
        
    }
}
