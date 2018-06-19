using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    [Header("Your Clickables")]

    public string itemName;
    public bool isKey;
    public string key = null;

    [SerializeField] public bool objectCharge = true;
    [SerializeField] public bool boost = false;

    [SerializeField] public int value;

    [SerializeField] private PlayerCharge playerCharge;

    public bool selected = false;

    private Camera mainCamera;

    void Start () {
        mainCamera = Camera.main;
    }

    // Checks whether the object can be flipped
    public bool Check()
    {

        RaycastManager rm = mainCamera.GetComponentInParent<RaycastManager>();
        return (boost)?false:(value + playerCharge.flipCost + rm.SumList(rm.selectedObjs) <= playerCharge.chargeSlider.value);
    }

    // Handles charge point management
    public void Interaction()
    {
        if(objectCharge){
            
            //Check to see if it can be activated
            RaycastManager rm = mainCamera.GetComponentInParent<RaycastManager>();
            if (value + playerCharge.flipCost + rm.SumList(rm.selectedObjs) <= playerCharge.chargeSlider.value)
            {
                //subtract charge value:
                //for slider
                playerCharge.chargeSlider.value -= value + playerCharge.flipCost + rm.SumList(rm.selectedObjs);

                // for text
                playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();            }
        }


        if (boost)
        {
                //adds charge value:
                //for slider
                playerCharge.chargeSlider.value += value;

                //for text
                playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();
        }
    }
}


