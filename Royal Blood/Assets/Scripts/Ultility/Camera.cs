using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineFindPlayer : MonoBehaviour
{
    [Tooltip("Tag của đối tượng Player mà camera sẽ tìm kiếm.")]
    [SerializeField] private string playerTag = "Player";

    void Start()
    {
        var virtualCamera = GetComponent<CinemachineCamera>();
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            virtualCamera.Follow = playerObject.transform;
            virtualCamera.LookAt = playerObject.transform;
        }
        else
        {
            Debug.LogWarning($"CinemachineFindPlayer: Không tìm thấy đối tượng nào có tag '{playerTag}' trong scene.");
        }
    }
}