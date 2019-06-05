using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphController : MonoBehaviour {
    public float opacity;
    public float duration = .5f;
    public float armScale = 1f;
    public bool morphRunning = false;

    [Header("Internal Parts")]
    public GameObject realCollider;
    private MaterialPropertyBlock realArmPropBlock; 
    public Renderer realPreview;
    private Material realPreviewMaterial;

    public GameObject laserCollider;
    private MaterialPropertyBlock laserArmPropBlock;
    public Renderer laserPreview;
    private Material laserPreviewMaterial;



    public Shader laserOnlyShader;
    public Shader realOnlyShader;



    private MFPP.Modules.PickUpModule pickUp;

    // Use this for initialization
    private void Awake()
    {
        Physics.IgnoreCollision(this.gameObject.GetComponent<BoxCollider>(), realCollider.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(this.gameObject.GetComponent<BoxCollider>(), laserCollider.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(laserCollider.GetComponent<BoxCollider>(), realCollider.GetComponent<BoxCollider>());


        realArmPropBlock = new MaterialPropertyBlock();
        laserArmPropBlock = new MaterialPropertyBlock();
    }

    void Start () {

        pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();
        laserPreviewMaterial = laserPreview.material;
        laserPreviewMaterial.SetFloat("_Opacity", 0);

        realPreviewMaterial = realPreview.material;
        realPreviewMaterial.SetFloat("_Opacity", 0);


        laserCollider.GetComponent<Renderer>().GetPropertyBlock(laserArmPropBlock);
        realCollider.GetComponent<Renderer>().GetPropertyBlock(realArmPropBlock);



        Vector3 startScaled = new Vector3(1, 3 * armScale, 1);
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
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnFlip (bool dir) {

       // Debug.Log("called");
        //we're going to laser
        if (dir)
        {
            this.gameObject.layer = 10;

            laserCollider.layer = 10;
            realCollider.layer = 10;

            Renderer realArm = realCollider.GetComponent<Renderer>();
            Renderer laserArm = laserCollider.GetComponent<Renderer>();

            // Arms Going to laser
            ShaderUtility.ShaderWorldChange(realArm.material, true);
            ShaderUtility.ShaderWorldChange(laserArm.material, true);


            laserArmPropBlock.SetFloat("_TransitionState", 1);
            laserCollider.GetComponent<Renderer>().SetPropertyBlock(laserArmPropBlock);
            //laserCollider.GetComponent<Transition>().StopAllCoroutines();



            //realArmShader.shader = laserOnlyShader;
            realArmPropBlock.SetFloat("_TransitionState", 1);
            realCollider.GetComponent<Renderer>().SetPropertyBlock(realArmPropBlock);


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


            Renderer realArm = realCollider.GetComponent<Renderer>();
            Renderer laserArm = laserCollider.GetComponent<Renderer>();

            // Arms Going to real
            ShaderUtility.ShaderWorldChange(realArm.material, false);
            ShaderUtility.ShaderWorldChange(laserArm.material, false);





            laserCollider.layer = 11;
            laserArmPropBlock.SetFloat("_TransitionState", 0);
            laserCollider.GetComponent<Renderer>().SetPropertyBlock(laserArmPropBlock);
            realCollider.layer = 11;
            realArmPropBlock.SetFloat("_TransitionState", 0);
            realCollider.GetComponent<Renderer>().SetPropertyBlock(realArmPropBlock);


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

    private IEnumerator MorphCoroutine(bool direction)
    {
        morphRunning = true;
        //Debug.Log("moving again" + this.name);
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        Vector3 realScale = realCollider.transform.localScale;
        Vector3 laserScale = laserCollider.transform.localScale;

        float realStart = realCollider.transform.localScale.y;
        float laserStart = laserCollider.transform.localScale.y;

        float durationScale;


        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        rigidbody.isKinematic = false;

        this.tag = "NoTouch";

        if (direction)
        {

            durationScale = (((3f - laserStart) / 2f)*duration);


            while (elapsedTime < durationScale)
            {

                elapsedTime += Time.smoothDeltaTime;
               // ratio = elapsedTime / duration;

                realScale.y = Mathf.Lerp(realStart, 1, (elapsedTime/durationScale));
                realCollider.transform.localScale = realScale;
                laserScale.y = Mathf.Lerp(laserStart, 3*armScale, (elapsedTime / durationScale));
                laserCollider.transform.localScale = laserScale;
              

                yield return null;
            }
            this.tag = "Clickable";

        }

        else
        {

            durationScale = (((3f - realStart) / 2f) * duration);


            while (elapsedTime < durationScale)
            {

                elapsedTime += Time.smoothDeltaTime;
                ratio = elapsedTime / duration;

                realScale.y = Mathf.Lerp(realStart, 3*armScale, (elapsedTime / durationScale));
                realCollider.transform.localScale = realScale;

                laserScale.y = Mathf.Lerp(laserStart, 1, (elapsedTime / durationScale));
                laserCollider.transform.localScale = laserScale;

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

}
