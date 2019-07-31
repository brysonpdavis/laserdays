using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;


[DisallowMultipleComponent]


public class RaycastManager : MonoBehaviour {

    public GameObject raycastedObj;
    private GameObject previousRaycastedObj;
    public IList<GameObject> selectedObjs;

    //public Shader shaderoriginal;

    //shaders
    public Shader shaderselected;
    public Shader realWorldShader;
    public Shader laserWorldShader;
    public Shader coreLaser;
    public Shader coreReal;


    public Shader morphShaderselected;
    public Shader morphRealWorldShader;
    public Shader morphLaserWorldShader;


    private Color32 reticleColor = new Color32(255,222,77,255); //new Vector4(255, 222, 77, 1);


    [Header("Raycast Settings")]

    [SerializeField] private float rayLength = 2.5f;
    [SerializeField] private LayerMask newLayerMask;



    [Header("References")]
    public Image crossHair;
    public Text itemNameText;
    private Camera mainCam;
    private AudioSource audioSource;
    private AudioClip selectClip; 
    private AudioClip deselectClip;
    private MFPP.Modules.PickUpModule pickUp;
    private IconContainer iconContainer;
    private Transform playerCam;
    private float Radius;

    [Header("Scene Reset")]
    const float nSecond = 1f;

    float timer = 0;
    bool entered = false;
    private EdgeDetection edge;
    private ResetScene currentLevelReset;
    private ReticleObject raycastedReticleObj;
    private ISelectable raycastedSelectable;
    private IHoldable raycastedHoldable;
    private IFlippable raycastedFlippable;



    void Start () {
        selectedObjs = new List<GameObject>();
        mainCam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        selectClip = SoundBox.Instance.selection;
        deselectClip = SoundBox.Instance.deselect;
        pickUp = GetComponent<MFPP.Modules.PickUpModule>();
        iconContainer = Toolbox.Instance.GetIconContainer();
        playerCam = GetComponentInChildren<Camera>().transform;
        Radius = GetComponent<CharacterController>().radius;
        edge = Camera.main.GetComponent<EdgeDetection>();


    }
	
	// Update is called once per frame
	void Update () {

        if (this.gameObject.layer == 15) { newLayerMask = LayerMaskController.Laser; } // newLayerMask.value = 1024; } //layermask value of layer 10 is 1024 (2^10)  
        else if (this.gameObject.layer == 16) { newLayerMask = LayerMaskController.Real; }  //newLayerMask.value = 2048; } //layermask value of layer 11 is 2048   

        MainRaycast();
        //SceneResetCheck();
    }

    void MainRaycast()
    {

        RaycastHit hit;
        Vector3 fwd = mainCam.transform.TransformDirection(Vector3.forward);


        if (Physics.Raycast(mainCam.transform.position, fwd, out hit, rayLength, newLayerMask.value))
        {
            raycastedReticleObj = hit.collider.GetComponent<ReticleObject>();

            //if we hit something that can't be interacted with
            if (!raycastedReticleObj)
            {
                ClearRaycast();
            }

            //if we hit something!
            else 
            {
                //will be legacy
                CheckForLookObjects(hit);

                LookAtRaycastedObj(hit);
                raycastedSelectable = raycastedReticleObj as ISelectable;
                
                // SELECT ITEM: 
                if (ControlManager.Instance.GetButtonDown("Select") && raycastedSelectable != null)
                    SelectObject();

            }
        }

        //if the raycast hits nothing
        else
        {
            ClearRaycast();
        }
        
        previousRaycastedObj = raycastedObj;
        raycastedObj = null;
    }

    void LookAtRaycastedObj(RaycastHit hit)
    {
        IconCheck(hit.distance, hit.collider.gameObject);

        CrosshairActive();
        raycastedObj = hit.collider.gameObject;

        //Turn off if we hit a new interactable object while not holding anything
        if (previousRaycastedObj && !previousRaycastedObj.Equals(raycastedObj) && !Toolbox.Instance.EqualToHeld(raycastedObj))
        {
            previousRaycastedObj.GetComponent<ReticleObject>().OffHover();
            //Debug.Log(previousRaycastedObj.name + "  " + raycastedObj.name);
        }

        //HOVER: 1: if there isn't a held object, 2: keep setting an object to hover as long as it's the held object
        if (!pickUp.heldObject || (pickUp.heldObject && pickUp.heldObject.Equals(raycastedObj)))
        {
            raycastedReticleObj.OnHover();
        }
    }

