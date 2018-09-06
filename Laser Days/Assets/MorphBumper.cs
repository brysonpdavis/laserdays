using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphBumper : MonoBehaviour {

    public bool colliding = false;
    public MorphArm parentArmTrigger;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() && 
            other.GetComponent<Rigidbody>().isKinematic && 
            !(other.gameObject.tag == "Player"))
        {
            colliding = true;

            if (parentArmTrigger.colliding)
            {
                Debug.Log("stopping");
                parentArmTrigger.parentMorph.StopAllCoroutines();
                parentArmTrigger.parentMorph.morphRunning = false;
                parentArmTrigger.parentMorph.tag = "Clickable";
                parentArmTrigger.parentMorph.GetComponent<Rigidbody>().isKinematic = true;
            }




        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }


}
