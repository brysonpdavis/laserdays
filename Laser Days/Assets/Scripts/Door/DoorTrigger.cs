using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {

    public DoorController controller;
    public bool active = false;
    private int counter = 0;

    private void Awake()
    {

        if (!controller)
            controller = GetComponentInParent<DoorController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        counter += 1;
        active = true;

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
            active = false;

            if (CheckOtherTriggers())
                controller.CloseAll(); 

        }
    }

    private bool CheckOtherTriggers ()
    {
        int triggerCheck = 0;
        foreach (DoorTrigger trig in controller.doorTriggers)
        {
            if (trig.active)
                triggerCheck += 1;
        }

        if (triggerCheck == 0)
            return true;

        else return false;
            
    }


}
