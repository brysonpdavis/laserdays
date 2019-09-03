﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ExitMenu : MonoBehaviour {

	public string homeScene;
    public float fadeDuration = 1f;
    public Image background;
    public GameObject container;
    CanvasGroup canvasGroup;
    public static ExitMenu Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;    
        
        gameObject.SetActive(false);
    }
    public void ReturnHome()
	{
        Debug.Log("attempting");
        StartCoroutine(ReturnHomeRoutine());
	}

    public void Activate()
    {
        Instance.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        Toolbox.Instance.DisablePlayerMovementAndFlip();

        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }


    IEnumerator FadeIn()
    {
        var edgeDetection = Toolbox.Instance.playerCam.GetComponent<EdgeDetection>();

        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        Color fader = background.color;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            edgeDetection.PauseMenu = ratio;
            yield return null;
        }
        
        yield return new WaitForSeconds(2);
        
        elapsedTime = 0;
        ratio = elapsedTime / fadeDuration;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            canvasGroup.alpha = ratio;
            yield return null;
        }
        
        container.SetActive(true);
        EventSystem.current.SetSelectedGameObject(container.transform.GetChild(0).gameObject);
    }


    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        Color fader = background.color;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;
            Color newColor = Color.Lerp(fader, Color.black, ratio);
            background.color = newColor;
            yield return null;
        }

        //listener.SetActive(true);
        //listener.GetComponent<TemporaryListener>().StartWaitingForPlayer();
    }


	IEnumerator ReturnHomeRoutine()
    {
        
        //container.SetActive(false);

        //StartCoroutine(FadeOut());
        //yield return new WaitForSeconds(fadeDuration);

        //Destroy(Toolbox.Instance.player);

        Debug.Log("loading scene");

        AsyncOperation _async = new AsyncOperation();
        _async = SceneManager.LoadSceneAsync(homeScene, LoadSceneMode.Single);
        _async.allowSceneActivation = true;
        while (!_async.isDone)
        {
            yield return null;
        }
        Debug.Log("done going home");
        Toolbox.Instance.UpdateToolbox();
    }



}