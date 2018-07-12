using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanRotation : MonoBehaviour {


    public Transform player;



 private void OnTriggerStay(Collider collision)
    {

        if (collision.gameObject.CompareTag("Sokoban"))
        {

            Vector3 transformSokoban = collision.transform.position;
            Vector3 playerPosition = player.position;

            Vector3 playerRelative = collision.transform.InverseTransformPoint(playerPosition);
            Vector3 pivotRelative = collision.transform.InverseTransformPoint(transform.position);


            if (Mathf.Abs(playerRelative.x) >= Mathf.Abs(pivotRelative.x) && ((playerRelative.x*pivotRelative.x)<0))
            {
                collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            }

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Sokoban"))
        {


            collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        }

    }
}
