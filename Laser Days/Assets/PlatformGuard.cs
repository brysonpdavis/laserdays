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

    private bool initialized = false;


    void Start()
    {
        target = null;
        stuckObjects = new List<GameObject>();
        stuckObjectsOffset = new List<Vector3>();
        stuckSokoban = new List<GameObject>();
        breakingObjectsAbove= new List<GameObject>();
        breakingObjectsBelow = new List<GameObject>();
        mainGuard = this.gameObject;

        initialized = true;

        //main controller is always in guard's parents parent
        //platformController = transform.parent.transform.parent.GetComponent<PlatformController>();
    }

    private void OnEnable()
    {
        if (initialized)
        {
            stuckObjects.Clear();
            stuckObjectsOffset.Clear();
            stuckSokoban.Clear();
            breakingObjectsAbove.Clear();
            breakingObjectsBelow.Clear();
        }

    }


    void OnTriggerEnter(Collider col)
    {
        string collisionTag = "Null";
            if (col.CompareTag("Player")) { collisionTag = "Player"; }
            else if (col.GetComponent<InteractableObject>()) { collisionTag = col.GetComponent<InteractableObject>().objectType.ToString(); }
        //string collisionTagParent = "";


        //TWO OPTIONS FOR WHEN SOMETHING TRIES TO RIDE ON PLATFORM
        //1:objects colliding from above travel along if there's nothing else on the platform
        //2:objects from above jam the platform if there's already something on it
        if ((string.Equals(collisionTag, "Sokoban1x1") ||
             string.Equals(collisionTag, "FloorBouncer") ||
             string.Equals(collisionTag, "Morph")
             )
            //be sure we're colliding from above
            && (col.transform.position.y >= this.transform.position.y)
            //be sure we're NOT moving
            && !platformController.platformMovers[0].platformIsMoving
           )
        {
                stuckObjects.Add(col.gameObject);
            Debug.Log("ADDING" + col.gameObject.name);
        }

        //always add the player
        if (string.Equals(collisionTag, "Player") && (col.transform.position.y >= this.transform.position.y))
        {
            stuckObjects.Add(col.gameObject);
        }

        //objects colliding from below make the platform (either single or as group) get stuck
        if ((string.Equals(collisionTag, "Sokoban1x1") || 
             string.Equals(collisionTag, "Sokoban2x2") ||
             string.Equals(collisionTag, "Morph") || 
             string.Equals(collisionTag, "Clickable") ||
             string.Equals(collisionTag, "FloorBouncer") ||
             col.tag == "MorphArm")
             && (col.transform.position.y <= this.transform.position.y))
        {

            JamPlatform();
            if (!platformController.isGroup)
                stuckSokoban.Add(col.transform.gameObject);
        }


        //walls go into separate lists for colliding above/below
        if (string.Equals(collisionTag, "Wall") || col.gameObject.CompareTag("Wall") || string.Equals(collisionTag, "Sokoban2x2") )
        {
            Debug.Log("wallhit");
            if (col.transform.position.y <= this.transform.position.y)
                breakingObjectsBelow.Add(col.transform.gameObject);
            
            else
                breakingObjectsAbove.Add(col.transform.gameObject);


        }
    }

    void JamPlatform()
    {
        if (platformController.isGroup)
            {
                foreach (PlatformMover platform in platformController.platformMovers)
                {
                    platform.StopAllCoroutines();
                    //make everything but morphs selectable when platform is stuck
                    platform.PlatformStuckSelectable();
                    platform.platformIsMoving = false;
                }
            }

            else
            {
                GetComponentInParent<PlatformMover>().StopAllCoroutines();
                GetComponentInParent<PlatformMover>().PlatformStuckSelectable();
                GetComponentInParent<PlatformMover>().platformIsMoving = false;
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
            //string.Equals(collisionTag, "Sokoban2x2") || 
            string.Equals(collisionTag, "FloorBouncer") ||
            string.Equals(collisionTag, "Player") || 
            //string.Equals(collisionTag, "Clickable") || 
            string.Equals(collisionTag, "NoTouch") || 
            string.Equals(collisionTag, "Morph"))
        {
            if (!(col.gameObject.layer == 23 || col.gameObject.layer == 24))
            {

                if (!col.GetComponent<InteractableObject>() && !string.Equals(collisionTag, "Player"))
                {
                   // not using player on trigger stay
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

        //object that was jamming the platform from below has now been unstock and can be removed
        if ((string.Equals(collisionTag, "Sokoban1x1") ||
             string.Equals(collisionTag, "Sokoban2x2") || 
             string.Equals(collisionTag, "Morph") || 
             string.Equals(collisionTag, "Clickable") ||
             string.Equals(collisionTag, "FloorBouncer") ||
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
                stuckSokoban.Remove(col.gameObject);



            if (stuckSokoban.Count == 0)
            {
                ReturnPlatformAfterJam();
            }

        }

        if (string.Equals(collisionTag, "Wall") || col.gameObject.CompareTag("Wall") || string.Equals(collisionTag, "Sokoban2x2"))
        {

            //object was colliding from below
            if (col.transform.position.y <= this.transform.position.y)
            {
                breakingObjectsBelow.Remove(col.transform.gameObject);

                if (breakingObjectsBelow.Count == 0)
                {

                    ReturnPlatformAfterJam();
                }
            }

            //object was colliding from above
            else
            {
                //Debug.Log(this.transform.position.y);
                            
                breakingObjectsAbove.Remove(col.transform.gameObject);
                if (breakingObjectsAbove.Count == 0)
                {
                    ReturnPlatformAfterJam();

                }
            }
        }
        //otherwise the object has been riding on top, so remove it from list of objects that should move with the platform
        else if (string.Equals(collisionTag, "Sokoban1x1") || 
                 //string.Equals(collisionTag, "Sokoban2x2") || 
                 string.Equals(collisionTag, "Morph") || 
                 string.Equals(collisionTag, "FloorBouncer") ||
                 string.Equals(collisionTag, "Player"))

        
        {
            stuckObjects.Remove(col.gameObject);

            if (stuckObjects.Count <= 1)
                ReturnPlatformAfterJam();
        }

        target = null;
    }

    void ReturnPlatformAfterJam()
    {
        GameObject check = platformController.platformTriggers[0].gameObject;   //transform.parent.transform.parent.GetComponent<PlatformController>().triggers[0];

        if (!check.GetComponent<PlatformTrigger>().moving)
            check.GetComponent<PlatformTrigger>().MovePlatformToStart();

        else
            check.GetComponent<PlatformTrigger>().MovePlatformToEnd();
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