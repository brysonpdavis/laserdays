using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    [Header("Your Clickables")]

    public string itemName;

    [SerializeField] private bool objectCharge = true;
    [SerializeField] public bool boost = false;

    [SerializeField] public int value;

    [SerializeField] private PlayerCharge playerCharge;

    // Checks whether the object can be flipped
    public bool Check()
    {
        return (boost)?false:(value + playerCharge.flipCost <= playerCharge.chargeSlider.value);
    }

    // Handles charge point management
    public void Interaction()
    {
        if(objectCharge){
            
            //Check to see if it can be activated
            if (value + playerCharge.flipCost <= playerCharge.chargeSlider.value)
            {
                //subtract charge value:
                //for slider
                playerCharge.chargeSlider.value -= value + playerCharge.flipCost;

                // for text
                playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();

                //want something here to change the object's shader, showing that it can shift to either world

            }
        }


        if (boost)
        {
                //adds charge value:
                //for slider
                playerCharge.chargeSlider.value += value;

                //for text
                playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();

                //want something here to change the object's shader, showing that it can shift to either world

        }
    }
}


