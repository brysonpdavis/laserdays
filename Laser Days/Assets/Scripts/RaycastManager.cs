using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastManager : MonoBehaviour {

    public GameObject raycastedObj;
    public IList<GameObject> selectedObjs;
    public Shader shaderselected;
    public Shader shaderoriginal;
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

        RaycastHit hit;
        Vector3 fwd = mainCam.transform.TransformDirection(Vector3.forward);
       
        if (Physics.Raycast(mainCam.transform.position, fwd, out hit, rayLength, newLayerMask.value))
        {
            if (hit.collider.CompareTag("Clickable") || hit.collider.CompareTag("Sokoban"))
            {
                CrosshairActive();
                raycastedObj = hit.collider.gameObject;
                ItemProperties ip = raycastedObj.GetComponent<ItemProperties>();
                itemNameText.text = ip.itemName + " [" + ip.value + "]";
               
                //sets its position to square's
                // itemNameText.transform.position = mainCam.WorldToScreenPoint(raycastedObj.transform.position);

                // pick item up on left click [CURRENTLY COVERED BY NEW SCRIPT, CAN UN-COMMENT THIS IF NEEDED]
               /*
                if (Input.GetMouseButtonDown(0))
                {
                   // raycastedObj.GetComponent<ThrowObject>().PickUp();
                }
                */


                // SELECT ITEM: 
                // if item boosts charge, add value to boost on right click
                if(Input.GetMouseButtonDown(1))
                {
                    if (ip.boost)
                    {
                        pc.ItemInteraction(raycastedObj);
                    }
                    if (ip.objectCharge)
                    {

                        if (raycastedObj.GetComponent<ItemProperties>().selected)
                        {
                            selectedObjs.Remove(raycastedObj);
                            RemoveFromList(raycastedObj, false);
                        }
                        else 
                        {
                            if (!(GetComponent<MFPP.Modules.PickUpModule>().heldObject)) {
                                selectedObjs.Add(raycastedObj);
                                AddToList(raycastedObj);
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
        obj.GetComponent<Renderer>().material.shader = shaderselected;
        // change shader back
        pc.UpdatePredictingSlider();
    }

    public void RemoveFromList(GameObject obj, bool asGroup) 
    {

        //added asGroup bool to check if player is removing single objects or multiple
        //removing multiple at once shouldn't update the predicting slider at all, it's done separately on the flip

        ItemProperties ip = obj.GetComponent<ItemProperties>();
        obj.GetComponent<ItemProperties>().selected = false;
        obj.GetComponent<Renderer>().material.shader = shaderoriginal;
        //change shader
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
