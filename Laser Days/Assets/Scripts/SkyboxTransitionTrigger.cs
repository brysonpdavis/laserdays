using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxTransitionTrigger : MonoBehaviour {

    SkyboxTransition sky;
    public Material newSkyboxMaterial;
    public Color laserFog;
    public Color laserAmbient;
    public Color realFog;
    public Color realAbmient;



    private void Start()
    {
        sky = Toolbox.Instance.GetPickUp().GetComponent<SkyboxTransition>();
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
        sky.laserFog = laserFog;
        sky.laserAmbient = laserAmbient;
        sky.realFog = realFog;
        sky.realAbmient = realAbmient;



        float transitionProgress = sky.transitionProgress;
        Color currentFog;
        Color currentAmb;

        sky.material = newSkyboxMaterial;
        sky.material.SetFloat("_TransitionState", transitionProgress);

        RenderSettings.skybox = newSkyboxMaterial;

        if (sky.transitionDirection)
        {
            currentFog = Color.Lerp(laserFog, realFog, transitionProgress);
            currentAmb = Color.Lerp(laserAmbient, realAbmient, transitionProgress);   
        }

        else
        {
            currentFog = Color.Lerp(realFog, laserFog, transitionProgress);
            currentAmb = Color.Lerp(realAbmient, laserAmbient, transitionProgress); 
        }

        sky.currentFog = currentFog;
        sky.currentAmbient = currentAmb;

    }



}
