using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTrigger : MonoBehaviour
{

    public GameObject Interior;

    public GameObject backgroundScene;
    private Collider linkedCollider;
    private Collider player;
    private TransitionController[] transitionControllers;
    private FlippableObject[] internalFlippables;


    private void Awake()
    {
        internalFlippables = GetComponentsInChildren<FlippableObject>();

        foreach (FlippableObject obj in internalFlippables)
        {
            obj.sceneContainer = this.transform.GetChild(0);
        }
    }

    private void Start()
    {
        player = Toolbox.Instance.GetPlayer().GetComponent<CharacterController>();
        linkedCollider = GetComponent<BoxCollider>();
        backgroundScene = GameObject.FindWithTag("BackgroundLevel");
        Interior = transform.Find("Interior").gameObject;
        transitionControllers = GetComponentsInChildren<TransitionController>();


        if (!linkedCollider.bounds.Contains(player.transform.position))
        {
            Interior.SetActive(false);
        }

        else
        {
            Interior.SetActive(true);
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        //when colliding with player, make sure target scene isn't already open, if it's not then load the scene on top (additive) of current scene
        if (collider.tag.Equals("Player"))
        {
            //if part of the scene isn't loaded upon enter, load all 3 parts
            if (Interior.active == false)
            {

                foreach (TransitionController transition in transitionControllers) {

                    transition.PlayerCheck();

                }

                Interior.SetActive(true);

            }

        }

    }


    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.Equals(player.gameObject))
        {

            //if player is holding an object while leaving a scene it'll be put in the general scene, unless player brings it into to another scene

            if (player.GetComponent<MFPP.Modules.PickUpModule>().heldObject)
            {
                player.GetComponent<MFPP.Modules.PickUpModule>().PutDown();
                // GameObject held = player.GetComponentInParent<MFPP.Modules.PickUpModule>().heldObject;
                // held.transform.parent = backgroundScene.transform;
            }

            Interior.SetActive(false);

            // MeshRenderer m = this.GetComponentInParent<MeshRenderer>();
            // m.enabled = true;

            //old scene method
            //{SceneManager.UnloadSceneAsync(Interior);

        }
    }

}