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
    public TransitionController realTransitionController;
    public TransitionController laserTransitionController;

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

        //find the transition controllers within the scene
        TransitionController[] controllers = GetComponentsInChildren<TransitionController>();
        foreach (TransitionController control in controllers)
        {
            if (control.gameObject.CompareTag("Laser"))
                laserTransitionController = control;

            else if (control.gameObject.CompareTag("Real"))
                realTransitionController = control;
        }

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
        Transition t = collider.GetComponent<Transition>();

        //when colliding with player, make sure target scene isn't already open, if it's not then load the scene on top (additive) of current scene
        if (collider.tag.Equals("Player"))
        {
            //if part of the scene isn't loaded upon enter, load all 3 parts
            if (Interior.activeInHierarchy == false)
            {

                foreach (TransitionController transition in transitionControllers)
                {

                    transition.PlayerCheck();
                }

                Interior.SetActive(true);

            }

            if (Toolbox.Instance.GetPickUp().heldObject)
            {
                FlippableObject flippable = Toolbox.Instance.GetPickUp().heldObject.GetComponent<FlippableObject>();

                if (collider.gameObject.layer == 10)
                {
                    //add new transform parent / transform list
                    Toolbox.Instance.GetPickUp().heldObject.GetComponent<Collider>().transform.parent = laserTransform;
                    flippable.AddToTransitionList(laserTransitionController);
                }

                else
                {
                    //add to new transform parent / transform list
                    Toolbox.Instance.GetPickUp().heldObject.GetComponent<Collider>().transform.parent = realTransform;
                    flippable.AddToTransitionList(realTransitionController);
                }

                //set its laser/real transforms to this scene
                flippable.laserTransform = laserTransform;
                flippable.realTransform = realTransform;

                //remove from whichever background transitions list it was in
                flippable.RemoveFromTransitionList(laserBackgroundContainer.GetComponentInParent<TransitionController>());
                flippable.RemoveFromTransitionList(realBackgroundContainer.GetComponentInParent<TransitionController>());

                    
            }
        }
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
            Debug.Log("setting flippable! " + collider.gameObject.name);
            SetFlippable(collider);
        }

    }


    //PUTS THE HELD OBJECT IN THE BACKGROUND SCENE
    void SetFlippable(Collider collider)
    {
        Transition t = collider.GetComponent<Transition>();
        FlippableObject flippable = collider.GetComponent<FlippableObject>();
        if (collider.gameObject.layer == 10)
        {
            //set it to the new transform parent
            collider.transform.parent = laserBackgroundContainer;

            //add it to the background scene transition list
            flippable.AddToTransitionList(laserBackgroundContainer.GetComponentInParent<TransitionController>());
        }


        else
        {
            collider.transform.parent = realBackgroundContainer;
            flippable.AddToTransitionList(realBackgroundContainer.GetComponentInParent<TransitionController>());
        }


            //remove it from whichever transition controller it was initially in
            flippable.RemoveFromTransitionList(laserTransitionController);
            flippable.RemoveFromTransitionList(realTransitionController);

            //set its laser/real transforms so it knows which container to switch to
            flippable.laserTransform = laserBackgroundContainer; 
            flippable.realTransform = realBackgroundContainer; 
    }

}