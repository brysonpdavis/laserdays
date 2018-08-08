using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEdgeGuard : MonoBehaviour {

    public bool active = false;
    public PlatformEdgeGuard secondaryTrigger;
    public GameObject associatedCollider;
    public Renderer associatedRenderer;
    public bool isMoveable;
    public GameObject parent;
    public GameObject player;

    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
        GameObject check = this.transform.parent.transform.parent.transform.parent.transform.gameObject;
        if (check.CompareTag("Sokoban")){
            parent = check;
            isMoveable = true;
        }
        //associatedCollider = GetComponentInChildren<BoxCollider>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Guard"))
        {
            
            active = true;
            if (active && secondaryTrigger.active ){
                associatedCollider.SetActive(false);
                associatedRenderer.enabled = false;

                if (isMoveable && (player.GetComponent<MFPP.Modules.PickUpModule>().heldObject) && player.GetComponent<MFPP.Modules.PickUpModule>().heldObject.Equals(parent)){
                    player.GetComponent<MFPP.Modules.PickUpModule>().PutDown();
                    //PLAY SOUND HERE
                }

            }


        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Guard")){

            active = false;
            associatedCollider.SetActive(true);
            associatedRenderer.enabled = true;

        }
    }


}
