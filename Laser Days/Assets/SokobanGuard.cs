using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps everybody moving with the platform so things don't fall off when it's moving quickly

public class SokobanGuard : MonoBehaviour
{
    private GameObject target = null;
    private Vector3 offset;
    public GameObject player;
    void Start()
    {
        player = Toolbox.Instance.GetPlayer();
        target = null;
    }


    void OnTriggerStay(Collider col)
    {
        string collisionTag = col.tag;

        if (string.Equals(collisionTag, "Sokoban"))
        {
            if (!col.GetComponent<ItemProperties>().selected)
            {
                if (transform.parent.gameObject.Equals(player.GetComponent<MFPP.Modules.PickUpModule>().heldObject))
                {
                    player.GetComponent<MFPP.Player>().Movement.AllowMouseMove = false;
                }
                target = col.gameObject;
                target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

                offset = target.transform.position - GetComponentInParent<Transform>().position;
            }
        }

    }

    void OnTriggerExit(Collider col)
    {
        target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        player.GetComponent<MFPP.Player>().Movement.AllowMouseMove = true;
        target = null;
    }
    void LateUpdate()
    {
        if (target != null)
        {
            target.transform.position = GetComponentInParent<Transform>().position + offset;
        }
    }
}