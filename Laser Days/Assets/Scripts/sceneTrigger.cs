using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTrigger : MonoBehaviour
{

    private GameObject Interior;

    //public GameObject backgroundScene;
    private Collider linkedCollider;
    private Collider player;
    private TransitionController[] transitionControllers;
    private FlippableObject[] internalFlippables;
    public Transform laserBackgroundContainer;
    public Transform realBackgroundContainer;  

    Transform realTransform;
    Transform laserTransform;

    private void Awake()
    {
        Transform sceneContainer = this.transform.GetChild(1);

        internalFlippables = GetComponentsInChildren<FlippableObject>();

        //sets the scene container for each flippable object in this scene
        foreach (FlippableObject obj in internalFlippables)
        {
            obj.sceneContainer = sceneContainer;

        }

        //gathering the real and laser transforms for this scene
        realTransform = sceneContainer.Find("Real");
        laserTransform = sceneContainer.Find("Laser");
    }

    private void Start()
    {
        player = Toolbox.Instance.GetPlayer().GetComponent<CharacterController>();
        linkedCollider = GetComponent<BoxCollider>();
        Interior = transform.Find("Interior").gameObject;
        transitionControllers = GetComponentsInChildren<TransitionController>();
        realBackgroundContainer = GameObject.FindWithTag("RealBackgroundScene").transform;
        laserBackgroundContainer = GameObject.FindWithTag("LaserBackgroundScene").transform;


        if (!linkedCollider.bounds.Contains(player.transform.position))
        {
            Interior.SetActive(false);
        }

        else
        {
            Interior.SetActive(true);
        }

    }


    //if player or flippables enter a new scene
    void OnTriggerEnter(Collider collider)
    {
        //when colliding with player, make sure target scene isn't already open, if it's not then load the scene on top (additive) of current scene
        if (collider.tag.Equals("Player"))
        {
            //if part of the scene isn't loaded upon enter, load all 3 parts
            if (Interior.activeInHierarchy == false)
            {
                
                foreach (TransitionController transition in transitionControllers) {

                    transition.PlayerCheck();
                }

                Interior.SetActive(true);

            }

            if (Toolbox.Instance.GetPickUp().heldObject)
            {

                if (collider.gameObject.layer == 10)
                    Toolbox.Instance.GetPickUp().heldObject.GetComponent<Collider>().transform.parent = laserTransform;

                else
                    Toolbox.Instance.GetPickUp().heldObject.GetComponent<Collider>().transform.parent = realTransform;

            }
        }

        //flippables' real/laser parent containers need to be set to that of this scene when they enter



    
    }

    //if player or flippables exit whatever scene, flippables need to be set to the background
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.Equals(player.gameObject))
        {
            //if player is holding an object while leaving a scene it'll be put in the general scene, unless player brings it into to another scene

            if (Toolbox.Instance.GetPickUp().heldObject)
            {
                SetFlippable(Toolbox.Instance.GetPickUp().heldObject.GetComponent<Collider>());
            }
            Interior.SetActive(false);
        }


        
        if (collider.GetComponent<FlippableObject>() && !Toolbox.Instance.EqualToHeld(collider.gameObject))
        {
            Debug.Log("my guy!");
            SetFlippable(collider);
        }


    }

    void SetFlippable(Collider collider)
    {
        if (collider.gameObject.layer == 10)
                collider.transform.parent = laserBackgroundContainer;

            else
                collider.transform.parent = realBackgroundContainer;

            collider.GetComponent<FlippableObject>().laserTransform = laserBackgroundContainer; 
            collider.GetComponent<FlippableObject>().realTransform = realBackgroundContainer; 
    }

}