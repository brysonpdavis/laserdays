using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnCollisionBack : MonoBehaviour {

    public SwitchOnCollision switchParent;
    public bool active = false;
    public bool objCurrentlyInside = false;

    private void Start()
    {
        switchParent = transform.parent.GetComponent<SwitchOnCollision>();
    }

    private void OnTriggerEnter(Collider other)
    {
        active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //objCurrentlyInside = false;
        //if (switchParent.active)
        //{
            //if (!switchParent.objCurrentlyInside)
            //    Attempt(other.gameObject);
            //active = false;
            //switchParent.active = false;
        //}
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    objCurrentlyInside = true;
    //}

    private void Attempt(GameObject obj)
    {
        if (obj.GetComponent<FlippableObject>())
        {
            obj.GetComponent<FlippableObject>().ForcedFlip();
        }

        else if (obj.CompareTag("Player"))
        {
            Toolbox.Instance.GetFlip().FlipAttempt();
        }
    }
}
