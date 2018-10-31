using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicClickable : FlippableObject {

    public int maxVelocity = 8;

    private float originalVelocity = 10f;
    public AudioClip overridePop;
    //public AudioClip collide;
    private MFPP.SoundContainerAsset sounds;
    private bool wait = false;
    private float waitTime = .2f;


    public override void Pickup () 
    {
        if (raycastManager.selectedObjs.Contains(this.gameObject))
        { raycastManager.selectedObjs.Remove(this.gameObject); }

            InteractingIconHover();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false;
            rigidbody.freezeRotation = true;
            Select();
            renderer.material.SetFloat("_Shimmer", 1f);
            renderer.material.SetInt("_onHover", 1);
            rigidbody.constraints = RigidbodyConstraints.None;

            if (!beenPickedUp)
            { StartCoroutine(SlowPickup());}
    }

    public override void Drop()
    {
        StopAllCoroutines();
        wait = false;
        currentPositionVelocity = originalVelocity;

        //put the object down with the right shader
        if (player.GetComponent<flipScript>().space)
        {
            renderer.material.shader = raycastManager.realWorldShader;
            GetComponent<Transition>().SetStart(0f);
            renderer.material.SetInt("_onHover", 1);

            //renderer.material.SetInt("_onHold", 0);
            this.gameObject.layer = 11;
        }

        else
        {
            renderer.material.shader = raycastManager.laserWorldShader;
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

        FlipCore(false);
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

        ParticleSystem.Burst burst = new ParticleSystem.Burst(.025f, 100f);

        var main = particleSystem.main;
        main.startLifetime = 4f;
        //particleSystem.main.startLifetime = .5f;
        particleSystem.emission.SetBurst(0, burst);
        particleSystem.Play();

        if (audioSource)
        {
            if (overridePop) { audioSource.clip = overridePop; }
            else { audioSource.clip = player.gameObject.GetComponent<SoundBox>().pop; }

            audioSource.Play();
        }
    }

    public override bool Flippable { get { return true; } }

    private void OnCollisionEnter(Collision collision)
    {
        if (!wait && gameObject.layer == Toolbox.Instance.GetPlayer().layer-5)
        {
            audioSource.clip = Toolbox.Instance.testercubeSounds.main.GetRandomSoundClip();

            //Vector3 vel = rigidbody.velocity / 6f;
            //float value = Vector3.ClampMagnitude(vel, 1f).magnitude;
            //Debug.Log(value);
            audioSource.volume = Toolbox.Instance.soundEffectsVolume; //* value;
            audioSource.Play();
            StartCoroutine(WaitTime());
                   
        }
    }

    private IEnumerator WaitTime()
    {
        wait = true;
        yield return new WaitForSeconds(waitTime);
        wait = false;
        yield return null;
    }

}
