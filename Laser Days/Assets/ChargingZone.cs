using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingZone : MonoBehaviour
{
    public int chargeRate = 1;
    [SerializeField] private PlayerCharge playerCharge;
    private int i = 1;
    void OnTriggerStay(Collider col)
    {
        if (i % 30 == 0)
        {
            if (col.gameObject.tag == "Player")
            {
                if (playerCharge.chargeSlider.value < playerCharge.maxCharge)
                {
                    playerCharge.chargeSlider.value += chargeRate;
                    playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();

                }
            }
        }
        i++;
    }
}