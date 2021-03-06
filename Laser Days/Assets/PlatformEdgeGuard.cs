﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEdgeGuard : MonoBehaviour {

    public bool active = false;
    public PlatformEdgeGuard secondaryTrigger;
    public GameObject associatedCollider;
    public Renderer associatedRenderer;
    public bool isMoveable;
    public GameObject parent;
    public GameObject player;

    public AudioSource audioSource;
    public AudioClip clickInClip;
    public AudioClip clickOffClip;



    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
        GameObject check = this.transform.parent.transform.parent.transform.parent.transform.gameObject;
        //Debug.Log(check.name);

        //checks to see if is part of a sokoban or not
        if (check.GetComponent<HoldableObject>() && 
            (check.GetComponent<Sokoban1x1>() ||
             check.GetComponent<Sokoban2x2>()))
        {
            parent = check;
            isMoveable = true;

            //sets up for sound effects for sokoban when they click on and off to guards
            clickInClip = SoundBox.Instance.sokobanClickOn;
            clickOffClip = SoundBox.Instance.sokobanClickOff;
        }


        audioSource = gameObject.AddComponent<AudioSource>();//GetComponentInParent<AudioSource>();
        //associatedCollider = GetComponentInChildren<BoxCollider>().gameObject;
        associatedRenderer.material.SetFloat("_Open", 0f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Guard") && (associatedCollider.gameObject.layer == other.GetComponent<PlatformEdgeGuard>().associatedCollider.gameObject.layer))
        {

            //if the other object isn't the sokoban
            if (!other.GetComponent<PlatformEdgeGuard>().parent )
            {
                //&& other.gameObject.layer == this.gameObject.layer
                //Debug.Log("option 1");
                ClickIn(other);
            }

            //if this object isn't a sokoban
            else if (!parent){
                //Debug.Log("option 2");
                ClickIn(other);
            }

            //if the other object is on the same layer as this one
            else if (other.GetComponent<PlatformEdgeGuard>().parent.layer == parent.layer)
           {
                //Debug.Log("option 3");
                ClickIn(other);
            }
       }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Guard")){

            active = false;
            associatedCollider.SetActive(true);
            //associatedRenderer.enabled = true;
            StopAllCoroutines();
            StartCoroutine(EdgeRender(true));


            if (secondaryTrigger.active && clickOffClip && (this.gameObject.layer + 5 == player.layer)){
                Toolbox.Instance.SetVolume(audioSource);
                audioSource.clip = clickOffClip;
                audioSource.Play();
            }


        }
    }

    private void ClickIn (Collider other)
    {
        active = true;
        if (active && secondaryTrigger.active)
        {
            associatedCollider.SetActive(false);
            //associatedRenderer.enabled = false;
            StopAllCoroutines();
            StartCoroutine(EdgeRender(false));


            //playing sound effect

            if (clickInClip && (this.gameObject.layer+5 == player.layer)){
                Toolbox.Instance.SetVolume(audioSource);
                audioSource.clip = clickInClip;
                audioSource.Play();  
            }

            if (isMoveable && 
                (player.GetComponent<MFPP.Modules.PickUpModule>().heldObject) &&
                 player.GetComponent<MFPP.Modules.PickUpModule>().heldObject.Equals(parent))
            {
                player.GetComponent<MFPP.Modules.PickUpModule>().PutDown();

            }

        }
    }


    private IEnumerator EdgeRender(bool on)
    {
        float duration = .2f;
        float scaledDuration;
        Material material = associatedRenderer.material;
        float startpoint = material.GetFloat("_Open");
        float endpoint;

        //will be visible
        if (on)
            endpoint = 0f;
        
        //will be invisible
        else
            endpoint = 1f;
       
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        //int property = Shader.PropertyToID("_D7A8CF01");
      
        while (ratio< 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, ratio);
            material.SetFloat("_Open", value);
            RendererExtensions.UpdateGIMaterials(associatedRenderer);

            yield return null;
        }
    }

}
