using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabHold : ReticleObject, IHoldable
{

    public int maxVelocity = 8;

    public bool NoConstraintsOnDrop = true;

    private float originalVelocity = 10f;
    public AudioClip overridePop;
    Rigidbody rigidbody;
    Camera mainCamera;
    float currentPositionVelocity;



    public void DoPickup()
    {
        if (_action)
        {
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

        rigidbody.constraints = RigidbodyConstraints.None;

        transform.localRotation = Quaternion.Euler(Vector3.zero);

        GetComponent<CrabWalk>().OnHold();
    }

    public override void Start()
    {
        base.Start();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        mainCamera = Camera.main;
        currentPositionVelocity = 10f;

    }

    public void HoldPosition()
    {
        Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * _activateDistance; //pickUp.MaxPickupDistance;
        rigidbody.angularVelocity *= 0.5f;
        rigidbody.velocity = ((floatingPosition - rigidbody.transform.position) * (currentPositionVelocity * 1f));
    }

    public void Drop()
    {
        StopAllCoroutines();
        currentPositionVelocity = originalVelocity;
        GetComponent<CrabWalk>().OnDrop();
        
        CloseIconHover();
        //selected = false;
        //UnSelect();

        
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.freezeRotation = true;

        //beenPickedUp = true;
        rigidbody.useGravity = true;
        //ResetWalk();

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




}
