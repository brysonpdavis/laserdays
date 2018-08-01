using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanBumper : MonoBehaviour {

    public GameObject attachedSecondary;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.name + ", " + other.gameObject.name);

        if (!other.CompareTag("Player") && (!other.CompareTag("Trigger")) && (!other.gameObject.Equals(this.transform.parent.parent)))
         {
            Debug.Log(this.name + ", " + other.gameObject.name);
            attachedSecondary.SetActive(false);
        }
    }


    private void OnTriggerExit(Collider other)
    {

        Debug.Log(this.name + ", " + other.gameObject.name + "EXIT");

        if (!other.CompareTag("Player") && !other.CompareTag("Trigger") && (!other.gameObject.Equals(this.transform.parent.parent)))
        {

            attachedSecondary.SetActive(true);
        }    
    }

}
