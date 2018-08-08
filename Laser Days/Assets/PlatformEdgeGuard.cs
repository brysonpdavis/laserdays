using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEdgeGuard : MonoBehaviour {

    public bool active = false;
    public PlatformEdgeGuard secondaryTrigger;
    public GameObject associatedCollider;
    public Renderer associatedRenderer;

    private void Start()
    {
        //associatedCollider = GetComponentInChildren<BoxCollider>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Guard"))
        {
            active = true;
            if (active && secondaryTrigger.active ){
                associatedCollider.SetActive(false);
                associatedRenderer.enabled = false;

            }


        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Guard")){

            active = false;
            associatedCollider.SetActive(true);
            associatedRenderer.enabled = true;

        }
    }


}
