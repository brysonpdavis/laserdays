using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSokobanCheck : MonoBehaviour {
    

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Sokoban"))
        {
            //if (GetComponent<Collider>().bounds.Contains(collision.bounds.extents)){

            if (collision.gameObject.GetComponent<Rigidbody>().useGravity)
            {
                collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
                collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }

            } 
        }
    }
