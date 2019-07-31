using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morph : FlippableObject {


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
        Renderer r = GetComponent<Renderer>();

        if (player.GetComponent<flipScript>().space)
        {
            //Going to real
            ShaderUtility.ShaderWorldChange(r.material, false);

            this.gameObject.layer = 11;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(0f);
            GetComponent<MorphController>().OnDeselection();
        }

        else
        {
            //Going to laser
            ShaderUtility.ShaderWorldChange(r.material, true);

            //GetComponent<Renderer>().material.shader = raycastManager.laserWorldShader;
            this.gameObject.layer = 10;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(1f);
            GetComponent<MorphController>().OnDeselection();

        }

        _iconContainer.SetOpenHand();
        selected = false;
        rigidbody.freezeRotation = false;
        rigidbody.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.isKinematic = true;
        ResetWalk();
    }

    public override void DistantIconHover()
    {
        _iconContainer.SetSelectHover();
    }

    public override void CloseIconHover()
    {
        _iconContainer.SetOpenHandFill();
    }

    public override void InteractingIconHover()
    {
        _iconContainer.SetDragFill();
    }

    public override bool Flippable { get { return true; } }

}
