using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleWorldClickable : InteractableObject {

    public int maxVelocity = 8;

    private float originalVelocity = 10f;
    public AudioClip overridePop;

    public override void Pickup()
    {
        if (raycastManager.selectedObjs.Contains(this.gameObject))
        { raycastManager.selectedObjs.Remove(this.gameObject); }

        InteractingIconHover();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        rigidbody.freezeRotation = true;
        Select();
        renderer.material.SetFloat("_Shimmer", 0f);
        renderer.material.SetInt("_onHover", 1);
        rigidbody.constraints = RigidbodyConstraints.None;

        transform.localRotation = Quaternion.Euler(Vector3.zero);

        if (!beenPickedUp)
        { StartCoroutine(SlowPickup()); }
    }

    public override void Drop()
    {
        StopAllCoroutines();
        currentPositionVelocity = originalVelocity;

        //put the object down with the right shader
        if (player.GetComponent<flipScript>().space)
        {
            ShaderUtility.ShaderToReal(renderer.material);
            GetComponent<Transition>().SetStart(0f);
            renderer.material.SetInt("_onHover", 1);

            //renderer.material.SetInt("_onHold", 0);
            this.gameObject.layer = 11;
        }

        else
        {
            ShaderUtility.ShaderToLaser(renderer.material);
            GetComponent<Transition>().SetStart(1f);
            UnSelect();
            renderer.material.SetInt("_onHover", 1);

            //renderer.material.SetInt("_onHold", 0);
            this.gameObject.layer = 10;
        }

        Vector3 currentVelocity = rigidbody.velocity;

        if (currentVelocity.magnitude > maxVelocity)
        {
            //.Log("before " + currentVelocity);
            rigidbody.velocity = Vector3.ClampMagnitude(currentVelocity, maxVelocity);
            //Debug.Log(rigidbody.velocity + ", " + rigidbody.velocity.magnitude);
        }

       
        iconContainer.SetOpenHand();
        selected = false;
        UnSelect();
        rigidbody.freezeRotation = false;
        beenPickedUp = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;
        ResetWalk();

    }

    public override void SetType()
    {
        objectType = ObjectType.SingleWorldClickable;
    }

    public override void DistantIconHover()
    {
        iconContainer.SetInteractHover();
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
        
        ParticleSystem.Burst burst = new ParticleSystem.Burst(.025f, 100f);

        var main = particleSystem.main;
        main.startLifetime = 4f;
        particleSystem.emission.SetBurst(0, burst);
        particleSystem.Play();

        if (audioSource)
        {
            if (overridePop) { audioSource.clip = overridePop; }
            else { audioSource.clip = player.gameObject.GetComponent<SoundBox>().pop; }

            audioSource.Play();
        }
        
    }

    public override bool Flippable { get { return false; } }



}
