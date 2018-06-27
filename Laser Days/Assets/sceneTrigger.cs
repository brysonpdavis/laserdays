using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTrigger : MonoBehaviour
{

    public GameObject SceneToLoad;
    public GameObject backgroundScene;
    public Collider linkedCollider;
    public Collider player;

    void OnTriggerEnter(Collider collider)
    {
        //when colliding with player, make sure target scene isn't already open, if it's not then load the scene on top (additive) of current scene
        if (collider.tag.Equals("Player"))
        {

            if (SceneToLoad.active == false)
            {
                SceneToLoad.active = true;


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
        if (collider.tag.Equals("Player"))
        {
            //when player leaves area, make sure player's bounds are NOT within the 
            //parallel bounds before unloading the current scene
            if (!player.bounds.Intersects(linkedCollider.bounds)){

                //if player is holding an object while leaving a scene it'll be put in the general scene, unless player brings it into to another scene

                if (player.GetComponentInParent<MFPP.Modules.PickUpModule>().heldObject != null)
                {
                    GameObject held = player.GetComponentInParent<MFPP.Modules.PickUpModule>().heldObject;
                    held.transform.parent = backgroundScene.transform;
                }
                SceneToLoad.active = false;

                MeshRenderer m = this.GetComponentInParent<MeshRenderer>();
                m.enabled = true;

                //old scene method
                //{SceneManager.UnloadSceneAsync(SceneToLoad);


            }
        }
    }

}
