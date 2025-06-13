using UnityEngine;

/// <summary>
/// Đánh dấu một vị trí trong Scene nơi Player có thể xuất hiện.
/// PlayerTeleporter sẽ tìm một EntryPoint có entryName khớp với
/// target name được chỉ định để đặt Player vào vị trí này.
/// </summary>
public class EntryPoint : MonoBehaviour
{
    [Tooltip("Tên định danh cho điểm vào này. Tên này phải là duy nhất trong scene và phải khớp với tên mà Portal ở scene trước đó chỉ định.")]
    public string entryName;

    private void OnDrawGizmos()
    {
        // Vẽ một quả cầu màu xanh tại vị trí của EntryPoint
        Gizmos.color = new Color(0, 1, 1, 0.75f); // Màu Cyan với độ trong suốt
        Gizmos.DrawSphere(transform.position, 0.5f);

        // Vẽ một mũi tên chỉ hướng "tiến" (hữu ích cho việc xác định hướng nhìn của nhân vật khi xuất hiện)
        // Trong game 2D, thường là hướng sang phải (transform.right)
        Gizmos.color = Color.yellow;
        Vector3 arrowEndPosition = transform.position + transform.right * 1.0f; // Hướng sang phải 1 đơn vị
        Gizmos.DrawLine(transform.position, arrowEndPosition);
    }
}