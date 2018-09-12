﻿using UnityEngine;  namespace MFPP.Modules {     [HelpURL("https://ashkoredracson.github.io/MFPP/#pick-up-module")]      [DisallowMultipleComponent]       public class PickUpModule : PlayerModule     {         /// <summary>         /// The maximum pickup distance.         /// </summary>         [Space]         [Tooltip("The maximum pickup distance.")]         public float MaxPickupDistance = 2f;         public float breakDistance = 5f;         public float maxVelocity = 12;          [Tooltip("Pick up button.")]         public string PickUpButton = "Pick Up";         public Transform playerCam;         public GameObject heldObject;         public FootstepAsset originalFootstep;         public FootstepAsset dragFootstep;          [SerializeField] private LayerMask newLayerMask;         private PlayerCharge pc;         private AudioSource audioSource;         private AudioClip pickupClip;         private AudioClip dropClip;         private Camera mainCamera;         private Vector3 sokobanPosition;         private float originalWalkingSpeed = 3;         private IconContainer iconContainer;         public Rigidbody target;             void Start ()         {             mainCamera = Camera.main;             pc = GetComponent<PlayerCharge>();             audioSource = GetComponent<AudioSource>();             pickupClip = GetComponent<SoundBox>().pickup;             dropClip = GetComponent<SoundBox>().deselect;             iconContainer = Toolbox.Instance.GetIconContainer();          }           public override void AfterUpdate()         {             if (Input.GetButtonDown(PickUpButton)) // If pick up button was pressed             {                 if (target) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.                 {                     PutDown();                 }                 else                 {                      //check if the object is already selected, remove it from list                     if (heldObject && heldObject.GetComponent<ItemProperties>().selected)                     {                         RaycastManager rm = GetComponent<RaycastManager>();                         rm.RemoveFromList(target.gameObject, true);                         rm.selectedObjs.Remove(target.gameObject);                         heldObject.GetComponent<Renderer>().material.SetInt("_IsSelected", 1);                          pc.UpdatePredictingSlider();                     }                       //FOR LATER: not sure this is totally working (why I'm checking rigidbody's layer vs. player's layer right before the pickup)                     if (this.gameObject.layer == 15) { newLayerMask.value = 1024; } //layermask value of layer 10 is 1024                     else if (this.gameObject.layer == 16) { newLayerMask.value = 2048; } //layermask value of layer 11 is 2048                      Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);                     RaycastHit hit;                     if (Physics.Raycast(r, out hit, MaxPickupDistance, newLayerMask.value)) // Otherwise, if target was null, shoot a ray where we are aiming.                     {                         Rigidbody body = hit.collider.attachedRigidbody;                          // Retreive the rigidbody and assure that it is not kinematic, and that it's not an unwanted obj type                         //checking object's layer once more, wasn't working for morphs using normal layermask                         if  (body &&                              body.CompareTag("Clickable") &&                             ((body.gameObject.layer + 5) == this.gameObject.layer ))                          {                             if (body.GetComponent<ItemProperties>() && (body.GetComponent<ItemProperties>().objectType == ItemProperties.ObjectType.Wall))                             {                                 WallSelect(body.gameObject);                             }                              else {                                 PickUp(body);                                 //play sounds for pickup!                                 audioSource.clip = pickupClip;                                 audioSource.Play();                             }                          }                       }                   }             }              if (target) // If target is not null, move the target in front of the camera at max pickup distance.
            {                 ItemProperties.ObjectType type = target.GetComponent<ItemProperties>().objectType;  
                //AVOIDING FLYING ON TOP OF HELD OBJECT GLITCH:
                //checks rotation of player so that it's down (the between the 67 and 91f)
                //checks that the player is actually looking at OBJECT, (colored crosshair), rather than looking at ground while object is distant
                if ((playerCam.transform.localEulerAngles.x > 65f) && (playerCam.transform.localEulerAngles.x < 91f) && GetComponent<RaycastManager>().crossHair.color == new Color32(255, 222, 77, 255))                  {                     PutDown();                 }                   else  {                     //These Objects don't get moved in front of the camera every frame                     if (!(type == ItemProperties.ObjectType.Sokoban1x1 ||                            type == ItemProperties.ObjectType.Sokoban2x2||                           type == ItemProperties.ObjectType.Morph))
                    {
                        Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * MaxPickupDistance;
                        target.angularVelocity *= 0.5f;
                        target.velocity = ((floatingPosition - target.transform.position) * 10f);
                    }
                     //anything else should get moved in front of the camera
                    else
                    {
                        Vector3 floatingPosition = this.transform.position + (sokobanPosition + mainCamera.transform.forward);
                        target.angularVelocity *= 0.5f;
                        target.velocity = ((floatingPosition - target.transform.position) * 10f);
                    }

                    //drops the held object if the player manages to get too far away from it
                    if (Mathf.Abs(Vector3.Distance(this.transform.position, target.transform.position)) > breakDistance)
                    {                         PutDown();                     }
                }             }         }          public void PickUp (Rigidbody body) {             target = body; // Set the target             sokobanPosition = body.transform.position - (this.transform.position + mainCamera.transform.forward);             heldObject = target.gameObject;             bool secondaryLock = body.GetComponent<ItemProperties>().secondaryLock;             //target.useGravity = false;              switch (target.GetComponent<ItemProperties>().objectType)             {
                case ItemProperties.ObjectType.Clickable:
                    {
                        iconContainer.SetHold();                         target.freezeRotation = true;
                        heldObject.GetComponent<Renderer>().material.SetInt("_onHold", 1);
                        heldObject.GetComponent<ItemProperties>().Select();
                        break;
                    }                  case ItemProperties.ObjectType.Sokoban1x1:
                    {                         iconContainer.SetDrag();
                        target.constraints = RigidbodyConstraints.None;
                        target.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                        heldObject.GetComponent<SelectionRenderChange>().SwitchRenderersOff();
                        heldObject.GetComponent<SelectionRenderChange>().OnHold();
                        GetComponent<Player>().Footstep.Asset = dragFootstep;                         target.isKinematic = false;
                        break;
                    }                   case ItemProperties.ObjectType.WallSliderX:                     {                         iconContainer.SetDrag();                         target.constraints = RigidbodyConstraints.None;                         if (!secondaryLock) { target.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY; }                         else {target.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY; }                         heldObject.GetComponent<SelectionRenderChange>().SwitchRenderersOff();                         heldObject.GetComponent<SelectionRenderChange>().OnHold();                         GetComponent<Player>().Footstep.Asset = dragFootstep;                         target.isKinematic = false;                         break;                     }                  case ItemProperties.ObjectType.WallSliderZ:                     {                         iconContainer.SetDrag();                         target.constraints = RigidbodyConstraints.None;                         if (!secondaryLock) { target.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY; }                         else { target.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY; }                         heldObject.GetComponent<SelectionRenderChange>().SwitchRenderersOff();                         heldObject.GetComponent<SelectionRenderChange>().OnHold();                         GetComponent<Player>().Footstep.Asset = dragFootstep;                         target.isKinematic = false;                         break;                     }                  case ItemProperties.ObjectType.Morph:                     {                         iconContainer.SetDrag();                         target.constraints = RigidbodyConstraints.None;                         target.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;                         heldObject.GetComponent<SelectionRenderChange>().SwitchRenderersOff();                         heldObject.GetComponent<SelectionRenderChange>().OnHold();                         heldObject.GetComponent<MorphController>().OnSelection();                         GetComponent<Player>().Footstep.Asset = dragFootstep;                         target.isKinematic = false;                          break;                     }                  case ItemProperties.ObjectType.Sokoban2x2:                     {                         iconContainer.SetDrag();                         target.constraints = RigidbodyConstraints.None;                         target.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;                         heldObject.GetComponent<SelectionRenderChange>().SwitchRenderersOff();                         heldObject.GetComponent<SelectionRenderChange>().OnHold();                         GetComponent<Player>().Footstep.Asset = dragFootstep;                         target.isKinematic = false;                          break;                     }
             }                      pc.UpdatePredictingSlider();             if (!(target.GetComponent<ItemProperties>().objectType == ItemProperties.ObjectType.Clickable))                     {                         //changing player's walking based on target's mass                         if (target.mass > 2 && target.mass < 3)                         {                             GetComponent<Player>().Movement.Speed = 1;                             GetComponent<BobModule>().BobSpeed = 1.5f;                         }                          else if (target.mass >= 3)                         {                             GetComponent<Player>().Movement.Speed = 1;                             GetComponent<Player>().Movement.Jump.Power = 0;                             GetComponent<BobModule>().BobSpeed = 1.5f;                         }                     }         }          public void PutDown () {                          GetComponent<MFPP.Player>().Movement.AllowMouseMove = true;              RaycastManager rm = GetComponent<RaycastManager>();             switch (target.GetComponent<ItemProperties>().objectType)              {                 case ItemProperties.ObjectType.Clickable:                     {
                        //put the object down with the right shader
                        if (GetComponent<flipScript>().space)                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.realWorldShader;                             heldObject.GetComponent<Renderer>().material.SetInt("_onHold", 0);                             heldObject.layer = 11;                             heldObject.GetComponent<ItemProperties>().UnSelect();                             heldObject.GetComponent<Transition>().SetStart(0f);                         }                          else                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.laserWorldShader;                             heldObject.GetComponent<ItemProperties>().UnSelect();                             heldObject.GetComponent<Renderer>().material.SetInt("_onHold", 0);                             heldObject.layer = 10;                             heldObject.GetComponent<Transition>().SetStart(1f);                         }                          Vector3 currentVelocity = heldObject.GetComponent<Rigidbody>().velocity;                          if (currentVelocity.magnitude > maxVelocity)                         {                             heldObject.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(currentVelocity, maxVelocity);                         }                          target.freezeRotation = false;                         //target.useGravity = true;                      }                     break;                  case ItemProperties.ObjectType.Sokoban1x1:                     {                          if (GetComponent<flipScript>().space)                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.realWorldShader;                             heldObject.layer = 11;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(0f);                         }                          else                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.laserWorldShader;                             heldObject.layer = 10;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(1f);                         }                          target.freezeRotation = false;                         target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;                         target.isKinematic = true;                         //target.useGravity = true;                     }                     break;                  case ItemProperties.ObjectType.WallSliderX:                     {                          if (GetComponent<flipScript>().space)                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.realWorldShader;                             heldObject.layer = 11;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(0f);                         }                          else                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.laserWorldShader;                             heldObject.layer = 10;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(1f);                         }                          target.freezeRotation = false;                         target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;                         target.isKinematic = true;                         //target.useGravity = true;                     }                     break;                  case ItemProperties.ObjectType.WallSliderZ:                     {                          if (GetComponent<flipScript>().space)                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.realWorldShader;                             heldObject.layer = 11;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(0f);                         }                          else                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.laserWorldShader;                             heldObject.layer = 10;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(1f);                         }                          target.freezeRotation = false;                         target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;                         target.isKinematic = true;                         //target.useGravity = true;                     }                     break;                  case ItemProperties.ObjectType.Morph:                     {                          if (GetComponent<flipScript>().space)                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.realWorldShader;                             heldObject.layer = 11;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(0f);                             heldObject.GetComponent<MorphController>().OnDeselection();                         }                          else                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.laserWorldShader;                             heldObject.layer = 10;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(1f);                             heldObject.GetComponent<MorphController>().OnDeselection();                          }                          target.freezeRotation = false;                         target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;                         target.isKinematic = true;                     }                     break;                  case ItemProperties.ObjectType.Sokoban2x2:                     {                          if (GetComponent<flipScript>().space)                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.realWorldShader;                             heldObject.layer = 11;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(0f);                         }                          else                         {                             heldObject.GetComponent<Renderer>().material.shader = rm.laserWorldShader;                             heldObject.layer = 10;                             heldObject.GetComponent<SelectionRenderChange>().OnDrop();                             heldObject.GetComponent<Transition>().SetStart(1f);                         }                          target.freezeRotation = false;                         target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;                         target.isKinematic = true;                     }                     break;             }                                                  iconContainer.SetOpenHand();                         heldObject.GetComponent<ItemProperties>().selected = false;                         heldObject = null; //remove from the list                         target = null;                         pc.UpdatePredictingSlider();                         MFPP.Player player = Toolbox.Instance.GetPlayer().GetComponent<Player>();                           //plays sound effect                         audioSource.clip = dropClip;                         audioSource.Play();                          //resets player's walking speed movement, jump                         player.Movement.Speed = 3;                         player.Movement.Jump.Power = 4.2f;                         player.Footstep.Asset = originalFootstep;                         GetComponent<BobModule>().BobSpeed = 3f;           }          //we don't want walls to be able to picked up in the normal way, but we do want them to be added to the normal list with the pickup key         private void WallSelect(GameObject wall)         {              RaycastManager rm = GetComponent<RaycastManager>();             ItemProperties item = wall.GetComponent<ItemProperties>();              if (item.selected){                 rm.RemoveFromList(wall, false);                 rm.selectedObjs.Remove(wall);                 item.selected = false;                 item.UnSelect();                 //wall.GetComponent<SelectionRenderChange>().OnDrop()                 //wall.GetComponent<Renderer>().material.SetInt("_onHold", 0);                  audioSource.clip = dropClip;                 audioSource.Play();              }             else {                 rm.AddToList(wall);                 rm.selectedObjs.Add(wall);                 item.selected = true;                 item.Select();                 //wall.GetComponent<SelectionRenderChange>().OnHold();                 audioSource.clip = pickupClip;                 audioSource.Play();              }             heldObject = null;             target = null;         }        }   }