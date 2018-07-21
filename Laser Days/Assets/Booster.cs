using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour {

    public int boostAmount = 20;
    GameObject player;


    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
    }


    private void OnTriggerEnter(Collider other)
    {
        MFPP.Modules.PickUpModule pickUp = player.GetComponent<MFPP.Modules.PickUpModule>();

        if (pickUp.heldObject)
        {
            if (pickUp.heldObject.Equals(other.gameObject)){

                pickUp.PutDown();
            }
        }

        other.GetComponent<Rigidbody>().AddForce(transform.up * boostAmount, ForceMode.Impulse);
        Debug.Log(transform.up * boostAmount);

    }
}
