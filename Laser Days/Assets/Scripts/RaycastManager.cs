using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]


public class RaycastManager : MonoBehaviour {

    public GameObject raycastedObj;
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
    [SerializeField] private PlayerCharge playerCharge;
    [SerializeField] public Image crossHair;
    [SerializeField] private Text itemNameText;
    private Camera mainCam;
    private PlayerCharge pc;
    private AudioSource audioSource;
    private AudioClip selectClip; 
    private AudioClip deselectClip;
    private MFPP.Modules.PickUpModule pickUp;
    private IconContainer iconContainer;
    private Transform playerCam;
    private float Radius;


    void Start () {
        selectedObjs = new List<GameObject>();
        mainCam = Camera.main;
        pc = GetComponent<PlayerCharge>();
        audioSource = GetComponent<AudioSource>();
        selectClip = SoundBox.Instance.selection;
        deselectClip = SoundBox.Instance.deselect;
        pickUp = GetComponent<MFPP.Modules.PickUpModule>();
        iconContainer = Toolbox.Instance.GetIconContainer();
        playerCam = GetComponentInChildren<Camera>().transform;
        Radius = GetComponent<CharacterController>().radius;

    }
	
	// Update is called once per frame
	void Update () {

        if (this.gameObject.layer == 15) { newLayerMask = 1 << 0 | 1 << 10 | 1 << 17; } // newLayerMask.value = 1024; } //layermask value of layer 10 is 1024 (2^10)  
        else if (this.gameObject.layer == 16) { newLayerMask = 1 << 0 | 1 << 11 | 1 << 17; }  //newLayerMask.value = 2048; } //layermask value of layer 11 is 2048   

        RaycastHit hit;
        Vector3 fwd = mainCam.transform.TransformDirection(Vector3.forward);


       if (Physics.Raycast(mainCam.transform.position, fwd, out hit, rayLength, newLayerMask.value))
        {
            //makes sure no objects have the toggle indicating they're within range.
            //if something was previously in range on the last frame it'll deselect it

            //CASES TO TURN THINGS OFF:
            //if (!Toolbox.Instance.GetPickUp().heldObject){

            if (hit.collider.CompareTag("Completion"))
            {
                raycastedObj = hit.collider.gameObject;
                IconCheck(hit.distance, hit.collider.gameObject);
            }


            //1: objects on the default/shared layers are included in the layermask to block raycasts,
            //if we were previously raycasting something OTHER than the held object, turn that off
            else if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 17)
            {
                if (raycastedObj && !pickUp.heldObject)
                {
                    raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
                    if (raycastedObj.GetComponent<SelectionRenderChange>())
                    {
                        raycastedObj.GetComponent<SelectionRenderChange>().SwitchRenderersOff();
                    }
                }
                CrosshairNormal();
                itemNameText.text = null;
                raycastedObj = null;
            }



            else if (hit.collider.CompareTag("Clickable"))
            {
                IconCheck(hit.distance, hit.collider.gameObject);

                //2: Turn off if we hit a new interactable object while not holding anything
                if (raycastedObj && !Toolbox.Instance.EqualToHeld(raycastedObj))
                {
                    raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
                    if (raycastedObj.GetComponent<SelectionRenderChange>())
                    {
                        raycastedObj.GetComponent<SelectionRenderChange>().SwitchRenderersOff();

                    }
                }

                CrosshairActive();
                raycastedObj = hit.collider.gameObject;
                InteractableObject ip = raycastedObj.GetComponent<InteractableObject>();
                itemNameText.text = ip.itemName + " [" + ip.value + "]";

                //SHOW HOVER: 1: if there isn't a held object, 2: keep setting an object to hover as long as it's the held object
                if (!pickUp.heldObject || (pickUp.heldObject && pickUp.heldObject.Equals(raycastedObj))){
                    if (raycastedObj.GetComponent<SelectionRenderChange>())
                    {                             
                        raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 1);
                        raycastedObj.GetComponent<SelectionRenderChange>().SwitchRenderersOn();
                    }

                    else
                    {
                        raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 1);
                    }
                }



