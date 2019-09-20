using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmailScreen : MonoBehaviour {

    CanvasGroup canvas;
    public float duration = 2f;
    public GameObject nextScreen;
    [SerializeField] private GameObject _continue;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        OpeningSongSingleton.FadeThenDestroy();
        EventSystem.current.SetSelectedGameObject(_continue);
        Toolbox.Instance.DisablePlayerMovementAndFlip();
    }

    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }


    private IEnumerator FadeRoutine()
    {
        Toolbox.Instance.DisablePlayerMovementAndFlip();
        
        GameObject pause = GameObject.Find("PauseMenu");
        if (pause)
        {
            Debug.Log("turning off menu");
            Toolbox.Instance.Resume(true);
        }

        nextScreen.SetActive(true);
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;

            float current = Mathf.Lerp(1f, 0f, ratio);
            canvas.alpha = current;
            yield return null;

        }
        
        yield return new WaitForSeconds(3f);
        
        Toolbox.Instance.EnablePlayerMovementAndFlip(false);
        
        gameObject.SetActive(false);
    }
}
