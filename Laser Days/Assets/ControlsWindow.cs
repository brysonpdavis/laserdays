using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Permissions;

public class ControlsWindow : MonoBehaviour {

    bool isShowing;
    CanvasGroup canvasGroup;
    public GameObject keyboardControls;
    public GameObject ps4Controls;

    // Use this for initialization
	void Start () {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

    }
	
    public void ShowHideControls()
    {
        if(isShowing)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            isShowing = false;
        } else
        {
            SetUpControls(UsingPS4());
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            isShowing = true;
        }
    }

    public void HideControls()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        isShowing = false;
    }

    bool UsingPS4()
    {
        return (ControlManager.GetControllerState() == ControlManager.ControllerState.JoystickPS4);
    }

    void SetUpControls(bool ps4)
    {
        if(ps4)
        {
            ps4Controls.SetActive(true);
            keyboardControls.SetActive(false);
        } else
        {
            ps4Controls.SetActive(false);
            keyboardControls.SetActive(true);
        }
    }

}
