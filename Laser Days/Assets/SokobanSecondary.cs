using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SokobanSecondary : MonoBehaviour
{

    public GameObject attachedCollider;
    public GameObject player;

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
                other.transform.localPosition = new Vector3 (0, 1, 0);
                player.GetComponent<MFPP.Modules.PickUpModule>().PutDown();
            }
        }
    }





    private void OnTriggerExit(Collider other)
    {
        attachedCollider.SetActive(false);
    }

}
