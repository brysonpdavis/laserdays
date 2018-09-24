using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {

    public DoorController controller;
    private int counter = 0;

    private void OnTriggerEnter(Collider other)
    {
        counter += 1;

        if (counter == 1)
        {
            controller.OpenAll();
        }


    }

    private void OnTriggerExit(Collider other)
    {
        counter -= 1;
        if (counter == 0)
        {
            controller.CloseAll();
        }
    }


}
