using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailScreen : MonoBehaviour {

    CanvasGroup canvas;
    public float duration = 2f;
    public GameObject nextScreen;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        OpeningSongSingleton.FadeThenDestroy();
    }

    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }


    private IEnumerator FadeRoutine()
    {
  
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

        Time.timeScale = 1f;
        gameObject.SetActive(false);


    }
}
