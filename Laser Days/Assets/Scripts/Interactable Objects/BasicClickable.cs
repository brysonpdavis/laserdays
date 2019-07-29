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
            InteractingIconHover();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false;
            rigidbody.freezeRotation = true;
            
        //verify if we should add shimmer effects while holding
            if (MaxFlipCheck(false))
            {
                OnSelect();
                SetMaterialFloatProp("_Shimmer", 1f);
                SetMaterialFloatProp("_onHover", 1);
            }
                
            rigidbody.constraints = RigidbodyConstraints.None;
            transform.localRotation = Quaternion.Euler(Vector3.zero);

        if (slowPickup && !beenPickedUp)
            {StartCoroutine(SlowPickup());}
    }


    private void OnCollisionExit(Collision collision)
    {
        transform.position = transform.position + new Vector3(0f, 0.00001f, 0f);
    }


    public override void Drop()
    {
        StopAllCoroutines();
        wait = false;
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
            OffSelect();
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
        _iconContainer.SetOpenHand();
        selected = false;
        OffSelect();
        rigidbody.freezeRotation = false;
        beenPickedUp = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;
        ResetWalk();

    }

    public override void DistantIconHover()
    {
        if (AbleToFlip)
            _iconContainer.SetSelectHover();
        else 
            _iconContainer.SetInteractHover();

    }

    public override void CloseIconHover()
    {
        if (AbleToFlip)
            _iconContainer.SetOpenHandFill();
        else
            _iconContainer.SetOpenHand();

    }

    public override void InteractingIconHover()
    {
        if (AbleToFlip)
            _iconContainer.SetHoldFill();
        else
            _iconContainer.SetHold();
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

        if (audio)
        {
            if (overridePop) { audio.clip = overridePop; }
            else { audio.clip = player.gameObject.GetComponent<SoundBox>().pop; }

            audio.Play();
        }
    }

    public override bool Flippable { get { return true; } }

    private void OnCollisionEnter(Collision collision)
    {
        if (!wait && gameObject.layer == Toolbox.Instance.GetPlayer().layer-5)
        {
            audio.clip = SoundBox.Instance.testercubeSounds.main.GetRandomSoundClip();


            //Vector3 vel = rigidbody.velocity / 6f;
            //float value = Vector3.ClampMagnitude(vel, 1f).magnitude;
            //Debug.Log(value);
            audio.volume = Toolbox.Instance.soundEffectsVolume; //* value;
            audio.Play();
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
