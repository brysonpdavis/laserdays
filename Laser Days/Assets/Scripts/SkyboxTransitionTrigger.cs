using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxTransitionTrigger : MonoBehaviour {

    SkyboxTransition sky;
    public WorldSettings settings;


    private void Start()
    {
        sky = Toolbox.Instance.GetPlayer().GetComponent<SkyboxTransition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetSkyboxValues();
        }
    }

    void SetSkyboxValues()
    {
        sky.laserFog = settings.GetLaserFog();
        sky.laserAmbient = settings.GetLaserAmbient();
        sky.realFog = settings.GetRealFog();
        sky.realAbmient = settings.GetRealAmbient();



        float transitionProgress = sky.transitionProgress;
        Color currentFog;
        Color currentAmb;

        sky.material = settings.GetSkyBoxMaterial();

        RenderSettings.skybox = sky.material;

        if (sky.transitionDirection)
        {
            currentFog = Color.Lerp(sky.laserFog, sky.realFog, transitionProgress);
            currentAmb = Color.Lerp(sky.laserAmbient, sky.realAbmient, transitionProgress);
            sky.material.SetFloat("_TransitionState", 1f- transitionProgress);

        }

        else
        {
            currentFog = Color.Lerp(sky.realFog, sky.laserFog, transitionProgress);
            currentAmb = Color.Lerp(sky.realAbmient, sky.laserAmbient, transitionProgress); 
            sky.material.SetFloat("_TransitionState", transitionProgress);

        }

        sky.currentFog = currentFog;
        sky.currentAmbient = currentAmb;

    }



}
