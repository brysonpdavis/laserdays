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

    public AudioSource audioSource;
    public AudioClip clickInClip;
    public AudioClip clickOffClip;



    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
        GameObject check = this.transform.parent.transform.parent.transform.parent.transform.gameObject;
        //Debug.Log(check.name);

        //checks to see if is part of a sokoban or not
        if (check.GetComponent<InteractableObject>() && 
            (check.GetComponent<InteractableObject>().objectType == InteractableObject.ObjectType.Sokoban1x1 ||
             check.GetComponent<InteractableObject>().objectType == InteractableObject.ObjectType.Sokoban2x2))
        {
            parent = check;
            isMoveable = true;

            //sets up for sound effects for sokoban when they click on and off to guards
            clickInClip = player.GetComponent<SoundBox>().sokobanClickOn;
            clickOffClip = player.GetComponent<SoundBox>().sokobanClickOff;
        }


        audioSource = GetComponentInParent<AudioSource>();
        //associatedCollider = GetComponentInChildren<BoxCollider>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Guard") && (associatedCollider.gameObject.layer == other.GetComponent<PlatformEdgeGuard>().associatedCollider.gameObject.layer))
        {

            //if the other object isn't the sokoban
            if (!other.GetComponent<PlatformEdgeGuard>().parent )
            {
                //&& other.gameObject.layer == this.gameObject.layer
                //Debug.Log("option 1");
                ClickIn(other);
            }

            //if this object isn't a sokoban
            else if (!parent){
                //Debug.Log("option 2");
                ClickIn(other);
            }

            //if the other object is on the same layer as this one
            else if (other.GetComponent<PlatformEdgeGuard>().parent.layer == parent.layer)
           {
                //Debug.Log("option 3");
                ClickIn(other);
            }
       }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Guard")){

            active = false;
            associatedCollider.SetActive(true);
            associatedRenderer.enabled = true;


            if (secondaryTrigger.active && clickOffClip && (this.gameObject.layer + 5 == player.layer)){
                audioSource.clip = clickOffClip;
                audioSource.Play();
            }


        }
    }

    private void ClickIn (Collider other)
    {


        active = true;
        if (active && secondaryTrigger.active)
        {
            associatedCollider.SetActive(false);
            associatedRenderer.enabled = false;

            //playing sound effect

            if (clickInClip && (this.gameObject.layer+5 == player.layer)){
                audioSource.clip = clickInClip;
                audioSource.Play();  
            }

            if (isMoveable && 
                (player.GetComponent<MFPP.Modules.PickUpModule>().heldObject) &&
                 player.GetComponent<MFPP.Modules.PickUpModule>().heldObject.Equals(parent))
            {
                player.GetComponent<MFPP.Modules.PickUpModule>().PutDown();

            }

        }
    }

}
