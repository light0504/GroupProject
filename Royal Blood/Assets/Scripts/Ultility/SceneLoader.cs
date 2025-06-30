using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneLoader : MonoBehaviour
{
#if UNITY_EDITOR
    [Tooltip("Kéo các Scene Asset vào đây. Tên scene sẽ tự động được điền vào mảng bên dưới.")]
    public SceneAsset[] sceneAssetsToLoad;
#endif

    [Tooltip("Danh sách tên các scene có thể được tải. Tự động cập nhật từ mảng Scene Assets ở trên.")]
    public string[] sceneNamesToLoad;

    public void LoadConfiguredSceneFromArray(int index)
    {
        if (sceneNamesToLoad == null || index < 0 || index >= sceneNamesToLoad.Length || string.IsNullOrEmpty(sceneNamesToLoad[index])) return;
        SceneManager.LoadScene(sceneNamesToLoad[index]);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        SceneManager.LoadScene(sceneName);
    }


    /// <summary>
    /// Thoát khỏi trò chơi.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("SceneLoader: Đang thoát game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (sceneAssetsToLoad != null)
        {
            sceneNamesToLoad = new string[sceneAssetsToLoad.Length];
            for (int i = 0; i < sceneAssetsToLoad.Length; i++)
            {
                sceneNamesToLoad[i] = (sceneAssetsToLoad[i] != null) ? sceneAssetsToLoad[i].name : "";
            }
        }
    }
#endif
}