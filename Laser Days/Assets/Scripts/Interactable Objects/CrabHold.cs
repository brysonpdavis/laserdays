using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabHold : HoldableObject
{

    public int maxVelocity = 8;

    public bool NoConstraintsOnDrop = true;

    private float originalVelocity = 10f;
    public AudioClip overridePop;

    public override void Pickup()
    {
        InteractingIconHover();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;
        rigidbody.freezeRotation = true;

        rigidbody.constraints = RigidbodyConstraints.None;

        transform.localRotation = Quaternion.Euler(Vector3.zero);

        //if (!beenPickedUp)
        //{ 
        //    //StartCoroutine(SlowPickup()); 
        //}

        GetComponent<CrabWalk>().OnHold();
    }

    public override void Start()
    {
        base.Start();
        GetComponent<Rigidbody>().isKinematic = false;

    }

    public override void Drop()
    {
        StopAllCoroutines();
        currentPositionVelocity = originalVelocity;
        GetComponent<CrabWalk>().OnDrop();
        
        CloseIconHover();
        selected = false;
        //UnSelect();

        rigidbody.freezeRotation = false;
        rigidbody.constraints = RigidbodyConstraints.None;

        beenPickedUp = true;
        rigidbody.useGravity = true;
        ResetWalk();

    }

    public override void DistantIconHover()
    {
        _iconContainer.SetInteractHover();
    }

    public override void CloseIconHover()
    {
        _iconContainer.SetOpenHand();
    }

    public override void InteractingIconHover()
    {
        _iconContainer.SetHold();
    }

    public override void OnSelect()
    {
        return;
    }
    
    public override bool Flippable { get { return false; } }



}
