using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanShift : MonoBehaviour {
    public GameObject parent;
    public float duration = .2f;
    public Transform snap;
    public GameObject objectToMove;
    public GameObject mainGuard;


    private IEnumerator positionObject;
    private Vector3 objectsPosition;
    private Vector3 moverPosition;


    //moves sokoban onto nearest guard if it's hanging off an edge - either to a sokoban or a 4way guard

    private void Start()
    {


    }

    private void OnTriggerEnter(Collider other)
    {
        //checks to be sure the object triggering the movement is a soko 1x1, what's being triggered is not this trigger's parent sokoban obj
        //and checking to be sure the object being moved in the correct world
        if (other.CompareTag("Clickable") && 
            (other.GetComponent<ItemProperties>().objectType == ItemProperties.ObjectType.Sokoban1x1) &&
            (!other.gameObject.Equals(parent)) && 
            (other.gameObject.layer == parent.layer || other.gameObject.layer + 13 == parent.layer))

           // !((other.gameObject.layer == (parent.layer+1)) || (other.gameObject.layer == (parent.layer - 1))))
        {
            if (Vector3.Distance(other.gameObject.transform.position, parent.transform.position)<=1.52f){
                if (!other.gameObject.GetComponent<ItemProperties>().inMotion)
                {
                    objectToMove = other.gameObject;
                    positionObject = (CenterObjectRoutine());
                    StartCoroutine(positionObject);
                }
            }
        }
    }

    private IEnumerator CenterObjectRoutine()
    {
        //moves the sokoban into place


            objectToMove.GetComponent<ItemProperties>().inMotion = true;
            float elapsedTime = 0;
            float ratio = elapsedTime / duration;
            MFPP.Modules.PickUpModule pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();

        if (pickUp.heldObject && pickUp.heldObject.Equals(objectToMove)){
            pickUp.PutDown();
        }


        //if the parent object has item properties it means that it is a sokoban
        if (parent.GetComponent<ItemProperties>()){
         //   Debug.Log("Sokoban");
            objectToMove.transform.parent = parent.transform;
            objectToMove.GetComponent<BoxCollider>().enabled = false;


            objectsPosition = objectToMove.transform.localPosition;
            moverPosition = snap.localPosition;
            moverPosition.y = objectsPosition.y;

            //do the transition
            while (ratio < 1f)
            {
                elapsedTime += Time.deltaTime;
                ratio = elapsedTime / duration;
                objectToMove.transform.localPosition = Vector3.Slerp(objectsPosition, moverPosition, ratio);
                yield return null;
            }

            objectToMove.GetComponent<ItemProperties>().inMotion = false;
            objectToMove.GetComponent<BoxCollider>().enabled = true;
         //   Debug.Log("done!" + parent.name);

        }

        //for NOT sokoban: aka static obj, we want to use its global position
        else {

            objectToMove.GetComponent<Renderer>().material.SetInt("_IsSelected", 0);

           // Debug.Log("Static");
            objectToMove.transform.parent = parent.transform;
            objectToMove.GetComponent<BoxCollider>().enabled = false;


            objectsPosition = objectToMove.transform.localPosition;
            moverPosition = snap.localPosition;
            moverPosition.y = objectsPosition.y;


            while (ratio < 1f)
            {
                elapsedTime += Time.deltaTime;
                ratio = elapsedTime / duration;
                objectToMove.transform.localPosition = Vector3.Slerp(objectsPosition, moverPosition, ratio);
                yield return null;
            }
            objectToMove.GetComponent<ItemProperties>().inMotion = false;
            objectToMove.GetComponent<BoxCollider>().enabled = true;


           // Debug.Log("done!" + parent.name);
        }


        AudioSource audioSource = GetComponentInParent<AudioSource>();
        
        mainGuard.SetActive(true);

    }


}
