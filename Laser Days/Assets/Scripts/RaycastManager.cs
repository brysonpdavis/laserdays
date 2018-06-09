using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastManager : MonoBehaviour {

    private GameObject raycastedObj;
    public IList<GameObject> selectedObjs;


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

                // pick item up on left click
                if(Input.GetMouseButtonDown(0))
                {
                    raycastedObj.GetComponent<ThrowObject>().PickUp();
                }
                
                // if item boosts charge, add value to boost on right click
                if(Input.GetMouseButtonDown(1))
                {
                    if (ip.boost)
                        ip.Interaction();
                    
                    ThrowObject throwObj = raycastedObj.GetComponent<ThrowObject>();
                    if (ip.objectCharge)
                    {
                        if (throwObj.selected)
                        {
                            RemoveFromList(raycastedObj);
                            throwObj.selected = false;
                        
                            // change shader
                        }
                        else 
                        {
                            AddToList(raycastedObj);
                            throwObj.selected = true;

                            // change shader back
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

    void AddToList(GameObject obj) 
    {
        selectedObjs.Add(obj);
        Debug.Log("added");
    }

    void RemoveFromList(GameObject obj)
    {
        selectedObjs.Remove(obj);
        Debug.Log("removed");
    } 

    public int SumList(IList<GameObject> objs)
    {
        int i = 0;
        foreach (GameObject obj in objs) {
            i += obj.GetComponent<ItemProperties>().value;
        }
        Debug.Log("done: total is " + i);
        return i;
    }
}
