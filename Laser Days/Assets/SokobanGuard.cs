using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps everybody moving with the platform so things don't fall off when it's moving quickly

public class SokobanGuard : MonoBehaviour
{
    public GameObject target = null;
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

        if (otherItemProperties && otherItemProperties.objectType == ItemProperties.ObjectType.Sokoban1x1)
        {
            col.GetComponent<Rigidbody>().isKinematic = false;
            col.transform.SetParent(this.transform.parent);
        }
    }
    void OnTriggerExit(Collider col)
    {
        string collisionTag = col.tag;

        if (col.gameObject.GetComponent<ItemProperties>() && col.gameObject.GetComponent<ItemProperties>().objectType == ItemProperties.ObjectType.Sokoban1x1)
        {
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