using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanBumper : MonoBehaviour {

    public GameObject attachedBumper;


    private void OnTriggerEnter(Collider other)
    {
        attachedBumper.active = false;
    }


    private void OnTriggerExit(Collider other)
    {
        attachedBumper.active = true;
    }

}
