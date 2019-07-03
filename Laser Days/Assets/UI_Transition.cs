using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Transition : MonoBehaviour {

    private Material mat;
    private Image image;
    private Mask mask;

    private IEnumerator flipTransition;

    public float ScaleSpeed = 1f;



    // Use this for initialization
    void Start () {
        image = GetComponent<Image>();
        mat = image.material;

        mask = GetComponent<Mask>();
        mask.enabled = true;
        mask.showMaskGraphic = true;
	}
	
	// Flip
	public void Flip (float end, float duration) {

        float start = mat.GetFloat("_TransitionState");

        flipTransition = flipTransitionRoutine(start, end, duration / ScaleSpeed);
        StartCoroutine(flipTransition);

    }


    private IEnumerator flipTransitionRoutine(float startpoint, float endpoint, float duration)
    {

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
      

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, TweeningFunctions.EaseInOut(ratio));

            mask.enabled = false;
            mat.SetFloat("_TransitionState", value);
            mask.enabled = true;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}



