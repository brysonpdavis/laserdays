using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEdgeGuard : MonoBehaviour {

    public GameObject associatedCollider;

    private void Start()
    {
        //associatedCollider = GetComponentInChildren<BoxCollider>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Guard"))
        {
            associatedCollider.SetActive(false);

        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Guard")){

            associatedCollider.SetActive(true);
        }
    }


}