                // SELECT ITEM: 
                // if item boosts charge, add value to boost on right click
                //only lets you select items that are flippable
                if(Input.GetButtonDown("Select") && hit.collider.GetComponent<InteractableObject>().Flippable)
                {

                    if (ip.boost)
                    {
                        pc.ItemInteraction(raycastedObj);
                    }
                    if (ip.objectCharge && !Toolbox.Instance.EqualToHeld(raycastedObj))
                    {
                        InteractableObject.ObjectType type = raycastedObj.GetComponent<InteractableObject>().objectType;
                        //if the object is already a selected object:
                        if (raycastedObj.GetComponent<InteractableObject>().selected)
                        {
                            //unselect it
                            selectedObjs.Remove(raycastedObj);

                            //put the object back to its original shader
                            if (this.gameObject.layer == 15) {

                                ShaderUtility.ShaderToLaser(raycastedObj.GetComponent<Renderer>().material);
                                    //raycastedObj.GetComponent<Renderer>().material.shader = laserWorldShader; 
                                    //raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
                            }
                            else if (this.gameObject.layer == 16) {
                                ShaderUtility.ShaderToReal(raycastedObj.GetComponent<Renderer>().material);
                                //raycastedObj.GetComponent<Renderer>().material.shader = realWorldShader;
                                    //raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
                            }

                            //remove it from list
                            RemoveFromList(raycastedObj, false, false);

                            //play deselect sound effect
                            audioSource.clip = deselectClip;
                            audioSource.Play();


                        }
                        else 
                        {
                            if (!(GetComponent<MFPP.Modules.PickUpModule>().heldObject) && raycastedObj.GetComponent<FlippableObject>().MaxFlipCheck(false)) {
                                selectedObjs.Add(raycastedObj);
                                AddToList(raycastedObj);

                                //play the sound effect
                                audioSource.clip = selectClip;
                                audioSource.Play();


                            }
                        }
                    }
                }
            }

            //for objects on real/laser layers that aren't interactable
            else 
            {
                ClearRaycast();
            }

        }

        //if the raycast hits nothing
        else{
            
            ClearRaycast();
        }
	}

    void ClearRaycast()
    {
        //if it hits nothing within the layermask it should also mke sure the raycasted obj from the last frame is set to off
        //same idea for when hitting non-interactable objects on real/laser layers
        //works unless player is holding something

        if (raycastedObj && !pickUp.heldObject)
        {
            raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
        }

        //turns off the rest of the selection indicator
        CrosshairNormal();
        itemNameText.text = null;
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
        InteractableObject ip = obj.GetComponent<InteractableObject>();
        ip.selected = true;

        switch (obj.GetComponent<InteractableObject>().objectType)

        {
        case InteractableObject.ObjectType.Clickable:
            {
                // obj.GetComponent<Renderer>().material.SetInt("_onHover", 1);
                obj.GetComponent<InteractableObject>().Select();
                break;
            }

        case InteractableObject.ObjectType.Wall:
            {
                obj.GetComponent<InteractableObject>().Select();
                break;
            }


        case InteractableObject.ObjectType.Morph:
            {
                    obj.GetComponent<MorphController>().OnSelection();
                    break;
            }


        }

        // change shader back
        pc.UpdatePredictingSlider();
    }

    public void RemoveFromList(GameObject obj, bool asGroup, bool duringFlip) 
    {

        //added asGroup bool to check if player is removing single objects or multiple
        //removing multiple at once shouldn't update the predicting slider at all, it's done separately on the flip

        obj.GetComponent<InteractableObject>().selected = false;
        if (!duringFlip)
        {
            obj.GetComponent<InteractableObject>().UnSelect();
        }

        if (obj.GetComponent<InteractableObject>().objectType == InteractableObject.ObjectType.Morph)
        {
            obj.GetComponent<MorphController>().OnDeselection();
        }

        //shader change is now happening in flip script

        if (!asGroup)
        {
            pc.UpdatePredictingSlider();
        }
    } 

    int SumList(IList<GameObject> objs)
    {
        int i = 0;
        foreach (GameObject obj in objs) {
            i += obj.GetComponent<InteractableObject>().value;
        }
        return i;
    }

    public int SumSelectedObjects() {
        return SumList(selectedObjs);
    }

    public void IconCheck(float distance, GameObject raycastedObj)
    {
        InteractableObject interactable = raycastedObj.GetComponent<InteractableObject>();

        if (raycastedObj.CompareTag("Completion"))
        {
            if (distance <= pickUp.MaxPickupDistance)
                iconContainer.SetOpenHand();

            //can have an else with another icon
        }

        else if (!pickUp.heldObject)
        {
            if (distance <= pickUp.MaxPickupDistance)
            {

                Ray r = new Ray(transform.position + Vector3.up * Radius, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity) && hit.collider.gameObject.Equals(raycastedObj))
                {
                        interactable.DistantIconHover();
                }

                else {
                    interactable.CloseIconHover();
                }
            }

            else {
                interactable.DistantIconHover();
            }
        }
    }

}
