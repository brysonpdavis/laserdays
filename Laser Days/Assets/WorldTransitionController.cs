using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTransitionController : MonoBehaviour {

    public Material material;
    public WorldTransition[] worldTransitions;

    private bool grav = false;
    //public Quaternion angle;

    private void Awake()
    {
        material = RenderSettings.skybox;
        worldTransitions = GetComponents<WorldTransition>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            WorldTransition(worldTransitions[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            WorldTransition(worldTransitions[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            WorldTransition(worldTransitions[2]);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            WorldTransition(worldTransitions[3]);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            WorldTransition(worldTransitions[4]);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            WorldTransition(worldTransitions[5]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (grav)
            //WorldTransition(worldTransitions[6]);
                Physics.gravity = new Vector3(1f, 15.81f, 0f);
            else
                Physics.gravity = new Vector3(1f, -9.81f, 0f);

            grav = !grav;
        }

    }

    private void WorldTransition(WorldTransition world)
    {
        //StopCoroutine("WorldTransitionRoutine");
        StopAllCoroutines();
        StartCoroutine(WorldTransitionRoutine(world));
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
            material.SetColor("_RealColor", skyboxColor);

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

}
