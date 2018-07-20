using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController: MonoBehaviour
{
    
    private PlayerCharge pc;
    private GameObject player;
    public float speed = .4f;
    private Component[] components;


    // Use this for initialization
    void Start()
    {
        //automatically set up the player and charge that script will be checking

        player = GameObject.FindWithTag("Player");
        pc = player.GetComponent<PlayerCharge>();

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

    void Update() //check to make sure the switch is possible (just as player is doing at the same time)
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
                    FlipSurrounding(!direction); //using !direction because we're using the opposite of where player currently is
                }

            }
            else
            {
                if (pc.CheckPlayerCharge())
                {
                    FlipSurrounding(!direction); //using !direction because we're using the opposite of where player currently is
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
                if ((!albo.GetComponentInParent<ItemProperties>()) || !albo.GetComponentInParent<ItemProperties>().selected)
                {
                    albo.Flip(0f, speed);
                }

            }
        }
        else {

            foreach (Transition albo in components)
            {
                if ((!albo.GetComponentInParent<ItemProperties>()) || !albo.GetComponentInParent<ItemProperties>().selected)
                {
                    albo.Flip(1f, speed); //means player has switched to 'laser world': material should transition from current value to 1
                }
            }

        }

    }
}
