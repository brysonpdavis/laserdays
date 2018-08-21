using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTrigger : MonoBehaviour
{

    public GameObject SceneToLoad;
    public GameObject backgroundScene;
    private Collider linkedCollider;
    private Collider player;

    private void Start()
    {
        player = Toolbox.Instance.GetPlayer().GetComponent<CharacterController>();
        linkedCollider = GetComponent<BoxCollider>();
        backgroundScene = GameObject.FindWithTag("BackgroundLevel");
    }

    void OnTriggerEnter(Collider collider)
    {
        //when colliding with player, make sure target scene isn't already open, if it's not then load the scene on top (additive) of current scene
        if (collider.tag.Equals("Player"))
        {

            if (SceneToLoad.active == false)
            {
                SceneToLoad.SetActive(true);


                //adds any held object to the scene that the player is entering
                if (player.GetComponentInParent<MFPP.Modules.PickUpModule>().heldObject != null) {

                GameObject held = player.GetComponentInParent<MFPP.Modules.PickUpModule>().heldObject;
                held.transform.parent = SceneToLoad.transform;
                 }

                //can turn off object's renderer
                //MeshRenderer m = this.GetComponentInParent<MeshRenderer>();
                //m.enabled = false;

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
            SceneToLoad.SetActive(false);

               // MeshRenderer m = this.GetComponentInParent<MeshRenderer>();
               // m.enabled = true;

                //old scene method
                //{SceneManager.UnloadSceneAsync(SceneToLoad);

        }
    }

}
