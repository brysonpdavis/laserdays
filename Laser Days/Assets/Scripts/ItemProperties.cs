using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    [Header("Your Clickables")]

    public string itemName;

    [SerializeField] private bool objectCharge = true;
    [SerializeField] private bool boost;

    [SerializeField] public int value;

    [SerializeField] private PlayerCharge playerCharge;

    public void Interaction()
    {
        if(objectCharge){
            
            //Check to see if it can be activated
            if (value <= playerCharge.chargeSlider.value)
            {
                //subtract charge value:
                //for slider
                playerCharge.chargeSlider.value -= value;

              
                playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();



                //want something here to change the object's shader, showing that it can shift to either world


                //turn on the switching script
                this.GetComponent<flipScript>().enabled = true;

            }
            }


        if (boost)
        {

            //Check to see if it can be activated
            if (value <= playerCharge.chargeSlider.value)
            {
                //subtract charge value:
                //for slider
                playerCharge.chargeSlider.value += value;


                playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();



                //want something here to change the object's shader, showing that it can shift to either world


                //turn on the switching script
                this.GetComponent<flipScript>().enabled = true;

            }
        }







    }



}


