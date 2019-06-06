using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.ShaderGraph;

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
    public bool manualTransitionChildren;
    bool transitionAllChildren;
    protected Transition[] childrenTransitions;

    private bool amCore;


    protected virtual void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        mRenderer = GetComponent<Renderer>();

        if (GetComponent<LineRenderer>())
        {
            mRenderer = GetComponent<LineRenderer>();
            shared = true;
        }

        if(GetComponent<Decal>()){


        }
            

        if (!(gameObject.layer == 10 || gameObject.layer == 11 || gameObject.layer == 27) || shared)
        {
            material = mRenderer.sharedMaterial;
            sharedMaterial = true;
            shared = true;
        }


        offset = Random.value;
        speed = Random.Range(1f, 2f);
        SetupChildrenTransitions();

    }

    protected virtual void Start()
    {
        if (sharedMaterial)
        {
            if (!Toolbox.Instance.sharedMaterials.Contains(material))
                Toolbox.Instance.sharedMaterials.Add(material);
        }
    }

    private void SetupChildrenTransitions()
    {
        //set all objects to transition children to have bool setup
        //only happens on relevant objects
        if (GetComponent<PlatformMover>() || GetComponent<InteractableObject>() || manualTransitionChildren)
        {
            transitionAllChildren = true;
            childrenTransitions = GetComponentsInChildren<Transition>();
        }

        if (GetComponent<Core>())
        {
            amCore = true;
        }
    }

    private bool CoreParentHeldCheck()
    {
        if (!amCore || (amCore && !Toolbox.Instance.EqualToHeld(transform.parent.gameObject)))
            return true;
        else return false;
    }

    public virtual void Flip(float end, float duration)
    {

        //first need to make sure the object isn't already selected before starting any transition
        //objects that are selected will be flipped and shouldn't have any animation, but should change their parent gameobject

            mRenderer.GetPropertyBlock(_propBlock);
            
            float start = _propBlock.GetFloat("_TransitionState");  //material.GetFloat("_TransitionState");
            


            //start new direction from where we've left off but in the direction we've specified with "end"
            if (!Toolbox.Instance.EqualToHeld(this.gameObject) && CoreParentHeldCheck())
            {
                flipTransition = flipTransitionRoutine(start, end, duration / ScaleSpeed);
                StartCoroutine(flipTransition);

            }
            
            if (transitionAllChildren)
                foreach (Transition transition in childrenTransitions)
                {
                if (!transition.gameObject.Equals(this.gameObject) && !Toolbox.Instance.EqualToHeld(transition.gameObject))
                    {
                        transition.StopAllCoroutines();
                        transition.Flip(end, duration);

                    }
                }
    }



    //use setstart to be sure that when gameobjects are initialized they start with 
    //dissolve amount that corresponds to the world that player is in
    //useful when switching an object, immediately sets it without transition
    public virtual void SetStart (float value){

        //material.SetFloat("_TransitionState", value);
        //if (GetComponent<Sokoban1x1>())
        //Debug.Log("starting");

        //in case start has not occurred yet
        if (!mRenderer)
            Awake();
        
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
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, TweeningFunctions.EaseInOut(ratio));

            _propBlock.SetFloat("_TransitionState", value);
            mRenderer.SetPropertyBlock(_propBlock);
            //material.SetFloat("_TransitionState", value);
            //RendererExtensions.UpdateGIMaterials(mRenderer);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}