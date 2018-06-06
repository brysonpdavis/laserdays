using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    [Header("Your Clickables")]

    public string itemName;

    [SerializeField] private bool objectCharge = true;
    [SerializeField] public int value;

    [SerializeField] private PlayerCharge playerCharge;

    public void Interaction()
    {
        if(objectCharge){

            //Check to see if it can be activated
            if (value <= playerCharge.chargeSlider.value)
            {
                //subtract charge value
                playerCharge.chargeSlider.value -= value;

                //want something here to change the object's shader, showing that it can shift to either world


                //turn on the switching script
                this.GetComponent<flipScript>().enabled = true;

            }
            }
    }

}


