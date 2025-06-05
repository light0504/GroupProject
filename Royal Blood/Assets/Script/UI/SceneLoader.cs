using UnityEngine;
using UnityEngine.SceneManagement; // Quan trọng: Namespace để quản lý Scene

#if UNITY_EDITOR
using UnityEditor; // Quan trọng: Namespace cho SceneAsset, chỉ dùng trong Editor
#endif

public class SceneLoader : MonoBehaviour
{
    // Mảng các Scene Asset để kéo thả trong Inspector (chỉ hoạt động trong Editor)
#if UNITY_EDITOR
    [Tooltip("Kéo các Scene Asset bạn muốn có thể load vào đây. Tên Scene sẽ tự động được cập nhật cho mảng 'Scene Names To Load'.")]
    public SceneAsset[] sceneAssetsToLoad;
#endif

    [Tooltip("Mảng tên các Scene có thể load. Được tự động điền từ 'Scene Assets To Load' hoặc có thể nhập thủ công.")]
    public string[] sceneNamesToLoad;

    public void LoadConfiguredSceneFromArray(int arrayIndex)
    {
        if (sceneNamesToLoad == null || arrayIndex < 0 || arrayIndex >= sceneNamesToLoad.Length)
        {
            return;
        }

        string sceneToLoad = sceneNamesToLoad[arrayIndex];
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    // Hàm để thoát game
    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");
        Application.Quit();

#if UNITY_EDITOR
        // Dòng này để dừng Play Mode trong Editor khi nhấn Quit (không ảnh hưởng đến build)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // OnValidate được gọi trong Editor khi script được load hoặc một giá trị thay đổi trong Inspector.
    // Chúng ta dùng nó để tự động cập nhật mảng sceneNamesToLoad khi mảng sceneAssetsToLoad thay đổi.
#if UNITY_EDITOR
    void OnValidate()
    {
        if (sceneAssetsToLoad != null)
        {
            // Đảm bảo mảng sceneNamesToLoad có cùng kích thước với sceneAssetsToLoad
            if (sceneNamesToLoad == null || sceneNamesToLoad.Length != sceneAssetsToLoad.Length)
            {
                sceneNamesToLoad = new string[sceneAssetsToLoad.Length];
            }

            // Cập nhật tên trong sceneNamesToLoad từ các SceneAsset
            for (int i = 0; i < sceneAssetsToLoad.Length; i++)
            {
                if (sceneAssetsToLoad[i] != null)
                {
                    sceneNamesToLoad[i] = sceneAssetsToLoad[i].name;
                }
                else
                {
                    if (sceneNamesToLoad.Length > i) // Đảm bảo không vượt quá giới hạn nếu mảng vừa được tạo
                    {
                        sceneNamesToLoad[i] = null; // Hoặc string.Empty
                    }
                }
            }
        }
        else
        {
            if (sceneNamesToLoad != null && sceneNamesToLoad.Length > 0)
            {
                sceneNamesToLoad = new string[0];
            }
        }
    }
#endif
}