using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLightTransition : MonoBehaviour {

    public float rotationTime = 1f;
    private Transform directionalLight;
    public Quaternion[] directionalRotations;

	// Use this for initialization
	void Start () {
        directionalLight = GameObject.Find("Directional Light").transform;

	}
	
	// Update is called once per frame
	void Update () {
        //light rotations
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            DirectionalRotation(directionalRotations[0]);
        }

        //light rotations
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            DirectionalRotation(directionalRotations[1]);
        }

        //light rotations
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            DirectionalRotation(directionalRotations[2]);
        }

	}

    private void DirectionalRotation(Quaternion rotation)
    {
        //StopCoroutine("WorldTransitionRoutine");
        StopAllCoroutines();
        StartCoroutine(DirectionalLightRoutine(rotation));
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
