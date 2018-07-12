﻿using UnityEngine;  namespace MFPP.Modules {     [HelpURL("https://ashkoredracson.github.io/MFPP/#pick-up-module")]     public class PickUpModule : PlayerModule     {         /// <summary>         /// The maximum pickup distance.         /// </summary>         [Space]         [Tooltip("The maximum pickup distance.")]         public float MaxPickupDistance = 2f;         /// <summary>         /// Pick up button.         /// </summary>         [Tooltip("Pick up button.")]         public string PickUpButton = "Pick Up";         private float originalWalkingSpeed = 3;           //OUR VARIABLES [will]         public Transform player;         public Transform playerCam;         private Camera mainCamera;         public GameObject heldObject;         private PlayerCharge pc;          void Start () {             mainCamera = Camera.main;             pc = GetComponent<PlayerCharge>();          }          private Rigidbody target;          public override void AfterUpdate()         {             if (Input.GetButtonDown(PickUpButton)) // If pick up button was pressed             {                 if (target) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.                 {                     PutDown();                 }                 else                 {                     Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);                     RaycastHit hit;                     if (Physics.Raycast(r, out hit, MaxPickupDistance)) // Otherwise, if target was null, shoot a ray where we are aiming.                     {                         Rigidbody body = hit.collider.attachedRigidbody;                         if (body && !body.isKinematic) // Retreive the rigidbody and assure that it is not kinematic.                         {                             PickUp(body);                         }                     }                      //check if the object is already selected, remove it from list                     if (heldObject && heldObject.GetComponent<ItemProperties>().selected)                     {                         RaycastManager rm = GetComponent<RaycastManager>();                         rm.RemoveFromList(target.gameObject, true);                         rm.selectedObjs.Remove(target.gameObject);                          //keep the shader in selected mode though the object is off the list                        heldObject.GetComponent<Renderer>().material.shader = rm.shaderselected;                            pc.UpdatePredictingSlider();                     }                 }             }              if (target) // If target is not null, move the target in front of the camera at max pickup distance.             {                 Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * MaxPickupDistance;                 target.angularVelocity *= 0.5f;                 target.velocity = ((floatingPosition - target.transform.position) * 10f);                   if (target.CompareTag("Sokoban")){                    // target.GetComponentInParent<Transform>().rotation.y = transform.rotation.y;                                      }                  //AVOIDING FLYING ON TOP OF HELD OBJECT GLITCH:                 //checks rotation of player so that it's down (the between the 67 and 91f)                 //checks that the player is actually looking at OBJECT, (colored crosshair), rather than looking at ground while object is distant                 if ((playerCam.transform.localEulerAngles.x > 65f) && (playerCam.transform.localEulerAngles.x < 91f) && GetComponent<RaycastManager>().crossHair.color == new Color32(255, 222, 77, 255))                 {                     PutDown();                 }                 // target.velocity = Vector3.zero;                 // target.AddForce((floatingPosition - target.transform.position) * 20f, ForceMode.VelocityChange);             }         }          void PickUp (Rigidbody body) {             target = body; // Set the target             heldObject = target.gameObject;             target.useGravity = false;              if (target.CompareTag("Clickable"))
            {
                target.freezeRotation = true;
                           }              if (target.CompareTag("Sokoban"))             {                 target.constraints = RigidbodyConstraints.None;                 target.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;             }              RaycastManager rm = GetComponent<RaycastManager>();             heldObject.GetComponent<Renderer>().material.shader = rm.shaderselected;             pc.UpdatePredictingSlider();              //changing player's walking based on target's mass             if (target.mass > 1 && target.mass < 3){                 player.GetComponent<Player>().Movement.Speed = 1;             }              if (target.mass >= 3)             {                 player.GetComponent<Player>().Movement.Speed = 1;                 player.GetComponent<Player>().Movement.Jump.Power = 0;              }          }          void PutDown () {               RaycastManager rm = GetComponent<RaycastManager>();             if (heldObject.CompareTag("Clickable")){
                heldObject.GetComponent<Renderer>().material.shader = rm.shaderoriginal;
                target.freezeRotation = false;
                target.useGravity = true;             }              if (heldObject.CompareTag("Sokoban"))
            {                 heldObject.GetComponent<Renderer>().material.shader = rm.shaderoriginal;                 target.freezeRotation = false;                 target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;                 target.useGravity = true;

            }             heldObject = null; //remove from the list             target = null;             pc.UpdatePredictingSlider();                  //resets player's walking speed movement, jump             player.GetComponent<Player>().Movement.Speed = 3;             player.GetComponent<Player>().Movement.Jump.Power = 4.2f;           }     } }  