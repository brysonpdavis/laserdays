using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphArm : MonoBehaviour {

    public MorphController parentMorph;
    public MorphBumper linkedBumper;
    private Collider[] internalColliders;
    public bool colliding;


    private void Start()
    {
        //parentMorph = GetComponentInParent<MorphController>();
        internalColliders = parentMorph.GetComponentsInChildren<Collider>();

        foreach (Collider col in internalColliders)
        {
            Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), col);
        }

       //Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), parentMorph.GetComponent<Collider>());

    }

    private void OnTriggerEnter(Collider other)
    {

        if (parentMorph.morphRunning &&
            !(other.gameObject.tag == "Player") &&
            other.gameObject.layer == parentMorph.gameObject.layer &&
            other.gameObject.GetComponent<Rigidbody>() &&
            other.gameObject.GetComponent<Rigidbody>().isKinematic)


        {
            Debug.Log(other.gameObject.name);
            colliding = true;


            if (linkedBumper)
            {
                if (linkedBumper.colliding)
                {
                    Debug.Log("stopping");
                    parentMorph.StopAllCoroutines();
                    parentMorph.morphRunning = false;
                    parentMorph.tag = "Clickable";
                }

            }

            else
            {
                parentMorph.StopAllCoroutines();
                parentMorph.morphRunning = false;
                parentMorph.tag = "Clickable";
            }


        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }

}
