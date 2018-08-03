﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps everybody moving with the platform so things don't fall off when it's moving quickly

public class PlatformGuard : MonoBehaviour
{
    private GameObject target = null;
    private Vector3 offset;
    public IList<GameObject> stuckObjects;

    void Start()
    {
        target = null;
        stuckObjects = new List<GameObject>();
    }


    void OnTriggerEnter(Collider col)
    {
        string collisionTag = col.tag;

        if (string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "MorphOn"))
        {

            stuckObjects.Add(col.gameObject);
        }


    }

    void OnTriggerStay(Collider col)
    {
        string collisionTag = col.tag;

        if (string.Equals(collisionTag, "Player") || string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Clickable")  || string.Equals(collisionTag, "NoTouch")){
            target = col.gameObject;
            offset = target.transform.position - GetComponentInParent<Transform>().position;
        }


            }

    void OnTriggerExit(Collider col)
    {
        string collisionTag = col.tag;

        if (string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "MorphOn"))
        {
            stuckObjects.Remove(col.gameObject);
        }


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