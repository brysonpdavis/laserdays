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
    public bool motionCheck;


    public Vector3 objectsPosition;
    public Vector3 moverPosition;


    private void OnTriggerEnter(Collider other)
    {


        if ((other.CompareTag("Sokoban") || other.CompareTag("MorphOn")) && (other.gameObject.layer == mainGuard.layer)){
                Debug.Log("here we go");
                incorrect = true;

            objectToMove = other.gameObject;
            if (motionCheck && mainGuard.GetComponent<PlatformGuard>().target && mainGuard.GetComponent<PlatformGuard>().target.Equals(objectToMove))
            {
                positionObject = (CenterObjectRoutine());
                StartCoroutine(positionObject);
                Debug.Log("GO!");
            }
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
        if (mainGuard.GetComponent<PlatformGuard>().target && mainGuard.GetComponent<PlatformGuard>().target.Equals(objectToMove)){
            objectToMove.tag = ("NoTouch");
            StartCoroutine(positionObject);
        }
    }

    private IEnumerator CenterObjectRoutine()
    {

        if (string.Equals(objectToMove.GetComponent<ItemProperties>().objectType.ToString(), "Sokoban2x2"))
        {

            yield return new WaitForFixedUpdate();
            mainGuard.GetComponent<PlatformGuard>().platformController.StopPlatforms();
            
        }

        else
        {
            Debug.Log(objectToMove.GetComponent<ItemProperties>().objectType.ToString());
            objectsPosition = objectToMove.transform.position;
            moverPosition = position.position;
            moverPosition.y = objectsPosition.y;

            Vector3 startpoint = objectToMove.transform.position;
            float elapsedTime = 0;
            float ratio = elapsedTime / duration;

            PlatformGuard platformGuard = mainGuard.GetComponent<PlatformGuard>();
            int index = platformGuard.stuckObjects.IndexOf(objectToMove);
            //Debug.Log("stuck obj: " + platformGuard.stuckObjects.Count);


            platformGuard.stuckObjects.Remove(objectToMove);
            platformGuard.stuckObjectsOffset.Remove(objectToMove.transform.position - mainGuard.transform.position);
            mainGuard.SetActive(false);

            while (ratio < 1f)
            {

                elapsedTime += Time.deltaTime;
                ratio = elapsedTime / duration;
                //value = Vector3.Slerp(startpoint, moverPosition, ratio);
                //objectToMove.transform.position = value;
                objectToMove.transform.position = Vector3.Slerp(startpoint, moverPosition, ratio);

                // Debug.Log(objectToMove.transform.position);

                yield return null;
            }
            mainGuard.SetActive(true);

            platformGuard.stuckObjects.Add(objectToMove);
            platformGuard.stuckObjectsOffset.Add((objectToMove.transform.position - mainGuard.transform.position));
            incorrect = false;
        }
    }
}


