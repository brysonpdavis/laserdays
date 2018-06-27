using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharge : MonoBehaviour {



    public Slider chargeSlider;
    public Slider predictingSlider;
    public Text chargeValue;
    public int maxCharge;
    public int flipCost = 10;
    RaycastManager rm;

    private void Start()
    {
        rm = GetComponent<RaycastManager>();

        chargeSlider.maxValue = maxCharge;
        chargeSlider.value = maxCharge;

        predictingSlider.maxValue = maxCharge;
        predictingSlider.value = maxCharge - flipCost;

        chargeValue.text = maxCharge.ToString();
    }

    public void PlayerInteraction() {
        chargeSlider.value -= flipCost + rm.SumList(rm.selectedObjs);
        chargeValue.text = chargeSlider.value.ToString();
    }

    public bool CheckPlayerCharge() {
        return (flipCost + rm.SumList(rm.selectedObjs) <= chargeSlider.value);
    }

    public void ItemInteraction(GameObject item) {
        ItemProperties itemProps = item.GetComponent<ItemProperties>();
        if(itemProps.objectCharge){
            
            //Check to see if it can be activated
            if (itemProps.value + flipCost + rm.SumList(rm.selectedObjs) <= chargeSlider.value)
            {
                //subtract charge value:
                //for slider
                chargeSlider.value -= itemProps.value + flipCost + rm.SumList(rm.selectedObjs);

                // for text
                chargeValue.text = chargeSlider.value.ToString();      
            }
        }

        if (itemProps.boost)
        {
            //adds charge value:
            //for slider
            chargeSlider.value += itemProps.value;

            //for text
            chargeValue.text = chargeSlider.value.ToString();

            UpdatePredictingSlider();
        }
    }

    // Checks whether the object can be flipped
    public bool Check(GameObject item) {
        ItemProperties ip = item.GetComponent<ItemProperties>();
        return (ip.boost)?false:(ip.value + flipCost + rm.SumList(rm.selectedObjs) <= chargeSlider.value);
    }

    public void UpdatePredictingSlider() {
        int heldValue;
        if (GetComponent<MFPP.Modules.PickUpModule>().heldObject)
            heldValue = GetComponent<MFPP.Modules.PickUpModule>().heldObject.GetComponent<ItemProperties>().value;
        else
            heldValue = 0;
        
        int selectedTotal = 0;
        foreach (GameObject obj in rm.selectedObjs) {
            selectedTotal += obj.GetComponent<ItemProperties>().value;
        }
        predictingSlider.value = chargeSlider.value - (flipCost + heldValue + selectedTotal);
    }
}

