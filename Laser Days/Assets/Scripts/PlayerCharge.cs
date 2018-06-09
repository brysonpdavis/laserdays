using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharge : MonoBehaviour {



    public Slider chargeSlider;
    public Text chargeValue;
    public int maxCharge;
    public int flipCost = 10;

    private void Start()
    {
        chargeSlider.maxValue = maxCharge;
        chargeSlider.value = maxCharge;

        chargeValue.text = maxCharge.ToString();
    }

    public void PlayerInteraction() {
        RaycastManager rm = GetComponent<RaycastManager>();
        chargeSlider.value -= flipCost + rm.SumList(rm.selectedObjs);
        chargeValue.text = chargeSlider.value.ToString();
    }

    public bool CheckPlayerCharge() {
        RaycastManager rm = GetComponent<RaycastManager>();
        return (flipCost + rm.SumList(rm.selectedObjs) <= chargeSlider.value);
    }
}

