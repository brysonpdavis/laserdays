﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharge : MonoBehaviour {



    public Slider chargeSlider;
    public Text chargeValue;
    public int maxCharge;



    private void Start()
    {
        chargeSlider.maxValue = maxCharge;
        chargeSlider.value = maxCharge;

        chargeValue.text = maxCharge.ToString();


    }


}

