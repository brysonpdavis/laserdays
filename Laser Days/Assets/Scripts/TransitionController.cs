using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController: MonoBehaviour
{
    
    private PlayerCharge pc;
    private GameObject player;
    private float speed;
    private Component[] components;
    public IList<Transition> transitions;
    bool initialized = false;


    private void Awake()
    {
        //if (!initialized)
          //  InitializeSharedMaterials();
    }
    // Use this for initialization
    void Start()
    {
        speed = Toolbox.Instance.globalRealLaserFlipSpeed;

        InitializeList();

        //automatically set up the player and charge that script will be checking
        PlayerCheck();

        player = Toolbox.Instance.GetPlayer();
        pc = player.GetComponent<PlayerCharge>();

        //make sure all materials are starting on correct transition material based on player
        if (player.layer == 16){ //if player is starting in RW

            //components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in transitions)
            {
                albo.SetStart(0f);
            }        
        }

        else {
            //player is in LW
           // components = GetComponentsInChildren<Transition>();
            foreach (Transition albo in transitions)
            {
                albo.SetStart(1f);
            }   
            
        }



    }

    void LateUpdate() //check to make sure the switch is possible (just as player is doing at the same time)
    {
        if (player)
        {
            bool isFlipped = player.GetComponent<flipScript>().flippedThisFrame;

            bool direction = player.GetComponent<flipScript>().space;
            if (isFlipped)
            {
                FlipSurrounding(direction);
                // FlipSharedMaterials(direction);
            }

            isFlipped = false;
        }    
    }

    void FlipSurrounding(bool direction)
    {
        //components = GetComponentsInChildren<Transition>();

            foreach (Transition albo in transitions)
            {
                if ((!albo.GetComponentInParent<InteractableObject>()) || !albo.GetComponentInParent<InteractableObject>().selected)
                {
                    //makes sure it doesn't transition the held object either!
                    if (!Toolbox.Instance.EqualToHeld(this.gameObject))
                    {
                        if (direction)
                            albo.Flip(0f, speed);
                        else
                            albo.Flip(1f, speed);
                      
                    }
                }
            }
    }

    public void PlayerCheck() {

        if (!player) { player = Toolbox.Instance.GetPlayer(); }

        InitializeList();

        //sets ALL contained objects to correct transition state when player enters scene
        Transition[] currentTransitions = GetComponentsInChildren<Transition>();

        if (player.layer == 16)
        { //if player is starting in RW
            
            foreach (Transition albo in currentTransitions)
            {
                if (albo.shared)
                    albo.MaterialSetStart(0f);
                else
                    albo.SetStart(0f);
                Debug.Log("setting to 0f");
            }
        }

        else
        {
            //player is in LW
            foreach (Transition albo in currentTransitions)
            {
                if (albo.shared)
                    albo.MaterialSetStart(1f);
                else
                    albo.SetStart(1f);
                Debug.Log("setting to 1f");

            }

        }

    }

    void InitializeList()
    {
        components = GetComponentsInChildren<Transition>();
        transitions = new List<Transition>();

        foreach (Transition t in components)
        {
            if (t.shared)
                transitions.Add(t);
        }
    }

}
