﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastManager : MonoBehaviour {

    private GameObject raycastedObj;


    [Header("Raycast Settings")]

    [SerializeField] private float rayLength = 10;
    [SerializeField] private LayerMask newLayerMask;



    [Header("References")]
    [SerializeField] private PlayerCharge playerCharge;
    [SerializeField] private Image crossHair;
    [SerializeField] private Text itemNameText;
	
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
                itemNameText.text = raycastedObj.GetComponent<ItemProperties>().itemName + " [" + raycastedObj.GetComponent<ItemProperties>().value + "]";

                if(Input.GetMouseButtonDown(1))
                {

                    raycastedObj.GetComponent<ItemProperties>().Interaction();
                    //Object properties
                }
            }
        }

        else{
            CrosshairNormal();
            itemNameText.text = null;

            //set text to normal
        }
	}

    void CrosshairActive()
    {
        crossHair.color = Color.red;

    }

    void CrosshairNormal()
    {
        crossHair.color = Color.white;
    }
}
