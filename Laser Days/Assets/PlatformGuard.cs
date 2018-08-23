using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps everybody moving with the platform so things don't fall off when it's moving quickly

public class PlatformGuard : MonoBehaviour
{
    public GameObject target = null;
    private Vector3 offset;
    public IList<GameObject> stuckObjects;
    public GameObject mainGuard;

    void Start()
    {
        target = null;
        stuckObjects = new List<GameObject>();
    }


    void OnTriggerEnter(Collider col)
    {
        string collisionTag = col.tag;
        string collisionTagParent = "";

        if (col.transform.parent){
            collisionTagParent = col.transform.parent.tag;
        }

        if (string.Equals(collisionTag, "MorphOn"))
        {
            //Debug.Log("got one!");
            stuckObjects.Add(col.gameObject);
        }


        else if (string.Equals(collisionTag, "Sokoban"))
        {
            stuckObjects.Add(col.transform.gameObject);
        }



    }

    void OnTriggerStay(Collider col)
    {
        string collisionTag = col.tag;
        string collisionTagParent = "";
        if (col.transform.parent)
        {
            collisionTagParent = col.transform.parent.tag;
        }


        if (string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Player") || string.Equals(collisionTag, "Clickable")  || string.Equals(collisionTag, "NoTouch") || string.Equals(collisionTag, "MorphOn")){

            if (!col.GetComponent<ItemProperties>()){
                target = col.transform.parent.gameObject;
            }
            else {
                target = col.gameObject;
            }
            offset = target.transform.position - GetComponentInParent<Transform>().position;
        }


            }

    void OnTriggerExit(Collider col)
    {
        string collisionTag = col.tag;
        string collisionTagParent = "";

        if (col.transform.parent)
        {
            collisionTagParent = col.transform.parent.tag;
        }

        //currently the case for 2x2x1 sokoban which aren't in a container
        if (string.Equals(collisionTag, "Sokoban") )
        {
            stuckObjects.Remove(col.gameObject);
        }

        //for 1x1 sokoban which are in a container, need to check parent
        else if (string.Equals(collisionTagParent, "Sokoban"))
        {
            stuckObjects.Remove(col.transform.parent.gameObject);
        }

        else if (string.Equals(collisionTag, "MorphOn"))
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