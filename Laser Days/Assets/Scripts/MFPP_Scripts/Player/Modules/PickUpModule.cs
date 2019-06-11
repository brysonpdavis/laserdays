﻿using UnityEngine; using System.Collections;  namespace MFPP.Modules {     [HelpURL("https://ashkoredracson.github.io/MFPP/#pick-up-module")]      [DisallowMultipleComponent]       public class PickUpModule : PlayerModule     {         /// <summary>         /// The maximum pickup distance.         /// </summary>         [Space]         [Tooltip("The maximum pickup distance.")]         public float MaxPickupDistance = 2f;         public float breakDistance = 5f;         public float maxVelocity = 12;          [Tooltip("Pick up button.")]         public string PickUpButton = "Pick Up";         public Transform playerCam;         public GameObject heldObject;         public FootstepAsset originalFootstep;         public FootstepAsset dragFootstep;         public Rigidbody target;          [SerializeField] private LayerMask newLayerMask;         private PlayerCharge pc;         private Camera mainCamera;         private float originalWalkingSpeed = 3;         private IconContainer iconContainer;         private AudioSource audioSource;         private AudioClip pickupClip;         private AudioClip dropClip;         private AudioClip popClip;         private float CurrentHeight;         private float Radius;          void Start ()         {             mainCamera = Camera.main;             pc = GetComponent<PlayerCharge>();             audioSource = GetComponent<AudioSource>();             SoundBox sound = SoundBox.Instance;             pickupClip = sound.pickup;             popClip = sound.pop;             dropClip = sound.deselect;             iconContainer = Toolbox.Instance.GetIconContainer();             CurrentHeight = GetComponent<MFPP.Player>().CurrentHeight;             Radius = GetComponent<CharacterController>().radius;         }           public override void AfterUpdate()         {             if (Input.GetKeyDown(ControlManager.CM.pickup)) // If pick up button was pressed             {                 if (target) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.                 {PutDown();}                 else                 {PrepareForPickup();}             }              if (target) // If target is not null, move the target in front of the camera at max pickup distance.
            {                 InteractableObject.ObjectType type = target.GetComponent<InteractableObject>().objectType;                 bool check = CheckPlayerStance(target.gameObject);                  if (!check){                     target.gameObject.GetComponent<InteractableObject>().HoldPosition();                     //ObjectPositioning(type);
                }                  if (target && Vector3.Distance(this.transform.position, target.transform.position) >= breakDistance)                 {                     PutDown();                 }              }         }          public void PickUp (Rigidbody body) {             target = body; // Set the target             heldObject = target.gameObject;              //have the object do the pickup changes!             target.GetComponent<InteractableObject>().Pickup();             pc.UpdatePredictingSlider();              //play sounds for pickup!             Toolbox.Instance.SetVolume(audioSource);             audioSource.clip = pickupClip;             audioSource.Play();          }          public void PutDown () {              //drop the object             target.GetComponent<InteractableObject>().Drop();             heldObject = null; //remove from the list             target = null;             pc.UpdatePredictingSlider();              //plays sound effect             Toolbox.Instance.SetVolume(audioSource);             audioSource.clip = dropClip;             audioSource.Play();         }          private void PrepareForPickup()         {             //check if the object is already selected, remove it from list             if (heldObject && heldObject.GetComponent<InteractableObject>().selected)             {                 RaycastManager rm = GetComponent<RaycastManager>();                 rm.RemoveFromList(target.gameObject, true, false);                 rm.selectedObjs.Remove(target.gameObject);                 heldObject.GetComponent<Renderer>().material.SetInt("_IsSelected", 1);                 pc.UpdatePredictingSlider();             }               if (this.gameObject.layer == 15) { newLayerMask.value = 1 << 10 | 1 << 17; } //layermask value of layer 10 is 1024             else if (this.gameObject.layer == 16) { newLayerMask.value = 1 << 11 | 1 << 17; } //layermask value of layer 11 is 2048              Ray r = new Ray(mainCamera.transform.position, mainCamera.transform.forward);             RaycastHit hit;             if (Physics.Raycast(r, out hit, MaxPickupDistance, newLayerMask.value)) // Otherwise, if target was null, shoot a ray where we are aiming.             {                 //completion zone                 //if (hit.collider.CompareTag("Completion"))                 //    hit.collider.GetComponent<PuzzleCompletion>().CompletionInteract();                  Rigidbody body = hit.collider.attachedRigidbody;                  //for picking up actual objects                 if  (body && body.CompareTag("Clickable") &&                     ((body.gameObject.layer + 5) == this.gameObject.layer)                      && !CheckPlayerStance(body.gameObject))                         { PickUp(body); }             }         }           public void KillPickup()         {             target = null;             heldObject = null;         }          private bool CheckPlayerStance(GameObject obj)         {
            //AVOIDING FLYING ON TOP OF HELD OBJECT GLITCH:
            //sends raycast down, if player is standing on top of held object, drops the object              Ray r = new Ray(transform.position + Vector3.up * Radius, Vector3.down);             RaycastHit hit;              if (Physics.SphereCast(r, Radius, out hit, Mathf.Infinity))             {                 if (hit.collider.gameObject.Equals(obj))                 {                     //for if player is holding the object that we're checking, should drop it                     if (heldObject && heldObject.Equals(obj) && !heldObject.GetComponentInChildren<FloorBouncer>())                         PutDown();                     return true;                 }                 else return false;             }              else return false;         }      } }