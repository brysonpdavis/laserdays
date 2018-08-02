using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanSecondary : MonoBehaviour
{

    private IEnumerator moveHome;


    public GameObject attachedCollider;
    public GameObject player;
    public float time = .1f;

    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
        //attachedCollider = GetComponentInChildren<BoxCollider>().gameObject;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Sokoban") && (!other.gameObject.Equals(this.transform.parent.parent.parent)))
        {
            attachedCollider.SetActive(true);

            //if it's the player that's holding the sokoban that they're trying to pull out the wrong way, they should drop it!

            if (player.GetComponent<MFPP.Modules.PickUpModule>().heldObject.Equals(other.gameObject))
            {
                other.transform.SetParent(this.transform.parent.transform.parent);

                player.GetComponent<MFPP.Modules.PickUpModule>().PutDown();

                moveHome = sendHomeRoutine(other.transform.localPosition, other.gameObject, time);
                StartCoroutine(moveHome);


               
            }
        }
    }





    private void OnTriggerExit(Collider other)
    {
        attachedCollider.SetActive(false);
    }



    private IEnumerator sendHomeRoutine(Vector3 startpoint, GameObject sokoban, float duration)
    {
        Vector3 value;
        Vector3 destination = new Vector3 (0, 1, 0);
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        Debug.Log(sokoban.transform.localPosition.x);
        Debug.Log(sokoban.transform.localScale.x);

        if (sokoban.transform.localScale.x < 1f)
        {
            if (sokoban.transform.localPosition.x > 0.12f) 
            {
                destination.x = 0.25f;
            }

            if (sokoban.transform.localPosition.x < -.12f) {
                destination.x = -.25f;
            }
        }

        while (ratio < 1f)
        {

            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            value = Vector3.Slerp(startpoint, destination, ratio);
            sokoban.transform.localPosition = value;
            yield return null;
        }

    }

}
