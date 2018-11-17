using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class Transition : MonoBehaviour
{
    Renderer mRenderer;
    private MaterialPropertyBlock _propBlock;
    Material material;
    public float ScaleSpeed = 1f;
    private bool sharedMaterial = false;
    //public IList<Material> sharedmaterials;
    private IEnumerator flipTransition;
    float offset;
    float speed;
    public bool shared;

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        mRenderer = GetComponent<Renderer>();
        if (GetComponent<LineRenderer>())
            mRenderer = GetComponent<LineRenderer>();

       
//        if ((gameObject.layer == 10 || gameObject.layer == 11)  && (GetComponent<InteractableObject>() || GetComponent<Core>()))
        //if ((gameObject.layer == 10 || gameObject.layer == 11) && !shared)
        //    material = mRenderer.material;
        //else 
        if (!(gameObject.layer == 10 || gameObject.layer == 11) || shared)
        {
            material = mRenderer.sharedMaterial;
            sharedMaterial = true;
            shared = true;
        }


        offset = Random.value;
        speed = Random.Range(1f, 2f);
    }

    private void Start()
    {
        if (sharedMaterial)
        {
            if (!Toolbox.Instance.sharedMaterials.Contains(material))
                Toolbox.Instance.sharedMaterials.Add(material);
        }
    }



    public void Flip(float end, float duration)
    {

        //first need to make sure the object isn't already selected before starting any transition
        //objects that are selected will be flipped and shouldn't have any animation, but should change their parent gameobject

            mRenderer.GetPropertyBlock(_propBlock);
            
            float start = _propBlock.GetFloat("_TransitionState");  //material.GetFloat("_TransitionState");


            //start new direction from where we've left off but in the direction we've specified with "end"
            flipTransition = flipTransitionRoutine(start, end, duration / ScaleSpeed);
            StartCoroutine(flipTransition);
    }



    //use setstart to be sure that when gameobjects are initialized they start with 
    //dissolve amount that corresponds to the world that player is in
    //useful when switching an object, immediately sets it without transition
    public void SetStart (float value){

            //material.SetFloat("_TransitionState", value);
            _propBlock.SetFloat("_TransitionState",value);
            mRenderer.SetPropertyBlock(_propBlock);
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

            _propBlock.SetFloat("_TransitionState", value);
            mRenderer.SetPropertyBlock(_propBlock);
            //material.SetFloat("_TransitionState", value);
            RendererExtensions.UpdateGIMaterials(mRenderer);

            yield return null;
        }
    }
}