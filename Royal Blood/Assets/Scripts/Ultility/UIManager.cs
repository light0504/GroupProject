using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Collectibles UI")]
    [SerializeField] private TextMeshProUGUI diamondsText;
    [SerializeField] private TextMeshProUGUI keysText;
    [SerializeField] private SceneDataManager sceneDataManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCollectiblesUI()
    {
        if (sceneDataManager.CanMoveNextScene())
        {
            if (diamondsText != null)
            {
                diamondsText.text = $"Mission completed";
            }
            if (keysText != null)
            {
                keysText.text = "";
            }
        }
        else
        {
            Debug.Log(sceneDataManager.IsBossScene);
            Debug.Log(sceneDataManager.isBossKilled);
            if (sceneDataManager.IsBossScene)
            {
                if (!sceneDataManager.isBossKilled)
                {
                    if (diamondsText != null)
                    {
                        diamondsText.text = $"Defeat BOSS: 0/1";
                    }
                    if (keysText != null)
                    {
                        keysText.text = "";
                    }
                }
            }
            else
            {
                if (diamondsText != null)
                {
                    diamondsText.text = $"Diamonds: {SceneDataManager.Instance.GetCurrentDiamonds()}/{SceneDataManager.Instance.GetRequiredDiamonds()}";
                }

                if (keysText != null)
                {
                    keysText.text = $"Keys: {SceneDataManager.Instance.GetCurrentKeys()}/{SceneDataManager.Instance.GetRequiredKey()}";
                }
            }

        }

    }
}