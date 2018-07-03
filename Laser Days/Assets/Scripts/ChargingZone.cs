using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingZone : MonoBehaviour
{
    public int chargeRate = 1;
    private PlayerCharge playerCharge;
    private ItemProperties itemprops;
    private int i = 1;
    void OnTriggerStay(Collider col)
    {
        if (i % 30 == 0)
        {
            if (col.gameObject.tag == "Player")
            {
                if (!playerCharge)
                {
                    playerCharge = col.GetComponent<PlayerCharge>();
                }
                if (playerCharge.chargeSlider.value < playerCharge.maxCharge)
                {
                    playerCharge.chargeSlider.value += chargeRate;
                    playerCharge.chargeValue.text = playerCharge.chargeSlider.value.ToString();

                    playerCharge.UpdatePredictingSlider();

                }
            }
            else if (col.gameObject.tag == "Clickable")
            {
                if(!itemprops)
                {
                  itemprops = col.GetComponent<ItemProperties>();
                }
                if (itemprops.reducible == true)
                  {
                  itemprops.touchingzone = true;
                  if (itemprops.value> itemprops.minvalue)
                  {
                  itemprops.value = itemprops.value - 1;
                }
                }
          }
        }
        i++;
    }

    void OnTriggerExit(Collider col)
    {
      if (col.gameObject.tag == "Clickable")
      {
        if(!itemprops)
        {
          itemprops = col.GetComponent<ItemProperties>();
        }
        itemprops.touchingzone = false;
      }
    }
}
