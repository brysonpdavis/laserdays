using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class Transition : MonoBehaviour
{
    Renderer mRenderer;
    Material material;
    public float ScaleSpeed = 1f;
    public bool laserShared = false;

    private IEnumerator flipTransition;
    float offset;
    float speed;

    private void Awake()
    {
        mRenderer = GetComponent<Renderer>();
        material = mRenderer.material;
        offset = Random.value;
        speed = Random.Range(1f, 2f);
       

        material.SetColor("_ColorPass", Random.ColorHSV());
    }


    public void Flip(float end, float duration)
    {

        //first need to make sure the object isn't already selected before starting any transition
        //objects that are selected will be flipped and shouldn't have any animation, but should change their parent gameobject

        if (material)
        {
            float start = material.GetFloat("_TransitionState");


            //start new direction from where we've left off but in the direction we've specified with "end"
            flipTransition = flipTransitionRoutine(start, end, duration / ScaleSpeed);
            StartCoroutine(flipTransition);
                    
        }
    }



    //use setstart to be sure that when gameobjects are initialized they start with 
    //dissolve amount that corresponds to the world that player is in
    //useful when switching an object, immediately sets it without transition
    public void SetStart (float value){

        if (material)
        {
            material.SetFloat("_TransitionState", value);
        }
    }



    private IEnumerator flipTransitionRoutine(float startpoint, float endpoint, float duration)
    {
        
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        //int property = Shader.PropertyToID("_D7A8CF01");

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, ratio);

            material.SetFloat("_TransitionState", value);
            RendererExtensions.UpdateGIMaterials(mRenderer);

            yield return null;
        }

    }
}