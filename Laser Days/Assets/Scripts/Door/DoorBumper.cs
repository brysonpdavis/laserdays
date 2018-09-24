using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBumper : MonoBehaviour {

    public DoorMover parentMover;
    public int counter = 0; 

    private void Start()
    {
        parentMover = GetComponentInParent<DoorMover>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Clickable"))
        {
            parentMover.jammed = true;
            counter += 1;

            if ((counter >= 1)&& !parentMover.controller.active)
            {
                parentMover.Jam();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Clickable"))
        {
            counter -= 1;
        }

        if (counter == 0)
        {
            parentMover.jammed = false;
            if (!parentMover.controller.active)
            {
                parentMover.Close();
            }
        }
    }



}
