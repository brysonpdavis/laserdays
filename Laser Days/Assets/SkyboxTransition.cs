//Script for setting up and transitions global lighting variables
//Copyright (c) 2019 Rustforms.

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SkyboxTransition : MonoBehaviour {

    Material material;

    private IEnumerator flipTransition;
    float offset;
    float speed;
    public float duration;



    public bool transitionFog;
    public float fogMultiplier = 1f;
    public float ambientMultiplier = 1f;

    public Color laserFog;
    public Color laserAmbient;
    public Color realFog;
    public Color realAbmient;

    public Color GlobalShadow;

    public Material realGlobalParticle;
    public Material laserGlobalParticle;


    // Use this for initialization
    void Start()
    {

        material = RenderSettings.skybox;

        Shader.SetGlobalColor("_GlobalShadowColor", GlobalShadow);

        if (Toolbox.Instance.GetPlayer().layer == 16) 
        { 
            SetStart(0); 
            RenderSettings.fogColor = realFog*fogMultiplier;
            RenderSettings.ambientLight = realAbmient*ambientMultiplier;

            realGlobalParticle.SetFloat("_TransitionState", 0);
            laserGlobalParticle.SetFloat("_TransitionState", 0);
        }
        else 
        { 
            SetStart(1); 
            RenderSettings.fogColor = laserFog*fogMultiplier;
            RenderSettings.ambientLight = laserAmbient*ambientMultiplier;

            realGlobalParticle.SetFloat("_TransitionState", 1);
            laserGlobalParticle.SetFloat("_TransitionState", 1);
        }


        PostProcessLayer p = Camera.main.GetComponent<PostProcessLayer>();
        int q = QualitySettings.GetQualityLevel();
        if (q == 1)
        {

            p.antialiasingMode = PostProcessLayer.Antialiasing.None;

     
        } else 
        {
            p.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        }

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
        Color endAmbient;
        Color startFog = RenderSettings.fogColor;
        Color startAmbient = RenderSettings.ambientLight;

        if (endpoint == 0f)
        {
            endFog = realFog*fogMultiplier;
            endAmbient = realAbmient*ambientMultiplier;

        }
        else
        {
            endFog = laserFog*fogMultiplier;
            endAmbient = laserAmbient*ambientMultiplier;
        }
            


        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(start, endpoint, ratio);

            material.SetFloat("_TransitionState", value);
            realGlobalParticle.SetFloat("_TransitionState", value);
            laserGlobalParticle.SetFloat("_TransitionState", value);

            if (transitionFog)
            {
                Color lerpFog = Color.Lerp(startFog, endFog, ratio);
                Color lerpAmbient = Color.Lerp(startAmbient, endAmbient, ratio);
                RenderSettings.fogColor = lerpFog;
                RenderSettings.ambientLight = lerpAmbient;
            }   
            yield return null;
        }
    }

}
