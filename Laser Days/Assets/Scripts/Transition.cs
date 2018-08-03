using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    Renderer mRenderer;
    Material material;
    public float ScaleSpeed = 1f;

    private IEnumerator flipTransition;
    private IEnumerator morphTransition;

    float offset;
    float speed;

    private void Awake()
    {
        mRenderer = GetComponent<Renderer>();

        material = mRenderer.material;

        offset = Random.value;
        speed = Random.Range(1f, 2f);


    }


    public void Flip(float end, float duration)
    {

        //first need to make sure the object isn't already selected before starting any transition
        //objects that are selected will be flipped and shouldn't have any animation, but should change their parent gameobject

        if (material){
            float start = material.GetFloat("_TransitionState");

            if (start > 0f && start < 1f)
            {
                //means that there's a transition animation already going. we need to be sure to stop it before moving on
                Debug.Log("I'm stopping one that's running!" + start + this.gameObject.name);
                StopCoroutine(flipTransition);
            }

        //start new direction from where we've left off but in the direction we've specified with "end"
        flipTransition = flipTransitionRoutine(start, end, duration / ScaleSpeed);
        StartCoroutine(flipTransition);
    }
}



    public void Morph(float end, float duration)
    {

        //first need to make sure the object isn't already selected before starting any transition
        //objects that are selected will be flipped and shouldn't have any animation, but should change their parent gameobject


        if (material)
        {
            float start = material.GetFloat("_TransitionStateB");
           

            if (start > 0f && start < 1f)
            {
                //means that there's a transition animation already going. we need to be sure to stop it before moving on
                StopAllCoroutines();
            }

            //start new direction from where we've left off but in the direction we've specified with "end"
            morphTransition = morphTransitionRoutine(start, end, duration / ScaleSpeed);
            StartCoroutine(morphTransition);
        }
    }


    //use setstart to be sure that when gameobjects are initialized they start with dissolve amount that corresponds to the world that player is in
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



    private IEnumerator morphTransitionRoutine(float startpoint, float endpoint, float duration)
    {
        int layerValue = this.gameObject.layer;
        Debug.Log("layervalue, " + this.gameObject.layer);

        if (layerValue == 10){
            this.gameObject.layer = 21;
        }
        if (layerValue == 11){
            this.gameObject.layer = 22;
        }

        bool stillSelected = false;
        if (Input.GetMouseButton(1))
        {
            
            //important for not turning this gameobject off, as it needs to act as preview since new parent object will still be selected now!
            stillSelected = true;
        }
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, ratio);

            material.SetFloat("_TransitionStateB", value);
            RendererExtensions.UpdateGIMaterials(mRenderer);

            yield return null;
        }

        if (GetComponent<Morph>().turnOff){

            if (!stillSelected)
            {
                this.gameObject.SetActive(false); 
            }
            //COULD TRANSITION HERE INSTEAD! [OTHER TURN OFF MOMENT THAT HAPPENS IS IN MORPH-PUTDOWN, FOR ASSOCIATED OBJ]
            GetComponent<Morph>().turnOff = false;
        }

        this.gameObject.layer = layerValue;

    }
}