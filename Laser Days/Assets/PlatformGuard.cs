using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps everybody moving with the platform so things don't fall off when it's moving quickly

public class PlatformGuard : MonoBehaviour
{
    public GameObject target = null;
    private Vector3 offset;
    public IList<GameObject> stuckObjects;
    public IList<GameObject> stuckSokoban;
    public GameObject mainGuard;

    void Start()
    {
        target = null;
        stuckObjects = new List<GameObject>();
        stuckSokoban = new List<GameObject>();

    }


    void OnTriggerEnter(Collider col)
    {
        string collisionTag = col.tag;
        //string collisionTagParent = "";


        if (string.Equals(collisionTag, "MorphOn"))
        {
            //Debug.Log("got one!");
           // stuckObjects.Add(col.gameObject);
        }

        if (string.Equals(collisionTag, "Sokoban") && (col.transform.position.y >= this.transform.position.y))
        {
            stuckObjects.Add(col.gameObject);
        }
            

        if ((string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Clickable")) && (col.transform.position.y <= this.transform.position.y))
        {
            Debug.Log("Go");
            GetComponentInParent<PlatformMover>().StopAllCoroutines();
            GetComponentInParent<PlatformMover>().PlatformObjectSelectable();
            stuckSokoban.Add(col.transform.gameObject);
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

            if (!(col.gameObject.layer == 23 || col.gameObject.layer == 24))
            {

                if (!col.GetComponent<ItemProperties>() && !string.Equals(collisionTag, "Player"))
                {
                    target = col.transform.parent.gameObject;
                }
                else if (col.transform.position.y > this.transform.position.y)
                {
                    target = col.gameObject;
                }

                if (target)
                {
                    offset = target.transform.position - GetComponentInParent<Transform>().position;
                }
            }
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

        if ((string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Clickable")) && (col.transform.position.y <= this.transform.position.y))
        {
            //remove the sokoban that's below it from the list
            stuckSokoban.Remove(col.gameObject);
            //if there are no stuck objects beneath it and the platform is not being told to move, it should go to its original place
            if (stuckObjects.Count == 0)
            {
                GameObject check = transform.parent.transform.parent.GetComponent<PlatformController>().triggers[0];

                if (!check.GetComponent<PlatformTrigger>().moving)
                {
                    check.GetComponent<PlatformTrigger>().MovePlatformToStart();

                }

                else 
                {
                    check.GetComponent<PlatformTrigger>().MovePlatformToEnd();
                }
            }


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
        if (target)
        {
            target.transform.position = GetComponentInParent<Transform>().position + offset;
        }
    }
}