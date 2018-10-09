using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSliderZ : InteractableObject {
    
    [SerializeField]
    private bool secondaryLock;

    public override void Pickup()
    {
        InteractingIconHover();
        rigidbody.constraints = RigidbodyConstraints.None;
        if (!secondaryLock) { rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY; }
        else { rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY; }
        GetComponent<SelectionRenderChange>().SwitchRenderersOff();
        GetComponent<SelectionRenderChange>().OnHold();
        player.Footstep.Asset = pickUp.dragFootstep;
        rigidbody.isKinematic = false;

        player.Movement.Speed = 1;
        player.Movement.Jump.Power = 0;
        player.GetComponent<MFPP.Modules.BobModule>().BobSpeed = 1.5f;

    }

    public override void Drop()
    {
        if (player.GetComponent<flipScript>().space)
        {
            GetComponent<Renderer>().material.shader = raycastManager.realWorldShader;
            this.gameObject.layer = 11;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(0f);
        }

        else
        {
            GetComponent<Renderer>().material.shader = raycastManager.laserWorldShader;
            this.gameObject.layer = 10;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(1f);
        }

        iconContainer.SetOpenHand();
        selected = false;
        rigidbody.freezeRotation = false;
        rigidbody.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.isKinematic = true;
        ResetWalk();
    }

    public override void SetType()
    {
        objectType = ObjectType.Clickable;
    }

    public override void DistantIconHover()
    {
        CheckBouncer();
    }

    public override void CloseIconHover()
    {
        iconContainer.SetOpenHand();
    }

    public override void InteractingIconHover()
    {
        iconContainer.SetDrag();
    }

}
