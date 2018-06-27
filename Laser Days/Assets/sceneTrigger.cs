﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTrigger : MonoBehaviour
{

    public GameObject SceneToLoad;
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

                //old scene method:
                //SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
           
                //can turn off object's renderer. right now it causes a flicker, so currently off.

                MeshRenderer m = this.GetComponentInParent<MeshRenderer>();
                m.enabled = false;

            }
        }
    }


    void OnTriggerExit(Collider collider)
    {
        //when player leaves area, make sure player's bounds are NOT within the parallel bounds before unloading the current scene
        if (collider.tag.Equals("Player"))
        {
            if (!player.bounds.Intersects(linkedCollider.bounds)){

                SceneToLoad.active = false;

                MeshRenderer m = this.GetComponentInParent<MeshRenderer>();
                m.enabled = true;

                //old scene method
                //{SceneManager.UnloadSceneAsync(SceneToLoad);


            }
        }
    }

}
