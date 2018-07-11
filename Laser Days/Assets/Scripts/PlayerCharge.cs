using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharge : MonoBehaviour {



    //public Slider chargeSlider;
    //public Slider predictingSlider;
    public Text chargeValue;
    public int maxCharge = 100;
    public int currentCharge;
    public int potentialCharge;
    public int flipCost = 0;
    RaycastManager rm;
    public Image radialSlider;
    public Image predictiveRadial;

    private void Awake()
    {
        rm = GetComponent<RaycastManager>();

        chargeValue.text = maxCharge.ToString();

        currentCharge = maxCharge;
        potentialCharge = maxCharge;
        radialSlider.fillAmount = 1;
        predictiveRadial.fillAmount = 1;


    }

    public void PlayerInteraction() {

        //change current charge amount;
        currentCharge -= flipCost + rm.SumSelectedObjects();

        //change slider to new current charege amt
        radialSlider.fillAmount = ((float)currentCharge / (float)maxCharge);

        currentCharge -= flipCost + rm.SumSelectedObjects();
        chargeValue.text = currentCharge.ToString();

    }

    public bool CheckPlayerCharge() {
        //compare flip costs to total 
        return (flipCost + rm.SumSelectedObjects() <= currentCharge);
            }

    public void ItemInteraction(GameObject item) {
        ItemProperties itemProps = item.GetComponent<ItemProperties>();
        if(itemProps.objectCharge){
            
            //Check to see if it can be activated
            if (itemProps.value + flipCost + rm.SumSelectedObjects() <= currentCharge)
            {
                currentCharge -= (itemProps.value + flipCost + rm.SumSelectedObjects());

                radialSlider.fillAmount = ((float)currentCharge / (float)maxCharge);
                Debug.Log("radial slider = " + radialSlider.fillAmount);

                // for text
                chargeValue.text = currentCharge.ToString();      
            }
        }

        if (itemProps.boost)
        {
            //adds charge value:
            currentCharge += itemProps.value;
            //for slider
            radialSlider.fillAmount = ((float)currentCharge / (float)maxCharge);
             

            //for text
            chargeValue.text = currentCharge.ToString();
            UpdatePredictingSlider();
        }
    }

    // Checks whether the object can be flipped
    public bool Check(GameObject item) {
        ItemProperties ip = item.GetComponent<ItemProperties>();
        return (ip.boost) ? false : (ip.value + flipCost + rm.SumSelectedObjects() <= currentCharge);
    }

    public void UpdatePredictingSlider() {
        int heldValue;
        if (GetComponent<MFPP.Modules.PickUpModule>().heldObject)
            heldValue = GetComponent<MFPP.Modules.PickUpModule>().heldObject.GetComponent<ItemProperties>().value;
        else
            heldValue = 0;
        
       // predictingSlider.value = currentCharge - (flipCost + heldValue + rm.SumSelectedObjects());
        predictiveRadial.fillAmount = (float)(currentCharge - (flipCost + heldValue + rm.SumSelectedObjects())) / (float)maxCharge;
    }
}

