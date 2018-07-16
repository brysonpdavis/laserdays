using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController: MonoBehaviour
{

    public PlayerCharge pc;
    public GameObject player;
    public Component[] components;


    // Use this for initialization
    void Start()
    {
        //make sure all materials are starting on correct transition material based on player
        if (player.layer == 16){ //if player is starting in RW

            components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in components)
            {
                albo.SetStart(0f);

            }        
        }

        else {
            //player is in LW
            components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in components)
            {
                albo.SetStart(1f);

            }   
            
        }

    }

    void LateUpdate() //doing this in lateUpdate so that we know which world player is in AFTER the switch
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject heldObj = player.GetComponent<MFPP.Modules.PickUpModule>().heldObject;
            RaycastManager rm = player.GetComponent<RaycastManager>();
            bool direction = player.GetComponent<flipScript>().space;

            if (heldObj)
            {
                if (pc.Check(heldObj))
                {
                    FlipSurrounding(direction);
                }

            }
            else
            {
                if (pc.CheckPlayerCharge())
                {
                    FlipSurrounding(direction);
                }
            }
        }

    }

    void FlipSurrounding(bool direction)
    {
        components = GetComponentsInChildren<Transition>();

        if (direction) //means player has switched to 'real world': material should transition from current value to zero
        {

            foreach (Transition albo in components)
            {
                if (!albo.GetComponentInParent<ItemProperties>().selected)
                {
                    albo.Flip(0f, .4f);
                }

            }
        }
        else {

            foreach (Transition albo in components)
            {
                if (!albo.GetComponentInParent<ItemProperties>().selected)
                {
                    albo.Flip(1f, .4f); //means player has switched to 'laser world': material should transition from current value to 1
                }
            }

        }

    }
}
