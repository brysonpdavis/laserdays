using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTransitionController : MonoBehaviour {

    public Material material;
    public WorldTransition[] worldTransitions;
    public Quaternion[] directionalRotations;
    public float rotationTime = 1f;
    private Transform directionalLight;
    //public Quaternion angle;

    private void Awake()
    {
        material = RenderSettings.skybox;
        worldTransitions = GetComponents<WorldTransition>();
        directionalLight = GameObject.Find("Directional Light").transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            WorldTransition(worldTransitions[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WorldTransition(worldTransitions[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WorldTransition(worldTransitions[2]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WorldTransition(worldTransitions[3]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            WorldTransition(worldTransitions[4]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            WorldTransition(worldTransitions[5]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            WorldTransition(worldTransitions[6]);
        }


        //light rotations
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            DirectionalRotation(directionalRotations[0]);
        }

        //light rotations
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            DirectionalRotation(directionalRotations[1]);
        }

        //light rotations
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DirectionalRotation(directionalRotations[2]);
        }


    }

    private void WorldTransition(WorldTransition world)
    {
        StopCoroutine("WorldTransitionRoutine");
        //StopAllCoroutines();
        StartCoroutine(WorldTransitionRoutine(world));
    }

    private void DirectionalRotation(Quaternion rotation)
    {
        //StopCoroutine("WorldTransitionRoutine");
        StopAllCoroutines();
        StartCoroutine(DirectionalLightRoutine(rotation));
    }


    private IEnumerator WorldTransitionRoutine(WorldTransition worldTransition)
    {

        float elapsedTime = 0;
        float actualDuration = worldTransition.duration;
        float ratio = elapsedTime / worldTransition.duration;
        Debug.Log(ratio);

        float startDensity = RenderSettings.fogDensity;
        Color startFog = RenderSettings.fogColor;
        Color startSkybox = material.GetColor("_LaserColor");
        Color startAmbient = RenderSettings.ambientLight;


        while (ratio < 1f)
        {
            Debug.Log(ratio);
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / worldTransition.duration;

            //lerp skybox
            Color skyboxColor = Color.Lerp(startSkybox, worldTransition.skyboxColor, ratio);
            material.SetColor("_LaserColor", skyboxColor);

            //lerp ambient
            Color ambient = Color.Lerp(startAmbient, worldTransition.ambientEnvironment, ratio);
            RenderSettings.ambientLight = ambient;

            //lerp fog
            Color lerpFog = Color.Lerp(startFog, worldTransition.fogColor, ratio);
                RenderSettings.fogColor = lerpFog;

            //lerp fog density
            float currentDensity = Mathf.Lerp(startDensity, worldTransition.fogDensity, ratio);
            RenderSettings.fogDensity = currentDensity;

            yield return null;
        }
    }

    private IEnumerator DirectionalLightRoutine(Quaternion finalRotation)
    {

        float elapsedTime = 0;
        float ratio = elapsedTime / rotationTime;
        Debug.Log(ratio);

        Quaternion startRotation = directionalLight.rotation;


        while (ratio < 1f)
        {
            Debug.Log(ratio);
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / rotationTime;

            Quaternion current = Quaternion.Lerp(startRotation, finalRotation, ratio);
            directionalLight.rotation = current;
            yield return null;
        }
    }

}
