using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObjectMover : MonoBehaviour {

    public bool incorrect = false;
    private IEnumerator positionObject;
    public float duration = .2f;
    public Transform position;
    public GameObject objectToMove;
    public GameObject mainGuard;


    public Vector3 objectsPosition;
    public Vector3 moverPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sokoban") || other.CompareTag("MorphOn")){
            incorrect = true;
            objectToMove = other.gameObject;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sokoban") || other.CompareTag("MorphOn"))
        {
            incorrect = false;
            //objectToMove = null;
        }
    }

    public void centerObject () {
        positionObject = (CenterObjectRoutine());
        StartCoroutine(positionObject);
    }

    private IEnumerator CenterObjectRoutine()
    {
        objectsPosition = objectToMove.transform.position;
        moverPosition = position.position;
        moverPosition.y = objectsPosition.y;
            
        Vector3 startpoint = objectToMove.transform.position;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        mainGuard.GetComponent<PlatformGuard>().target = null;
        mainGuard.SetActive(false);

        while (ratio < 1f)
        {

            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            //value = Vector3.Slerp(startpoint, moverPosition, ratio);
            //objectToMove.transform.position = value;
            objectToMove.transform.position = Vector3.Slerp(startpoint, moverPosition, ratio);

            Debug.Log(objectToMove.transform.position);

            yield return null;
        }
        mainGuard.SetActive(true);
    }
}


