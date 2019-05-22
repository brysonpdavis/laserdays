using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphArm : MonoBehaviour {

    public MorphController parentMorph;
    public MorphBumper linkedBumper;
    private Collider[] internalColliders;
    public bool colliding;
    public IList<Rigidbody> pushedSokoban;


    private void Start()
    {
        pushedSokoban = new List<Rigidbody>();

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
        //if it's a soko or a testercube
        if (parentMorph.morphRunning &&
         other.GetComponent<Sokoban1x1>() ||
         other.GetComponent<BasicClickable>())
        {
            if (other.GetComponent<Sokoban1x1>())
            {
                Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                pushedSokoban.Add(rb);
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                Debug.Log("pushing soko");
            }

        }



        else if (parentMorph.morphRunning &&
            !(other.gameObject.tag == "Player") &&
            (other.gameObject.layer == parentMorph.gameObject.layer || other.gameObject.layer == 17) &&
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
                    parentMorph.GetComponent<Rigidbody>().isKinematic = true;

                }

            }

            else
            {
                parentMorph.StopAllCoroutines();
                parentMorph.morphRunning = false;
                parentMorph.tag = "Clickable";
                parentMorph.GetComponent<Rigidbody>().isKinematic = true;

            }


        }


    }

    public void RefreezeSokoban(Rigidbody rb)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.isKinematic = true;
    }

    public void RefreezeAllSokoban()
    {
        foreach (Rigidbody rb in pushedSokoban)
        {
            RefreezeSokoban(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
        if (pushedSokoban.Contains(other.GetComponent<Rigidbody>()))
        {
            pushedSokoban.Remove(other.GetComponent<Rigidbody>());
            RefreezeSokoban(other.GetComponent<Rigidbody>());
        }
    }

}
