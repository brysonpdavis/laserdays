using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingZone : MonoBehaviour {

    public float duration = 0.8f;
    public float ambMultiplier = .5f;
    public float fogMultiplier = .5f;
    public float density = .1f;
    public float distance = 0f;

    public bool resetOnExit = true;

    SkyboxTransition skybox;
    UnityStandardAssets.ImageEffects.GlobalFog fog;

    private void Start()
    {
        skybox = Toolbox.Instance.GetPlayer().GetComponent<SkyboxTransition>();
        fog = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.GlobalFog>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(fogRoutine(fogMultiplier, ambMultiplier, density, distance));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && resetOnExit)
        {
            StopAllCoroutines();
            StartCoroutine(fogRoutine(1f, 1f, Toolbox.Instance.fogDensityDefault, Toolbox.Instance.cameraFogStartDefault));
        } 
    }

    private IEnumerator fogRoutine(float fogMult, float ambMult, float densityEnd, float fogDistance)
    {
        
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        //skybox.ambientMultiplier = ambMult;
        //skybox.fogMultiplier = fogMult;

        float fogMultStart = skybox.fogMultiplier;
        float ambientMultStart = skybox.ambientMultiplier;

        float densityStart = RenderSettings.fogDensity;
        float fogStartDistance = fog.startDistance;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float densityValue = Mathf.Lerp(densityStart, densityEnd, ratio);
            RenderSettings.fogDensity = densityValue;

            float distanceValue = Mathf.Lerp(fogStartDistance, fogDistance, ratio);
            fog.startDistance = distanceValue;

            float fogMultVal = Mathf.Lerp(fogMultStart, fogMult, ratio);
            skybox.fogMultiplier = fogMultVal;

            float ambientMultVal = Mathf.Lerp(ambientMultStart, ambMult, ratio);
            skybox.ambientMultiplier = ambientMultVal;

            yield return null;
        }
    }


}
