using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HideTextButton : MonoBehaviour
{
    private UnityEngine.UI.Image checkImage;
    private bool isHide;
    private PlayerState playerState;
    private GameObject[] list;
    void Start()
    {
        checkImage = GetComponent<UnityEngine.UI.Image>();
        playerState = AutoTrackPlayer.TrackPlayer().GetComponent<PlayerState>();
        isHide = playerState.IsHideWelcomeText;
        list = GameObject.FindGameObjectsWithTag("TextBlock");
        UpdateImageStatus();
        UpdateTextBlock();
    }

    void UpdateImageStatus()
    {
        Color x = checkImage.color;
        if (isHide)
        {
            checkImage.color = new Color(x.r, x.g, x.b, 1f);
        }
        else
        {
            checkImage.color = new Color(x.r, x.g, x.b, 0f);
        }
    }
    public void OnClick()
    {
        isHide = !isHide;
        playerState.IsHideWelcomeText = isHide;
        UpdateImageStatus();
        UpdateTextBlock();
    }

    void UpdateTextBlock()
    {
        foreach (GameObject go in list) 
        {
            go.SetActive(!isHide);
        }
    }
}
