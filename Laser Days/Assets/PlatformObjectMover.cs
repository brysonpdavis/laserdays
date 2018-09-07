﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObjectMover : MonoBehaviour {

    public bool incorrect = false;
    private IEnumerator positionObject;
    public float duration = .2f;
    public Transform position;
    public GameObject objectToMove;
    public GameObject mainGuard;
    public bool motionCheck;
    private MFPP.Modules.PickUpModule pickUp;


    public Vector3 objectsPosition;
    public Vector3 moverPosition;

    private void Start()
    {
        pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();

    }

    private void OnTriggerEnter(Collider other)
    {
        ItemProperties.ObjectType type = ItemProperties.ObjectType.Null;
        if (other.GetComponent<ItemProperties>()) { type = other.GetComponent<ItemProperties>().objectType; } 

        if ((type == ItemProperties.ObjectType.Sokoban1x1 || 
             type == ItemProperties.ObjectType.Sokoban2x2 || 
             type == ItemProperties.ObjectType.Morph)
             && (other.gameObject.layer == mainGuard.layer))
        {
             //   Debug.Log("here we go");
                incorrect = true;

            objectToMove = other.gameObject;

            if (motionCheck && mainGuard.GetComponent<PlatformGuard>().target && mainGuard.GetComponent<PlatformGuard>().target.Equals(objectToMove))
            {
                
                    positionObject = (CenterObjectRoutine());
                    StopAllCoroutines();
                    StartCoroutine(positionObject);

            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        ItemProperties.ObjectType type = ItemProperties.ObjectType.Null;
        if (other.GetComponent<ItemProperties>()) { type = other.GetComponent<ItemProperties>().objectType; } 

        if (type == ItemProperties.ObjectType.Sokoban1x1 ||
            type == ItemProperties.ObjectType.Sokoban2x2 ||
            type == ItemProperties.ObjectType.Morph)
        {
            incorrect = false;
            //objectToMove = null;
        }
    }

    public void centerObject () {
        positionObject = (CenterObjectRoutine());
        if (mainGuard.GetComponent<PlatformGuard>().target && mainGuard.GetComponent<PlatformGuard>().target.Equals(objectToMove)){
            objectToMove.tag = ("NoTouch");
            StopAllCoroutines();
            StartCoroutine(positionObject);
        }
    }

    private IEnumerator CenterObjectRoutine()
    {


        if (objectToMove.GetComponent<ItemProperties>().objectType == ItemProperties.ObjectType.Sokoban2x2)
        {

            yield return new WaitForFixedUpdate();
            mainGuard.GetComponent<PlatformGuard>().platformController.StopPlatforms();
            
        }

        else
        {


            if (pickUp.heldObject && pickUp.heldObject.Equals(objectToMove))
            {
                pickUp.PutDown();
                pickUp.target = null;
                yield return new WaitForFixedUpdate();
            }


            //Debug.Log(objectToMove.GetComponent<ItemProperties>().objectType.ToString());
            objectsPosition = objectToMove.transform.position;


            float differenceY = objectsPosition.y - position.position.y;

            Vector3 startpoint = objectToMove.transform.position;
            float elapsedTime = 0;
            float ratio = elapsedTime / duration;
            float startGuardY = mainGuard.transform.parent.transform.position.y;
            Debug.Log(startGuardY);

            PlatformGuard platformGuard = mainGuard.GetComponent<PlatformGuard>();
            int index = platformGuard.stuckObjects.IndexOf(objectToMove);
            //Debug.Log("stuck obj: " + platformGuard.stuckObjects.Count);


            platformGuard.stuckObjects.Remove(objectToMove);
            platformGuard.stuckObjectsOffset.Remove(objectToMove.transform.position - mainGuard.transform.position);
            mainGuard.SetActive(false);




            while (ratio < 1f)
            {
                moverPosition = position.transform.position;
                moverPosition.y = moverPosition.y+differenceY;


                elapsedTime += Time.smoothDeltaTime;
                ratio = elapsedTime / duration;
                //value = Vector3.Slerp(startpoint, moverPosition, ratio);
                //objectToMove.transform.position = value;
                objectToMove.transform.position = Vector3.Slerp(startpoint, moverPosition, ratio);

                // Debug.Log(objectToMove.transform.position);

                yield return null;
            }


            float newGuardY = mainGuard.transform.parent.transform.position.y - startGuardY;
            Debug.Log(newGuardY);

            Vector3 finalPosition = new Vector3(position.position.x, (position.position.y + differenceY ), position.position.z);
            objectToMove.transform.position = finalPosition;

            mainGuard.SetActive(true);

            //platformGuard.stuckObjects.Add(objectToMove);  [was adding this before, but realized that the platformGuard will be doing this already]
            platformGuard.stuckObjectsOffset.Add((objectToMove.transform.position - mainGuard.transform.position));
            incorrect = false;
            StopAllCoroutines();
        }
    }
}


