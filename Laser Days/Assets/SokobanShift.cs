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
        if (other.CompareTag("Sokoban") && (!other.gameObject.Equals(parent)))
        {
            Debug.Log("test"+ (Vector3.Distance(other.gameObject.transform.position, parent.transform.position)));
            if (Vector3.Distance(other.gameObject.transform.position, parent.transform.position)<=1.53f){
                objectToMove = other.gameObject;
                positionObject = (CenterObjectRoutine());
                StartCoroutine(positionObject);
                Debug.Log("true");
            }
        }
    }

    private IEnumerator CenterObjectRoutine()
    {
        objectsPosition = objectToMove.transform.localPosition;
        moverPosition = snap.localPosition;
        moverPosition.y = objectsPosition.y;

       // Vector3 startpoint = objectToMove.transform.position;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
       // mainGuard.GetComponent<SokobanGuard>().target = null;
       // mainGuard.SetActive(false);


        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            objectToMove.transform.localPosition = Vector3.Slerp(objectsPosition, moverPosition, ratio);
            yield return null;
        }
        Debug.Log("done!");
        mainGuard.SetActive(true);

    }


}
