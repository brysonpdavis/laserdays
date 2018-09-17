using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morph : InteractableObject {

    public override void Pickup()
    {
        InteractingIconHover();
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        rigidbody.isKinematic = false;

        GetComponent<SelectionRenderChange>().SwitchRenderersOff();
        GetComponent<SelectionRenderChange>().OnHold();
        GetComponent<MorphController>().OnSelection();


        player.Movement.Speed = 1;
        player.GetComponent<MFPP.Modules.BobModule>().BobSpeed = 1.5f;


        //GetComponent<Player>().Footstep.Asset = dragFootstep;
    }

    public override void Drop()
    {
        if (player.GetComponent<flipScript>().space)
        {
            GetComponent<Renderer>().material.shader = raycastManager.realWorldShader;
            this.gameObject.layer = 11;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(0f);
            GetComponent<MorphController>().OnDeselection();
        }

        else
        {
            GetComponent<Renderer>().material.shader = raycastManager.laserWorldShader;
            this.gameObject.layer = 10;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(1f);
            GetComponent<MorphController>().OnDeselection();

        }

        iconContainer.SetOpenHand();
        itemProperties.selected = false;
        rigidbody.freezeRotation = false;
        rigidbody.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.isKinematic = true;
        ResetWalk();
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
        iconContainer.SetDrag();
    }
}
