using UnityEngine;

/// <summary>
/// Kích hoạt việc chuyển scene khi Player đi vào.
/// Portal này sẽ giao tiếp với PlayerTeleporter để đặt vị trí cho Player ở scene tiếp theo
/// và sử dụng một SceneLoader để thực hiện việc load scene.
/// </summary>
[RequireComponent(typeof(Collider2D))] // Đảm bảo luôn có Collider2D để phát hiện va chạm
public class Portal : MonoBehaviour
{
    [Header("Scene Loader Configuration")]
    [Tooltip("Kéo GameObject chứa component SceneLoader vào đây. Đây là object sẽ thực sự load scene.")]
    public SceneLoader sceneLoader;

    [Tooltip("Chỉ số (index) của scene trong mảng 'Scene Names To Load' của SceneLoader mà bạn muốn chuyển đến.")]
    public int sceneIndexInLoader;

    [Header("Player Teleportation")]
    [Tooltip("Tên của EntryPoint trong scene đích mà người chơi sẽ xuất hiện. Phải khớp chính xác.")]
    public string targetEntryPointName;


    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sceneLoader == null)
            {
                Debug.LogError($"Portal '{gameObject.name}' chưa được gán SceneLoader! Không thể chuyển scene.", this.gameObject);
                return;
            }

            PlayerTeleporter.Instance.TargetEntryPointNameOnNextSceneLoad = targetEntryPointName;

            // 2. Yêu cầu SceneLoader load scene dựa trên chỉ số (index) đã cấu hình
            sceneLoader.LoadConfiguredSceneFromArray(sceneIndexInLoader);
        }
    }

    // Vẽ Gizmos trong Editor để dễ dàng hình dung cổng dịch chuyển
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0, 1, 0.5f);
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Vector3 worldCenter = transform.TransformPoint(boxCollider.offset);
            Vector3 worldSize = Vector3.Scale(transform.lossyScale, boxCollider.size);
            Gizmos.DrawCube(worldCenter, worldSize);
        }
        else
        {
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}