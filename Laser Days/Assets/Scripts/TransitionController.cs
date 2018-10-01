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

        player = Toolbox.Instance.GetPlayer();
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

    void LateUpdate() //check to make sure the switch is possible (just as player is doing at the same time)
    {
        bool isFlipped = player.GetComponent<flipScript>().flippedThisFrame;
        bool direction = player.GetComponent<flipScript>().space;
        if (Input.GetMouseButtonDown(0))
        {
            if (isFlipped)
            {
                FlipSurrounding(direction);
            }

        }

        isFlipped = false;
    }

    void FlipSurrounding(bool direction)
    {
        components = GetComponentsInChildren<Transition>();

        if (direction) //means player has switched to 'real world': material should transition from current value to zero
        {

            foreach (Transition albo in components)
            {
                if ((!albo.GetComponentInParent<InteractableObject>()) || !albo.GetComponentInParent<InteractableObject>().selected)
                {
                    //makes sure it doesn't transition the held object either!
                    if (player.GetComponent<MFPP.Modules.PickUpModule>().heldObject)
                    {
                        if (!albo.gameObject.Equals(player.GetComponent<MFPP.Modules.PickUpModule>().heldObject))
                        {
                            albo.Flip(0f, speed);
                        }
                    }

                    else { albo.Flip(0f, speed); }
                }
            }
        }
        else {

            foreach (Transition albo in components)
            {
                if ((!albo.GetComponentInParent<InteractableObject>()) || !albo.GetComponentInParent<InteractableObject>().selected)
                    //makes sure it doesn't transition the held object either!
                if (player.GetComponent<MFPP.Modules.PickUpModule>().heldObject)
                    {
                        if (!albo.gameObject.Equals(player.GetComponent<MFPP.Modules.PickUpModule>().heldObject))
                        {
                            albo.Flip(1f, speed);

                        }
                    }
                    else { albo.Flip(1f, speed); }
            }

        }

    }

    public void PlayerCheck() {

        if (!player) { player = Toolbox.Instance.GetPlayer(); }

        if (player.layer == 16)
        { //if player is starting in RW

            components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in components)
            {
                albo.SetStart(0f);

            }
        }

        else
        {
            //player is in LW
            components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in components)
            {
                albo.SetStart(1f);

            }

        }

    }

}
