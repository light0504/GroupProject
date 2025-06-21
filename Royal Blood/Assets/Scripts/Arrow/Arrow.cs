using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Arrow Stats")]
    public int damage = 15;
    public float speed = 20f;
    public Rigidbody2D rb;

    void Awake()
    {
        // L?y component Rigidbody2D khi m?i t�n ???c t?o ra
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // M?i t�n s? t? h?y sau 3 gi�y n?u kh�ng tr�ng b?t c? th? g�
        // ?i?u n�y gi�p d?n d?p Scene v� tr�nh l�ng ph� b? nh?
        Destroy(gameObject, 3f);
    }

    // H�m n�y ???c g?i b?i ArcherAI ?? b?n m?i t�n theo m?t h??ng c? th?
    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

        // Xoay h�nh ?nh m?i t�n theo h??ng bay ?? tr�ng t? nhi�n
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // H�m n�y ???c g?i khi m?i t�n va ch?m v?i m?t ??i t??ng kh�c c� Collider2D
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. Check if the object we hit has a PlayerController script.
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
