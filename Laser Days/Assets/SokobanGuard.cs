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

        ItemProperties myItemProperties = this.GetComponentInParent<ItemProperties>();
        ItemProperties otherItemProperties = col.GetComponent<ItemProperties>();

        string collisionTag = col.tag;

        if (string.Equals(collisionTag, "Sokoban"))
        {
            col.transform.SetParent(this.transform.parent);
           // col.GetComponent<Rigidbody>().isKinematic = true;
           // col.GetComponent<Rigidbody>().useGravity = false;
        
        }
    }
    void OnTriggerExit(Collider col)
    {
        string collisionTag = col.tag;

        if (string.Equals(collisionTag, "Sokoban"))
        {

            //col.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            player.GetComponent<MFPP.Player>().Movement.AllowMouseMove = true;
            col.transform.SetParent(this.transform.parent.transform.parent);
            target = null;
        }
    }
    void LateUpdate()
    {
        if (target != null)
        {
            target.transform.position = GetComponentInParent<Transform>().position + offset;
        }
    }
}