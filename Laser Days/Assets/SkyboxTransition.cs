//Script for setting up and transitions global lighting variables
//Copyright (c) 2019 Rustforms.

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public class SkyboxTransition : MonoBehaviour
{

    public Material material;

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


    public Color currentFog;
    public Color currentAmbient;

    public Material realGlobalParticle;
    public Material laserGlobalParticle;
    public float transitionProgress;
    public bool transitionDirection;

    private List<LightingMembrane> membranes;

    private void Awake()
    {
        membranes = new List<LightingMembrane>();
    }

    // Use this for initialization
    void Start()
    {

        material = RenderSettings.skybox;



        if (Toolbox.Instance.GetPlayer().layer == 16)
        {
            SetStart(0);
            currentFog = realFog;
            currentAmbient = realAbmient;
            RenderSettings.fogColor = currentFog;
            RenderSettings.ambientLight = currentAmbient;

            realGlobalParticle.SetFloat("_TransitionState", 0);
            laserGlobalParticle.SetFloat("_TransitionState", 0);
        }
        else
        {
            SetStart(1);
            currentFog = laserFog;
            currentAmbient = laserAmbient;
            RenderSettings.fogColor = currentFog;
            RenderSettings.ambientLight = currentAmbient;

            realGlobalParticle.SetFloat("_TransitionState", 1);
            laserGlobalParticle.SetFloat("_TransitionState", 1);
        }


        PostProcessLayer p = Camera.main.GetComponent<PostProcessLayer>();
        int q = QualitySettings.GetQualityLevel();
        if (q == 1)
        {

            p.antialiasingMode = PostProcessLayer.Antialiasing.None;


        }
        else
        {
            p.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        }



    }

    private void Update()
    {
        RenderSettings.fogColor = currentFog * fogMultiplier;
        RenderSettings.ambientLight = currentAmbient * ambientMultiplier;
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
        transitionProgress = elapsedTime / actualDuration;
        float start = material.GetFloat("_TransitionState");
        //int property = Shader.PropertyToID("_D7A8CF01");
        Color endFog;
        Color endAmbient;
        Color startFog = RenderSettings.fogColor / fogMultiplier;
        Color startAmbient = RenderSettings.ambientLight / ambientMultiplier;

        if (endpoint <= 0.0001f)
        {

            endFog = realFog;
            endAmbient = realAbmient;
            transitionDirection = true;

        }
        else
        {
            endFog = laserFog;
            endAmbient = laserAmbient;
            transitionDirection = false;
        }



        while (transitionProgress < 1f)
        {
            elapsedTime += Time.deltaTime;
            transitionProgress = elapsedTime / duration;
            float value = Mathf.Lerp(start, endpoint, transitionProgress);

            material.SetFloat("_TransitionState", value);
            realGlobalParticle.SetFloat("_TransitionState", value);
            laserGlobalParticle.SetFloat("_TransitionState", value);

            if (transitionFog)
            {
                Color lerpFog = Color.Lerp(startFog, endFog, transitionProgress);
                Color lerpAmbient = Color.Lerp(startAmbient, endAmbient, transitionProgress);
                currentFog = lerpFog;
                currentAmbient = lerpAmbient;
            }
            yield return null;
        }
    }

    public void AdMembrane(LightingMembrane lm)
    {
        membranes.Add(lm);
    }

    public void DeactivateMembranes()
    {
        foreach (LightingMembrane lm in membranes)
        {
            lm.active = false;
        }
    }

}
