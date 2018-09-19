﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicClickable : FlippableObject {

    public int maxVelocity = 8;

    private float originalVelocity = 10f;
    public AudioClip overridePop;


    public override void Pickup () 
    {
            InteractingIconHover();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false;
            rigidbody.freezeRotation = true;
            renderer.material.SetInt("_onHold", 1);
            Select();
            rigidbody.constraints = RigidbodyConstraints.None;
            

            if (!beenPickedUp)
            { StartCoroutine(SlowPickup());}
    }

    public override void Drop()
    {
        StopAllCoroutines();

        //put the object down with the right shader
        if (player.GetComponent<flipScript>().space)
        {
            renderer.material.shader = raycastManager.realWorldShader;
            renderer.material.SetInt("_onHold", 0);
            this.gameObject.layer = 11;
            UnSelect();
            GetComponent<Transition>().SetStart(0f);
            currentPositionVelocity = originalVelocity;

        }

        else
        {
            renderer.material.shader = raycastManager.laserWorldShader;
            UnSelect();
            renderer.material.SetInt("_onHold", 0);
            this.gameObject.layer = 10;
            GetComponent<Transition>().SetStart(1f);
        }

        Vector3 currentVelocity = rigidbody.velocity;

        if (currentVelocity.magnitude > maxVelocity)
        {
            Debug.Log("before " + currentVelocity);
            rigidbody.velocity = Vector3.ClampMagnitude(currentVelocity, maxVelocity);
            Debug.Log(rigidbody.velocity + ", " + rigidbody.velocity.magnitude);
        }


        iconContainer.SetOpenHand();
        selected = false;
        rigidbody.freezeRotation = false;
        beenPickedUp = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;
        ResetWalk();

    }

    public override void SetType()
    {
        objectType = ObjectType.Clickable;
    }

    public override void DistantIconHover()
    {
        iconContainer.SetSelectHover();
    }

    public override void CloseIconHover()
    {
        iconContainer.SetOpenHand();
    }

    public override void InteractingIconHover()
    {
        iconContainer.SetHold();
    }



    private IEnumerator SlowPickup()
    {
        float duration = .5f;
        float elapsedTime = 0f;
        float ratio = elapsedTime / duration;
        originalVelocity = currentPositionVelocity;

        while (ratio < 1f)
        {

            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            float value = Mathf.Lerp(0, 1, ratio);
            currentPositionVelocity = value;
            yield return null;
        }

        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = false;
        currentPositionVelocity = originalVelocity;

        if (audioSource)
        {
            if (overridePop) { audioSource.clip = overridePop; }
            else { audioSource.clip = player.gameObject.GetComponent<SoundBox>().pop; }

            audioSource.Play();
        }
    }

    public override bool Flippable { get { return true; } }


}