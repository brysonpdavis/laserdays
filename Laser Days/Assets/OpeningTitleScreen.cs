﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningTitleScreen : MonoBehaviour {

    CanvasGroup canvas;
    public float waitTime = 0f;
    public float textWaitTime = 0f;
    public float duration = 2f;

    public GameObject texto;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
        //GetComponentInParent<CanvasScaler>().enabled = true;
        //Time.timeScale = 0f;

    }

    private void Start()
    {
        StartCoroutine(FadeOnAwake());
    }

    public void FullSave()
    {
        Toolbox.Instance.FullSave();
    }

    public void FullReset()
    {
        Toolbox.Instance.FullReset();
        Time.timeScale = 1f;



        StartCoroutine(FadeOnAwake());

    }

    public void LoadFromSave()
    {
        Toolbox.Instance.LoadFromSave();
        Time.timeScale = 1f;

        StartCoroutine(FadeOnAwake());
    }

    private IEnumerator FadeOnAwake()
    {
        yield return new WaitForSeconds(textWaitTime);
        texto.SetActive(true);
        GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(waitTime);
     
        GameObject pause = GameObject.Find("PauseMenu");
        if (pause)
        {
            Debug.Log("turning off menu");
            Toolbox.Instance.Resume(true);
        }
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio <1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;

            float current = Mathf.Lerp(1f, 0f, ratio);
            canvas.alpha = current;
            yield return null;

        }


        Time.timeScale = 1f;
        

        // Invoke("Deactivate", 0.5f);
    }

    void Deactivate()
    {
        Debug.LogError("done with fadeonawake");
        gameObject.SetActive(false);
    }

}
