﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sokoban1x1 : InteractableObject {

    public override void Pickup()
    {
        pickupDistance = 2.3f;
        InteractingIconHover();
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        GetComponent<SelectionRenderChange>().SwitchRenderersOff();
        GetComponent<SelectionRenderChange>().OnHold();
        player.Footstep.Asset = pickUp.dragFootstep;

        player.Movement.Speed = 1;
        player.GetComponent<MFPP.Modules.BobModule>().BobSpeed = 1.5f;
        selected = true;
        latestPosition = transform.position;

        audio.clip = SoundBox.Instance.sokobanDrag;
        audio.volume = Toolbox.Instance.soundEffectsVolume;
        audio.loop = true;
        audio.Play();

    }

    public override void Drop()
    {

        if (player.GetComponent<flipScript>().space)
        {
            //renderer.material.shader = raycastManager.realWorldShader;
            this.gameObject.layer = 11;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(0f);
        }

        else
        {
            //renderer.material.shader = raycastManager.laserWorldShader;
            this.gameObject.layer = 10;
            GetComponent<SelectionRenderChange>().OnDrop();
            GetComponent<Transition>().SetStart(1f);
        }

        iconContainer.SetOpenHand();
        selected = false;
        rigidbody.freezeRotation = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.isKinematic = true;
        ResetWalk();
        audio.Stop();
    }

    public override void SetType()
    {
        objectType = ObjectType.Sokoban1x1;

        if (audio)
        {
            audio.clip = SoundBox.Instance.sokobanDrag;
        }
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

    private void LateUpdate()
    {
        CheckMovement();
        latestPosition = transform.position;
    }
}
