using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTransitionController : MonoBehaviour {

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
        components = GetComponentsInChildren<LightTransition>();
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


        if (direction) //means player has switched to 'real world': material should transition from current value to zero
        {

            foreach (LightTransition albo in components)
            {
                albo.Flip(true, speed);

            }
        }
        else
        {
            foreach (LightTransition albo in components)
            {
                albo.Flip(false, speed);

            }
        }   
    }
}
