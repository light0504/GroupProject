using UnityEngine;

/// <summary>
/// Lớp cơ sở trừu tượng cho tất cả các vật phẩm có thể nhặt được trong game.
/// Script này phải được gắn vào Prefab của mỗi vật phẩm.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class BaseItem : MonoBehaviour
{
    [Tooltip("Prefab của vật phẩm sẽ được tạo ra.")]
    public GameObject itemPrefab;

    [Tooltip("Tỷ lệ rơi của vật phẩm này (giá trị từ 0.0 đến 1.0, ví dụ: 0.1 = 10%).")]
    [Range(0f, 1f)]
    public float dropChance = 0.5f;

    public float pullSpeed = 8f;
    private Transform targetPlayer;
    private bool isBeingPulled = false;

    /// <summary>
    /// Định nghĩa hành động cụ thể khi vật phẩm được nhặt bởi người chơi.
    /// Ví dụ: hồi máu, cộng điểm, tăng sức mạnh,...
    /// </summary>
    /// <param name="playerObject">GameObject của người chơi đã nhặt vật phẩm.</param>
    protected abstract void OnCollected(GameObject playerObject);


    /// <summary>
    /// Được gọi ngay sau khi vật phẩm được tạo ra trên thế giới game.
    /// Mặc định chỉ đặt vị trí. Có thể được ghi đè để thêm hiệu ứng vật lý như nảy lên.
    /// </summary>
    /// <param name="position">Vị trí để vật phẩm xuất hiện.</param>
    public virtual void Initialize(Vector3 position)
    {
        transform.position = position;
    }

    public void StartPull(Transform playerTransform)
    {
        if (!isBeingPulled)
        {
            targetPlayer = playerTransform;
            isBeingPulled = true;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollected(other.gameObject);

            Destroy(gameObject);
        }
    }


}