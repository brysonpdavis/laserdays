﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphController : MonoBehaviour {

    public GameObject realCollider;
    private Material realArmShader;
    public Renderer realPreview;
    private Material realPreviewMaterial;

    public GameObject laserCollider;
    private Material laserArmShader;
    public Renderer laserPreview;
    private Material laserPreviewMaterial;

    public float opacity;

    public Shader laserOnlyShader;
    public Shader realOnlyShader;

    public float duration = 10f;

    private MFPP.Modules.PickUpModule pickUp;

    // Use this for initialization
    private void Awake()
    {
        Physics.IgnoreCollision(this.gameObject.GetComponent<BoxCollider>(), realCollider.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(this.gameObject.GetComponent<BoxCollider>(), laserCollider.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(laserCollider.GetComponent<BoxCollider>(), realCollider.GetComponent<BoxCollider>());

    }

    void Start () {

        pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();
        laserPreviewMaterial = laserPreview.material;
        realPreviewMaterial = realPreview.material;

        laserArmShader = laserCollider.GetComponent<Renderer>().material;
        realArmShader = realCollider.GetComponent<Renderer>().material;

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnFlip (bool dir) {

        Debug.Log("called");
        //we're going to laser
        if (dir)
        {
            this.gameObject.layer = 10;

            laserCollider.layer = 10;
            laserArmShader.shader = laserOnlyShader;
            laserArmShader.SetFloat("_TransitionState", 1);
            //laserCollider.GetComponent<Transition>().StopAllCoroutines();


            realCollider.layer = 10;
            realArmShader.shader = laserOnlyShader;
            realArmShader.SetFloat("_TransitionState", 1);

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
            laserArmShader.shader = realOnlyShader;
            laserArmShader.SetFloat("_TransitionState", 0);

            realCollider.layer = 11;
            realArmShader.shader = realOnlyShader;
            realArmShader.SetFloat("_TransitionState", 0);


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
                laserScale.y = Mathf.Lerp(laserStart, 3, (elapsedTime / durationScale));
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

                realScale.y = Mathf.Lerp(realStart, 3, (elapsedTime / durationScale));
                realCollider.transform.localScale = realScale;

                laserScale.y = Mathf.Lerp(laserStart, 1, (elapsedTime / durationScale));
                laserCollider.transform.localScale = laserScale;

                yield return null;
            }
            this.tag = "Clickable";
        }

        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        yield return null;

    }

}
