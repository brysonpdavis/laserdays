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
    public IList<GameObject> breakingObjectsAbove;
    public IList<GameObject> breakingObjectsBelow;


    public GameObject mainGuard;
    public PlatformController platformController;

    void Start()
    {
        target = null;
        stuckObjects = new List<GameObject>();
        stuckObjectsOffset = new List<Vector3>();
        stuckSokoban = new List<GameObject>();
        breakingObjectsAbove= new List<GameObject>();
        breakingObjectsBelow = new List<GameObject>();


        //main controller is always in guard's parents parent
        platformController = transform.parent.transform.parent.GetComponent<PlatformController>();
    }


    void OnTriggerEnter(Collider col)
    {
        string collisionTag = "Null";
            if (col.CompareTag("Player")) { collisionTag = "Player"; }
            else if (col.GetComponent<InteractableObject>()) { collisionTag = col.GetComponent<InteractableObject>().objectType.ToString(); }
        //string collisionTagParent = "";


        //objects colliding from above travel along
        if ((string.Equals(collisionTag, "Sokoban1x1") || 
             string.Equals(collisionTag, "Sokoban2x2") ||
             string.Equals(collisionTag, "Morph") || 
             string.Equals(collisionTag, "Player"))
            && (col.transform.position.y >= this.transform.position.y))
        {
            stuckObjects.Add(col.gameObject);

        }

        //objects colliding from below make the platform (either single or as group) get stuck
        if ((string.Equals(collisionTag, "Sokoban1x1") || 
             string.Equals(collisionTag, "Sokoban2x2") ||
             string.Equals(collisionTag, "Morph") || 
             string.Equals(collisionTag, "Clickable") ||
             col.tag == "MorphArm")
             && (col.transform.position.y <= this.transform.position.y))
        {
            if (platformController.isGroup)
            {
                foreach (PlatformMover platform in platformController.platformMovers)
                {
                    platform.StopAllCoroutines();
                    //make everything but morphs selectable when platform is stuck
                    platform.PlatformStuckSelectable();
                }
            }

            else {
                GetComponentInParent<PlatformMover>().StopAllCoroutines();
                GetComponentInParent<PlatformMover>().PlatformStuckSelectable();
                stuckSokoban.Add(col.transform.gameObject);
            }
        }
        
        if (string.Equals(collisionTag, "Wall"))
        {
            if (col.transform.position.y <= this.transform.position.y)
            {
                breakingObjectsAbove.Add(col.transform.gameObject);
            }
            else
                breakingObjectsBelow.Add(col.transform.gameObject);

            if (platformController.isGroup)
            {
                foreach (PlatformMover platform in platformController.platformMovers)
                {
                    platform.StopAllCoroutines();
                    //make everything but morphs selectable when platform is stuck
                    platform.PlatformStuckSelectable();
                }
            }

            else
            {
                GetComponentInParent<PlatformMover>().StopAllCoroutines();
                GetComponentInParent<PlatformMover>().PlatformStuckSelectable();
            }


        }
    }

    void OnTriggerStay(Collider col)
    {
        string collisionTag = "Null";
        if (col.CompareTag("Player")) { collisionTag = "Player"; }
        else if (col.GetComponent<InteractableObject>()) {collisionTag = col.GetComponent<InteractableObject>().objectType.ToString();}

        string collisionTagParent = "";
        if (col.transform.parent)
        {
            collisionTagParent = col.transform.parent.tag;
        }

        if (string.Equals(collisionTag, "Sokoban1x1") || 
            string.Equals(collisionTag, "Sokoban2x2") || 
            string.Equals(collisionTag, "Player") || 
            string.Equals(collisionTag, "Clickable") || 
            string.Equals(collisionTag, "NoTouch") || 
            string.Equals(collisionTag, "Morph"))
        {
            if (!(col.gameObject.layer == 23 || col.gameObject.layer == 24))
            {

                if (!col.GetComponent<InteractableObject>() && !string.Equals(collisionTag, "Player"))
                {
                   // target = col.transform.parent.gameObject;
                }
                else if (col.transform.position.y > this.transform.position.y)
                {
                    target = col.gameObject;
                }

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
        string collisionTag = "Null";
        if (col.CompareTag("Player")) { collisionTag = "Player"; }
        else if (col.GetComponent<InteractableObject>()) { collisionTag = col.GetComponent<InteractableObject>().objectType.ToString(); }

        string collisionTagParent = "";

        if (col.transform.parent)
        {
            collisionTagParent = col.transform.parent.tag;
        }

        //object that was jamming the platform has now been unstock and can be removed
        if ((string.Equals(collisionTag, "Sokoban1x1") ||
             string.Equals(collisionTag, "Sokoban2x2") || 
             string.Equals(collisionTag, "Morph") || 
             string.Equals(collisionTag, "Clickable") ||
             col.tag == "MorphArm")
            && (col.transform.position.y <= this.transform.position.y))
        {
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

        if (string.Equals(collisionTag, "Wall"))
        {
            if (col.transform.position.y <= this.transform.position.y)
            {
                breakingObjectsAbove.Remove(col.transform.gameObject);

                if (breakingObjectsAbove.Count == 0)
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
            else
            {
                breakingObjectsBelow.Remove(col.transform.gameObject);
                if (breakingObjectsAbove.Count == 0)
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
        }
        //otherwise the object has been riding on top, so remove it from list of objects that should move with the platform
        else if (string.Equals(collisionTag, "Sokoban1x1") || 
                 string.Equals(collisionTag, "Sokoban2x2") || 
                 string.Equals(collisionTag, "Morph") || 
                 string.Equals(collisionTag, "Player"))
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

        }
    }

}