using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps everybody moving with the platform so things don't fall off when it's moving quickly

public class PlatformGuard : MonoBehaviour
{
    public GameObject target = null;
    private Vector3 offset;
    public IList<GameObject> stuckObjects;
    public IList<Vector3> stuckObjectsOffset;
    public IList<GameObject> stuckSokoban;
    public GameObject mainGuard;
    public PlatformController platformController;

    void Start()
    {
        target = null;
        stuckObjects = new List<GameObject>();
        stuckObjectsOffset = new List<Vector3>();
        stuckSokoban = new List<GameObject>();

        //main controller is always in guard's parents parent
        platformController = transform.parent.transform.parent.GetComponent<PlatformController>();
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

        if ((string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Player")) && (col.transform.position.y >= this.transform.position.y))
        {
            stuckObjects.Add(col.gameObject);
           // Debug.Log("adding " + col.name);
        }


        if ((string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Clickable")) && (col.transform.position.y <= this.transform.position.y))
        {
            if (platformController.isGroup)
            {
                foreach (PlatformMover platform in platformController.platformMovers)
                {
                    platform.StopAllCoroutines();
                    platform.PlatformObjectSelectable();
                }
            }

            else {
                GetComponentInParent<PlatformMover>().StopAllCoroutines();
                GetComponentInParent<PlatformMover>().PlatformObjectSelectable();
                stuckSokoban.Add(col.transform.gameObject);
            }
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

        if (string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Player") || string.Equals(collisionTag, "Clickable") || string.Equals(collisionTag, "NoTouch") || string.Equals(collisionTag, "MorphOn"))
        {
            if (!(col.gameObject.layer == 23 || col.gameObject.layer == 24))
            {

                if (!col.GetComponent<ItemProperties>() && !string.Equals(collisionTag, "Player"))
                {
                   // target = col.transform.parent.gameObject;
                }
                else if (col.transform.position.y > this.transform.position.y)
                {
                    target = col.gameObject;
                }


                /*
                if (target)
                {
                    offset = target.transform.position - GetComponentInParent<Transform>().position;
                }
                */

                //run through all stuck objects, get offset value for each of them
                stuckObjectsOffset.Clear();
                for (int i = 0; i < stuckObjects.Count; i++)
                {
                    stuckObjectsOffset.Add(stuckObjects[i].transform.position - GetComponentInParent<Transform>().position);

                    // target.transform.position = GetComponentInParent<Transform>().position + offset;
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
            //Debug.Log("trying to unstick");


            if (platformController.isGroup)
            {
                foreach (PlatformMover platform in platformController.platformMovers)
                {
                    platform.mainGuard.GetComponent<PlatformGuard>().stuckSokoban.Remove(col.gameObject);
                }
            }
            else 
                {
                    stuckSokoban.Remove(col.gameObject);
                }


            if (stuckSokoban.Count == 0)
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
        else if (string.Equals(collisionTag, "Sokoban") || string.Equals(collisionTag, "Player"))
        {
            stuckObjects.Remove(col.gameObject);
        }

        else if (string.Equals(collisionTag, "MorphOn"))
        {
            stuckObjects.Remove(col.gameObject);
        }



        target = null;
    }



    void LateUpdate()
    {

        for (int i = 0; i < stuckObjects.Count; i++)
        {
            //set the offset for each obj
            stuckObjects[i].transform.position = GetComponentInParent<Transform>().position + stuckObjectsOffset[i];
           // Debug.Log("Stuck " + stuckObjects[i].name);


            // target.transform.position = GetComponentInParent<Transform>().position + offset;
        }
        // target.transform.position = GetComponentInParent<Transform>().position + offset;
    }

}