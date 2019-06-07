using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphController : MonoBehaviour {
    public float opacity;
    public float duration = .5f;
    public float armScale = 1f;
    public bool morphRunning = false;

    public float armScalingFactor = 11;

    [Header("Internal Parts")]
    public GameObject realCollider;
    private MaterialPropertyBlock realArmPropBlock; 
    public Renderer realPreview;
    private Material realPreviewMaterial;

    public GameObject laserCollider;
    private MaterialPropertyBlock laserArmPropBlock;
    public Renderer laserPreview;
    private Material laserPreviewMaterial;
    public Transition[] childrenTransitions;

    public Renderer internalPart;
    private MaterialPropertyBlock internalPartPropBlock;

    public struct renderersAndProps
    {
        public Renderer renderer;
        public MaterialPropertyBlock propertyBlock;
    }
    private renderersAndProps[] internalParts;



    private MFPP.Modules.PickUpModule pickUp;

    // Use this for initialization
    private void Awake()
    {
        Physics.IgnoreCollision(this.gameObject.GetComponent<BoxCollider>(), realCollider.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(this.gameObject.GetComponent<BoxCollider>(), laserCollider.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(laserCollider.GetComponent<BoxCollider>(), realCollider.GetComponent<BoxCollider>());

        //renderers = GetTransitionRenderers();

        internalParts = GetInternalRenderers();
        childrenTransitions = gameObject.GetComponentsInChildren<Transition>();
        //realArmPropBlock = new MaterialPropertyBlock();
        //laserArmPropBlock = new MaterialPropertyBlock();
        //internalPartPropBlock = new MaterialPropertyBlock();

    }

    void Start () {

        pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();
        laserPreviewMaterial = laserPreview.material;
        laserPreviewMaterial.SetFloat("_Opacity", 0);

        realPreviewMaterial = realPreview.material;
        realPreviewMaterial.SetFloat("_Opacity", 0);


        laserCollider.GetComponent<Renderer>().GetPropertyBlock(laserArmPropBlock);
        realCollider.GetComponent<Renderer>().GetPropertyBlock(realArmPropBlock);

        //childrenTransitions = gameObject. GetComponentsInChildren<Transition>();


        Vector3 startScaled = new Vector3(1, armScalingFactor * armScale, 1);
        Vector3 startUnscaled = new Vector3(1, 1, 1);
        if (this.gameObject.layer == 10)
        {
            laserCollider.transform.localScale = startScaled;
            realCollider.transform.localScale = startUnscaled;
        }

        else 
        {
            laserCollider.transform.localScale = startUnscaled;
            realCollider.transform.localScale = startScaled;
        }

	}
	

    public void FlipInternalRenderers(renderersAndProps[] internals, bool dir)
    {

        float currentTransitionState = internals[0].renderer.material.GetFloat("_TransitionState");

        foreach (renderersAndProps r in internals)
        {
            ShaderUtility.ShaderWorldChange(r.renderer.material, dir);
            if (dir)
            {
                r.propertyBlock.SetFloat("_TransitionState", 1f - currentTransitionState);
            }
            else 
                r.propertyBlock.SetFloat("_TransitionState", currentTransitionState);
            r.renderer.SetPropertyBlock(r.propertyBlock);

        }
    }



    public void OnFlip (bool dir) {
            //we're going to laser
        if (dir)
        {
            this.gameObject.layer = 10;

            laserCollider.layer = 10;
            realCollider.layer = 10;


            FlipInternalRenderers(internalParts, dir);
            //start coroutine


            //if object is currently held
            if (pickUp.heldObject && pickUp.heldObject.Equals(this.gameObject))
            {
                realPreviewMaterial.SetFloat("_Opacity", opacity);
            }

            StopAllCoroutines();
            StartCoroutine(MorphCoroutine(true));

        }

        //object is going to real
        else {

            this.gameObject.layer = 11;
            laserCollider.layer = 11;
            realCollider.layer = 11;


            FlipInternalRenderers(internalParts, dir);


            //if object is currently held
            if (pickUp.heldObject && pickUp.heldObject.Equals(this.gameObject))
            {
                laserPreviewMaterial.SetFloat("_Opacity", opacity);
            }

            StopAllCoroutines();
            StartCoroutine(MorphCoroutine(false));

        }

    }

    public void OnSelection()
    {
        //this.GetComponent<Renderer>().material.shader = GetComponent<ItemProperties>().selectedShader;

        GetComponent<SelectionRenderChange>().SwitchRenderersOff();
        GetComponent<SelectionRenderChange>().OnHold();

        realPreviewMaterial.SetFloat("_Opacity", opacity);
        laserPreviewMaterial.SetFloat("_Opacity", opacity);
    }

    public void OnDeselection()
    {
        GetComponent<SelectionRenderChange>().OnDrop();
        realPreviewMaterial.SetFloat("_Opacity", 0);
        laserPreviewMaterial.SetFloat("_Opacity", 0);
    }

    public renderersAndProps[] GetInternalRenderers()
    {
        Transition[] transitions = GetComponentsInChildren<Transition>();
        renderersAndProps[] transitionInternalParts = new renderersAndProps[transitions.Length];


        for (int i = 0; i < transitions.Length; i++)
        {
            transitionInternalParts[i].renderer = transitions[i].gameObject.GetComponent<Renderer>();
            transitionInternalParts[i].propertyBlock = new MaterialPropertyBlock();

            //transitions[i]. = false;
        }


        return transitionInternalParts;
    }

    private IEnumerator MorphCoroutine(bool direction)
    {

        morphRunning = true;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        Vector3 realScale = realCollider.transform.localScale;
        Vector3 laserScale = laserCollider.transform.localScale;

        float realStart = realCollider.transform.localScale.y;
        float laserStart = laserCollider.transform.localScale.y;

        float durationScale;
        int currentFlipCount= 0;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        rigidbody.isKinematic = false;

        this.tag = "NoTouch";

        Debug.Log("morph coroutine started");

        if (direction)
        {

            durationScale = (((armScalingFactor - laserStart) / (armScalingFactor-1f))*duration);


            while (elapsedTime < durationScale)
            {

                elapsedTime += Time.smoothDeltaTime;
                realScale.y = Mathf.Lerp(realStart, 1, (elapsedTime/durationScale));
                realCollider.transform.localScale = realScale;
                laserScale.y = Mathf.Lerp(laserStart, armScalingFactor * armScale, (elapsedTime / durationScale));
                laserCollider.transform.localScale = laserScale;

                float transitionB = (elapsedTime / durationScale);
                SetMaterialProperty("_TransitionStateB", transitionB);

                currentFlipCount = CheckFlipsForTransitions(currentFlipCount);

                yield return null;
            }
            this.tag = "Clickable";

        }

        else
        {

            durationScale = (((armScalingFactor - realStart) / (armScalingFactor - 1f)) * duration);
            Debug.Log("morph coroutine in loop");


            while (elapsedTime < durationScale)
            {

                elapsedTime += Time.smoothDeltaTime;
                ratio = elapsedTime / duration;

                realScale.y = Mathf.Lerp(realStart, armScalingFactor * armScale, (elapsedTime / durationScale));
                realCollider.transform.localScale = realScale;

                laserScale.y = Mathf.Lerp(laserStart, 1, (elapsedTime / durationScale));
                laserCollider.transform.localScale = laserScale;

                float transitionB = (1f - (elapsedTime / durationScale));
                SetMaterialProperty("_TransitionStateB", transitionB);

                currentFlipCount = CheckFlipsForTransitions(currentFlipCount);


                yield return null;
            }
            this.tag = "Clickable";
        }



        if (pickUp.heldObject && pickUp.heldObject.Equals(this.gameObject))
        {
            rigidbody.isKinematic = false;
        }

        else {
            rigidbody.isKinematic = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                }



        morphRunning = false;

        yield return null;

    }

    private int CheckFlipsForTransitions(int count)
    {
        if (Toolbox.Instance.GetFlip().flippedThisFrame)
        {
            count += 1;
        }
        if (Toolbox.Instance.EqualToHeld(gameObject) || count <= 1)
            StopArmTransitions();

        return count;
    }

    private void StopArmTransitions()
    {
        foreach (Transition t in childrenTransitions)
        {
            t.StopAllCoroutines();
        }
    }

    private void SetMaterialProperty(string name, float value)
    {

        foreach (renderersAndProps rp in internalParts)
        {
            rp.propertyBlock.SetFloat(name, value);
            rp.renderer.SetPropertyBlock(rp.propertyBlock);
        }
    }

}
