using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour {

    private IEnumerator flipTransition;
    public float duration = .4f;
    public float cameraReal = 65f;
    public float  cameraLaser = 75f;

    private void Start()
    {
        if (Toolbox.Instance.GetPlayer().layer == 16){
            Camera.main.fieldOfView = cameraReal;
        }
        else {Camera.main.fieldOfView = cameraLaser;}
    }


    public void Flip(bool end)
    {

        flipTransition = flipTransitionRoutine(Camera.main.fieldOfView, end);
            StartCoroutine(flipTransition);
    }



    private IEnumerator flipTransitionRoutine(float startpoint, bool direction)
    {

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        float endpoint;

        if (direction) { endpoint = cameraReal; }
        else { endpoint = cameraLaser; }

       // Debug.Log(startpoint + ", " + endpoint);
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, ratio);
            Camera.main.fieldOfView = value;
           // Debug.Log(value);
            yield return null;
        }
    }


}
