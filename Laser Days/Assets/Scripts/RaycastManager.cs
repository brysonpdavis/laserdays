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

    void Start () {
        selectedObjs = new List<GameObject>();
        mainCam = Camera.main;
        pc = GetComponent<PlayerCharge>();
    }
	
	// Update is called once per frame
	void Update () {

        if (this.gameObject.layer == 15) { newLayerMask.value = 1024; } //layermask value of layer 10 is 1024 (2^10)
        else if (this.gameObject.layer == 16) { newLayerMask.value = 2048; } //layermask value of layer 11 is 2048

        RaycastHit hit;
        Vector3 fwd = mainCam.transform.TransformDirection(Vector3.forward);


       if (Physics.Raycast(mainCam.transform.position, fwd, out hit, rayLength, newLayerMask.value))
        {
            
            if (hit.collider.CompareTag("Clickable") || hit.collider.CompareTag("Sokoban") || hit.collider.CompareTag("MorphOn"))
            
            {
                CrosshairActive();
                raycastedObj = hit.collider.gameObject;
                ItemProperties ip = raycastedObj.GetComponent<ItemProperties>();
                itemNameText.text = ip.itemName + " [" + ip.value + "]";


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

                        //if the object is already a selected object:
                        if (raycastedObj.GetComponent<ItemProperties>().selected)
                        {
                            //unselect it
                            selectedObjs.Remove(raycastedObj);



                            //put the object back to its original shader
                            if (this.gameObject.layer == 15) { 

                                if (raycastedObj.CompareTag("MorphOn") || raycastedObj.CompareTag("MorphOff")){
                                    raycastedObj.GetComponent<Renderer>().material.shader = morphLaserWorldShader; 
                                    //raycastedObj.GetComponent<Renderer>().material.SetInt("_IsSelected", 0);
                                }
                                else {
                                    raycastedObj.GetComponent<Renderer>().material.shader = laserWorldShader; 
                                    raycastedObj.GetComponent<Renderer>().material.SetInt("_IsSelected", 0);
                                } 
                            }
                            else if (this.gameObject.layer == 16) { 
                                if (raycastedObj.CompareTag("MorphOn") || raycastedObj.CompareTag("MorphOff"))
                                {
                                    raycastedObj.GetComponent<Renderer>().material.shader = morphRealWorldShader;
                                }

                                else {
                                    raycastedObj.GetComponent<Renderer>().material.shader = realWorldShader;
                                    raycastedObj.GetComponent<Renderer>().material.SetInt("_IsSelected", 0);
                                }
                            }

                            //remove it from list
                            RemoveFromList(raycastedObj, false);


                            //if it's a morph, make it drop its associated morph
                            if (hit.collider.CompareTag("MorphOn"))
                            {
                                hit.collider.GetComponent<Morph>().OnPutDown();
                            }

                        }
                        else 
                        {
                            if (!(GetComponent<MFPP.Modules.PickUpModule>().heldObject)) {
                                selectedObjs.Add(raycastedObj);
                                AddToList(raycastedObj);

                                //if it's a morph, make it add its associated morph
                                if (hit.collider.CompareTag("MorphOn"))
                                {
                                    hit.collider.GetComponent<Morph>().OnPickup();
                                }

                            }
                        }
                    }
                }
            }
        }

        else{
            CrosshairNormal();
            itemNameText.text = null;
            //set text to normal
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
        crossHair.color = Color.clear;
    }

    public void AddToList(GameObject obj) 
    {
        ItemProperties ip = obj.GetComponent<ItemProperties>();
        ip.selected = true;

        if (obj.CompareTag("Clickable"))
        {
            obj.GetComponent<Renderer>().material.SetInt("_IsSelected", 1);


        }

        else if (obj.CompareTag("Sokoban"))
        {
            //TODO: change this to sokobanselected when there is one!
            obj.GetComponent<Renderer>().material.shader = shaderselected;
        }

        else if (obj.CompareTag("MorphOn") || obj.CompareTag("MorphOff") )
        {
            //change to morph shader when we have one!

        }



        // change shader back
        pc.UpdatePredictingSlider();
    }

    public void RemoveFromList(GameObject obj, bool asGroup) 
    {

        //added asGroup bool to check if player is removing single objects or multiple
        //removing multiple at once shouldn't update the predicting slider at all, it's done separately on the flip

        obj.GetComponent<ItemProperties>().selected = false;

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
}
