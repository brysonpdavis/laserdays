using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleWorldClickable : ReticleObject, IHoldable {

    public int maxVelocity = 8;

    public bool NoConstraintsOnDrop = true;

    private float originalVelocity = 10f;
    public AudioClip overridePop;
    Rigidbody rigidbody;
    bool beenPickedUp;
    float currentPositionVelocity;
    GameObject player;
    AudioSource audio;
    Camera mainCamera;

    public override void Start()

    {
        base.Start();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        player = Toolbox.Instance.GetPlayer();
        audio = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        currentPositionVelocity = 10f;

    }

    public void DoPickup() {
        if (_action) {
            _action.PickedUp();
        }

        Pickup();
    }

    public void Pickup()
    {
        InteractingIconHover();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        rigidbody.freezeRotation = true;
        SetMaterialFloatProp("_Shimmer", 0f);
        SetMaterialFloatProp("_onHover", 1);
        rigidbody.constraints = RigidbodyConstraints.None;

        transform.localRotation = Quaternion.Euler(Vector3.zero);

        //if (!beenPickedUp)
        //{ 
        //    //StartCoroutine(SlowPickup()); 
        //}
    }

    public void Drop()
    {
        StopAllCoroutines();
        currentPositionVelocity = originalVelocity;

        //put the object down with the right shader
        if (player.GetComponent<flipScript>().space)
        {
            ShaderUtility.ShaderToReal(_material);
            GetComponent<Transition>().SetStart(0f);
            SetMaterialFloatProp("_onHover", 1f);

            //renderer.material.SetInt("_onHold", 0);
            this.gameObject.layer = 11;
        }

        else
        {
            ShaderUtility.ShaderToLaser(_renderer.material);
            GetComponent<Transition>().SetStart(1f);
            //OffSelect();
            SetMaterialFloatProp("_onHover", 1f);

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

       
        _iconContainer.SetOpenHand();
        //selected = false;
        //OffSelect();

        rigidbody.freezeRotation = false;
        rigidbody.constraints = RigidbodyConstraints.None;
    
        beenPickedUp = true;
        rigidbody.useGravity = true;
        //ResetWalk();

    }

    public void HoldPosition()
    {
        Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * _activateDistance; //pickUp.MaxPickupDistance;
        rigidbody.angularVelocity *= 0.5f;
        rigidbody.velocity = ((floatingPosition - rigidbody.transform.position) * (currentPositionVelocity * 1f));
    }


    public override void DistantIconHover()
    {
        _iconContainer.SetInteractHover();
    }

    public override void CloseIconHover()
    {
        _iconContainer.SetOpenHand();
    }

    public void InteractingIconHover()
    {
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
        
        //ParticleSystem.Burst burst = new ParticleSystem.Burst(.025f, 100f);

        //var main = particleSystem.main;
        //main.startLifetime = 4f;
        //particleSystem.emission.SetBurst(0, burst);
        //particleSystem.Play();

        if (audio)
        {
            if (overridePop) { audio.clip = overridePop; }
            else { audio.clip = player.gameObject.GetComponent<SoundBox>().pop; }

            audio.Play();
        }
    }


}
