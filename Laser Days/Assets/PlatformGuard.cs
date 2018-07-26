using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps everybody moving with the platform so things don't fall off when it's moving quickly

public class PlatformGuard : MonoBehaviour
{
    private GameObject target = null;
    private Vector3 offset;
    void Start()
    {
        target = null;
    }


    void OnTriggerStay(Collider col)
    {
        string collisionTag = col.tag;

        if (string.Equals(collisionTag, "Player") || string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Clickable") ){
            target = col.gameObject;
            offset = target.transform.position - GetComponentInParent<Transform>().position;
        }

            }

    void OnTriggerExit(Collider col)
    {
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