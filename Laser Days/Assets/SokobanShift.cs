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
    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sokoban") && (!other.gameObject.Equals(parent))&& !((other.gameObject.layer == (parent.layer+1)) || (other.gameObject.layer == (parent.layer - 1))))
        {
            //(&& (other.gameObject.layer == parent.layer)) ((was using early, should likely delete))

            Debug.Log("test"+ (Vector3.Distance(other.gameObject.transform.position, parent.transform.position)));
            if (Vector3.Distance(other.gameObject.transform.position, parent.transform.position)<=1.52f){
                objectToMove = other.gameObject;
                positionObject = (CenterObjectRoutine());
                StopAllCoroutines();
                StartCoroutine(positionObject);
            }
        }
    }

    private IEnumerator CenterObjectRoutine()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        if (parent.CompareTag("Sokoban")){
            Debug.Log("Sokoban");
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
            Debug.Log("done!" + parent.name);

        }

        //for NOT sokoban: aka static obj, we want to use its global position
        else {

            Debug.Log("Static");

            objectsPosition = objectToMove.transform.position;
            moverPosition = snap.position;
            moverPosition.y = objectsPosition.y;


            while (ratio < 1f)
            {
                elapsedTime += Time.deltaTime;
                ratio = elapsedTime / duration;
                objectToMove.transform.position = Vector3.Slerp(objectsPosition, moverPosition, ratio);
                yield return null;
            }
            Debug.Log("done!" + parent.name);
        }

       // Vector3 startpoint = objectToMove.transform.position;
       // float elapsedTime = 0;
       // float ratio = elapsedTime / duration;
       // mainGuard.GetComponent<SokobanGuard>().target = null;
       // mainGuard.SetActive(false);



        mainGuard.SetActive(true);

    }


}
