﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxTransition : MonoBehaviour {

    Material material;

    private IEnumerator flipTransition;
    float offset;
    float speed;
    public float duration;


    // Use this for initialization
    void Awake()
    {
        material = RenderSettings.skybox;
    }

    public void Flip(bool direction)
    {
        if (direction)
        {
            flipTransition = flipTransitionRoutine(0f);
        }

        else
            flipTransition = flipTransitionRoutine(1f);

        StopAllCoroutines();
        StartCoroutine(flipTransition);

    }

    //use setstart to be sure that when gameobjects are initialized they start with 
    //dissolve amount that corresponds to the world that player is in
    //useful when switching an object, immediately sets it without transition
    public void SetStart(float value)
    {

        if (material)
        {
            material.SetFloat("_TransitionState", value);
        }
    }



    private IEnumerator flipTransitionRoutine(float endpoint)
    {

        float elapsedTime = 0;
        float actualDuration = duration * (1 - (material.GetFloat("_TransitionState") / endpoint));
        float ratio = elapsedTime / actualDuration;
        float start = material.GetFloat("_TransitionState");
        //int property = Shader.PropertyToID("_D7A8CF01");

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(start, endpoint, ratio);
            material.SetFloat("_TransitionState", value);
                
            yield return null;
        }
    }

}
