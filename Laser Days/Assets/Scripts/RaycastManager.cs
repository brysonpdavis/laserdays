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


    void Start () {
        selectedObjs = new List<GameObject>();
        mainCam = Camera.main;
        pc = GetComponent<PlayerCharge>();
        audioSource = GetComponent<AudioSource>();
        selectClip = GetComponent<SoundBox>().selection;
        deselectClip = GetComponent<SoundBox>().deselect;
        pickUp = GetComponent<MFPP.Modules.PickUpModule>();
        iconContainer = Toolbox.Instance.GetIconContainer();
        playerCam = GetComponentInChildren<Camera>().transform;

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

            //objects on the default/shared layers are included in the layermask to block raycasts, but they turn off indicator if that's what the raycast hits
            if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 17)
            {
                if (raycastedObj)
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
                //TODO: IconCheck() Here
                IconCheck(hit.distance, hit.collider.gameObject);


                if (raycastedObj)
                {
                    raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
                    if (raycastedObj.GetComponent<SelectionRenderChange>())
                    {
                        raycastedObj.GetComponent<SelectionRenderChange>().SwitchRenderersOff();

                    }
                }

                CrosshairActive();
                raycastedObj = hit.collider.gameObject;
                ItemProperties ip = raycastedObj.GetComponent<ItemProperties>();
                itemNameText.text = ip.itemName + " [" + ip.value + "]";

                if (!pickUp.heldObject || (pickUp.heldObject && !pickUp.heldObject.Equals(raycastedObj))){
                    if (raycastedObj.GetComponent<SelectionRenderChange>()){
                        raycastedObj.GetComponent<SelectionRenderChange>().SwitchRenderersOn();
                    }

                    else hit.collider.gameObject.GetComponent<Renderer>().material.SetInt("_onHover", 1);
                }



                // SELECT ITEM: 
                // if item boosts charge, add value to boost on right click
                //only lets you select items that are flippable
                if(Input.GetMouseButtonDown(1) && !hit.collider.GetComponent<ItemProperties>().unflippable)
                {

                    if (ip.boost)
                    {
                        pc.ItemInteraction(raycastedObj);
                    }
                    if (ip.objectCharge)
                    {
                        ItemProperties.ObjectType type = raycastedObj.GetComponent<ItemProperties>().objectType;
                        //if the object is already a selected object:
                        if (raycastedObj.GetComponent<ItemProperties>().selected)
                        {
                            //unselect it
                            selectedObjs.Remove(raycastedObj);



                            //put the object back to its original shader
                            if (this.gameObject.layer == 15) { 

                                    raycastedObj.GetComponent<Renderer>().material.shader = laserWorldShader; 
                                    raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
                            }
                            else if (this.gameObject.layer == 16) { 
                                    raycastedObj.GetComponent<Renderer>().material.shader = realWorldShader;
                                    raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
                            }

                            //remove it from list
                            RemoveFromList(raycastedObj, false);


                            //play deselect sound effect
                            audioSource.clip = deselectClip;
                            audioSource.Play();


                        }
                        else 
                        {
                            if (!(GetComponent<MFPP.Modules.PickUpModule>().heldObject)) {
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

        }

        else{
            //if it hits nothing within the layermask it should also mke sure the raycasted obj from the last frame is set to off
            if (raycastedObj)
            {
                raycastedObj.GetComponent<Renderer>().material.SetInt("_onHover", 0);
               // Debug.Log("turning off");
            }
            //turns off the rest of the selection indicator
            CrosshairNormal();
            itemNameText.text = null;
            raycastedObj = null;
        }
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
        ItemProperties ip = obj.GetComponent<ItemProperties>();
        ip.selected = true;

        switch (obj.GetComponent<ItemProperties>().objectType)

        {
        case ItemProperties.ObjectType.Clickable:
            {
                // obj.GetComponent<Renderer>().material.SetInt("_onHover", 1);
                obj.GetComponent<ItemProperties>().Select();
                break;
            }

        case ItemProperties.ObjectType.Wall:
            {
                obj.GetComponent<Renderer>().material.SetInt("_onHover", 1);
                break;
            }


        case ItemProperties.ObjectType.Morph:
            {
                    obj.GetComponent<MorphController>().OnSelection();
                    break;
            }


        }

        // change shader back
        pc.UpdatePredictingSlider();
    }

    public void RemoveFromList(GameObject obj, bool asGroup) 
    {

        //added asGroup bool to check if player is removing single objects or multiple
        //removing multiple at once shouldn't update the predicting slider at all, it's done separately on the flip

        obj.GetComponent<ItemProperties>().selected = false;

        if (obj.GetComponent<ItemProperties>().objectType == ItemProperties.ObjectType.Morph)
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
            i += obj.GetComponent<ItemProperties>().value;
        }
        return i;
    }

    public int SumSelectedObjects() {
        return SumList(selectedObjs);
    }

    public void IconCheck(float distance, GameObject raycastedObj)
    {
        



        if (!pickUp.heldObject)
        {
            if (distance <= pickUp.MaxPickupDistance)
            {
                if ((playerCam.transform.localEulerAngles.x > 65f) && (playerCam.transform.localEulerAngles.x < 91f) && crossHair.color == new Color32(255, 222, 77, 255))
                {
                    if (!raycastedObj.GetComponent<ItemProperties>().unflippable)
                    {
                        iconContainer.SetSelectHover();  
                    }

                    else {
                        iconContainer.SetInteractHover();
                    }

                }

                else {
                    iconContainer.SetOpenHand(); 
                }
            }

            else if (!raycastedObj.GetComponent<ItemProperties>().unflippable) 
            {
                iconContainer.SetSelectHover();    
            }

            else {
                
                iconContainer.SetInteractHover();
            }

        }


    }

}