    void SelectObject()
    {
        if (!Toolbox.Instance.EqualToHeld(raycastedObj))
        {


            //if the object is already a selected object:
            if (raycastedSelectable.GetSelected())
            {
                //unselect it
                selectedObjs.Remove(raycastedObj);
                raycastedSelectable.OffSelect();

                //play deselect sound effect
                audioSource.clip = deselectClip;
                audioSource.Play();
            }
            else
            {
                FlippableObject flippableRaycastedObj = raycastedReticleObj as FlippableObject;


                if (flippableRaycastedObj == null) {
                    raycastedSelectable.OnSelect();
                }
                else {
                    if (!(pickUp.heldObject) && flippableRaycastedObj &&
                        flippableRaycastedObj.MaxFlipCheck(false))
                    {
                        selectedObjs.Add(raycastedObj);
                        flippableRaycastedObj.OnSelect();
                    }
                }

                //play the sound effect
                audioSource.clip = selectClip;
                audioSource.Play();
            }
        }
    }

    void CheckForLookObjects(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Completion"))
        {
            raycastedObj = hit.collider.gameObject;
            IconCheck(hit.distance, hit.collider.gameObject);
        }
    }

    void ClearRaycast()
    {
        //if it hits nothing within the layermask it should also mke sure the raycasted obj from the last frame is set to off
        //same idea for when hitting non-interactable objects on real/laser layers
        //works unless player is holding something


        if (previousRaycastedObj)
            previousRaycastedObj.GetComponent<ReticleObject>().OffHover();

        //turns off the rest of the selection indicator
        CrosshairNormal();
        raycastedObj = null;
    }


    //changing color of crosshair based on raycast
    void CrosshairActive()
    {
        //crossHair.color = Color.red;
        crossHair.color = reticleColor;

    }

    void CrosshairNormal()
    {
        if (!pickUp.heldObject){
            iconContainer.SetDefault();
        }
        crossHair.color = Color.clear;
    }

    public void AddToList(GameObject obj) 
    {
        HoldableObject ip = obj.GetComponent<HoldableObject>();
        ip.selected = true;

        switch (obj.GetComponent<HoldableObject>().objectType)

        {
        case HoldableObject.ObjectType.Clickable:
            {
                // obj.GetComponent<Renderer>().material.SetInt("_onHover", 1);
                obj.GetComponent<HoldableObject>().OnSelect();
                break;
            }

        case HoldableObject.ObjectType.Wall:
            {
                obj.GetComponent<HoldableObject>().OnSelect();
                break;
            }

            case HoldableObject.ObjectType.LinkedPair:
                {
                    obj.GetComponent<HoldableObject>().OnSelect();
                    break;
                }


        case HoldableObject.ObjectType.Morph:
            {
                    obj.GetComponent<MorphController>().OnSelection();
                    break;
            }


        }
    }

    public void RemoveFromList(GameObject obj, bool asGroup, bool duringFlip) 
    {

        //added asGroup bool to check if player is removing single objects or multiple
        //removing multiple at once shouldn't update the predicting slider at all, it's done separately on the flip

        obj.GetComponent<HoldableObject>().selected = false;
        if (!duringFlip)
        {
            obj.GetComponent<HoldableObject>().OffSelect();
        }

        if (obj.GetComponent<HoldableObject>().objectType == HoldableObject.ObjectType.Morph)
        {
            obj.GetComponent<MorphController>().OnDeselection();
        }
    } 
    
    public void IconCheck(float distance, GameObject raycastedObj)
    {
        ReticleObject reticleObject = raycastedObj.GetComponent<ReticleObject>();

        if (!pickUp.heldObject)
        {
            if (distance <= pickUp.MaxPickupDistance)
            {

                Ray r = new Ray(transform.position + Vector3.up * Radius, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity) && hit.collider.gameObject.Equals(raycastedObj))
                {
                    reticleObject.DistantIconHover();
                }

                else {
                    reticleObject.CloseIconHover();
                }
            }

            else {
                reticleObject.DistantIconHover();
            }
        }
    }

    public void PointerEnter()
    {
        entered = true;
    }

    public void PointerExit()
    {
        entered = false;
    }

}
