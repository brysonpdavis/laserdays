using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxTransition : MonoBehaviour {

    Material material;

    private IEnumerator flipTransition;
    float offset;
    float speed;
    public float duration;

    public bool transitionFog;
    public Color laserFog;
    public Color realFog;

    // Use this for initialization
    void Awake()
    {
        if (Toolbox.Instance.GetPlayer().layer == 16) 
        { 
            SetStart(0); 
            RenderSettings.fogColor = realFog; 
        }
        else 
        { 
            SetStart(1); 
            RenderSettings.fogColor = laserFog;
        }
        material = RenderSettings.skybox;
    }

    public void Flip(bool direction)
    {
        if (direction)
        {
            flipTransition = flipTransitionRoutine(0f);
            //going to RenderSettings.fogColor = realFog;
        }

        else
        {
            flipTransition = flipTransitionRoutine(1f);
            //going to RenderSettings.fogColor = laserFog;
        }

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
        Color endFog;
        Color startFog = RenderSettings.fogColor;

        if (endpoint == 0f)
            endFog = realFog;
        else
            endFog = laserFog;


        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(start, endpoint, ratio);
            material.SetFloat("_TransitionState", value);
            if (transitionFog)
            {
                Color lerpFog = Color.Lerp(startFog, endFog, ratio);
                RenderSettings.fogColor = lerpFog;
            }   
            yield return null;
        }
    }

}
