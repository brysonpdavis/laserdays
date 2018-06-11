using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastManager : MonoBehaviour {

    public GameObject raycastedObj;
    public IList<GameObject> selectedObjs;
    public Shader shaderselected;
    public Shader shaderoriginal;


    [Header("Raycast Settings")]

    [SerializeField] private float rayLength = 2.5f;
    [SerializeField] private LayerMask newLayerMask;



    [Header("References")]
    [SerializeField] private PlayerCharge playerCharge;
    [SerializeField] private Image crossHair;
    [SerializeField] private Text itemNameText;
    public GameObject heldObject;

    void Start () {
        selectedObjs = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
       
        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, newLayerMask.value))
        {
            if (hit.collider.CompareTag("Clickable"))
            {
                CrosshairActive();
                raycastedObj = hit.collider.gameObject;
                ItemProperties ip = raycastedObj.GetComponent<ItemProperties>();
                itemNameText.text = ip.itemName + " [" + ip.value + "]";
                itemNameText.transform.position = Camera.main.WorldToScreenPoint(raycastedObj.transform.position);

                // pick item up on left click
                if (Input.GetMouseButtonDown(0))
                {
                    raycastedObj.GetComponent<ThrowObject>().PickUp();
                }
                
                // if item boosts charge, add value to boost on right click
                if(Input.GetMouseButtonDown(1))
                {
                    if (ip.boost)
                        ip.Interaction();
                    
                    if (ip.objectCharge)
                    {
                        if (raycastedObj.GetComponent<ThrowObject>().selected)
                        {
                            selectedObjs.Remove(raycastedObj);
                            RemoveFromList(raycastedObj);
                        }
                        else 
                        {
                            selectedObjs.Add(raycastedObj);
                            AddToList(raycastedObj);
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
        crossHair.color = Color.red;
    }

    void CrosshairNormal()
    {
        crossHair.color = Color.white;
    }

    public void AddToList(GameObject obj) 
    {
        obj.GetComponent<ThrowObject>().selected = true;
        obj.GetComponent<Renderer>().material.shader = shaderselected;
        // change shader back

    }

    public void RemoveFromList(GameObject obj)
    {
        obj.GetComponent<ThrowObject>().selected = false;
        obj.GetComponent<Renderer>().material.shader = shaderoriginal; 
        //change shader

    } 

    public int SumList(IList<GameObject> objs)
    {
        int i = 0;
        foreach (GameObject obj in objs) {
            i += obj.GetComponent<ItemProperties>().value;
        }
        return i;
    }
}
