﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	[SerializeField] private int envSize = 100;
	public bool space;
	private PlayerCharge pc;

    //sounds
    private AudioSource audioSource;
    public AudioClip[] flip;
    private AudioClip flipClip;
    public bool flippedThisFrame = false;

	RaycastManager rm;


    private void Awake()
    {
        if (this.gameObject.layer == 16)
        {space = true;}

        if (this.gameObject.layer == 15)
        { space = false; }
    }

    void Start () {

        //make sure player is seeing ladders in correct world
        if (space)
        { GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 262144; } //only see ladders in real world
            else { GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 524288; } //only see ladders in laser world


		pc = GetComponent<PlayerCharge>();
        audioSource = GetComponent<AudioSource>();
		rm = GetComponent<RaycastManager>();
	}

	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject heldObj = GetComponent<MFPP.Modules.PickUpModule>().heldObject;

            if (heldObj) //first check to make sure the object that's held is flippable
            {
                if (heldObj.GetComponent<ItemProperties>().unflippable)
                {
                    GetComponent<MFPP.Modules.PickUpModule>().PutDown();
                    heldObj = null;
                }
            }

			if (heldObj) {
                
				if (pc.Check(heldObj)) { // if the player and any held object can be flipped:
					pc.ItemInteraction(heldObj); // subtract any charge required
					FlipPlayerAndThings(gameObject, heldObj, rm.selectedObjs);
					pc.UpdatePredictingSlider();
                    flippedThisFrame = true;
				}
				else {
					// make a sound effect letting player
					// know that they don't have enough charge
				}
			}
			else {
				if (pc.CheckPlayerCharge())
				{
					pc.PlayerInteraction();
					FlipPlayerAndThings(gameObject, heldObj, rm.selectedObjs);
					pc.UpdatePredictingSlider();
                    flippedThisFrame = true;
				}
			}
		}
	}

	void FlipPlayerAndThings (GameObject player, GameObject held, IList<GameObject> things) {
	
        space = !space;

        //change the layer that the player is on, for changing its collision detection
        if (space)
        { player.layer = 16;  //set player to real world
                GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 262144; //only see ladders in real world
        } 
        else { player.layer = 15; //set player to laser world
            GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 524288; //only see ladders in laser world
        } 


        if (held)
			Flip(held);
		FlipList(things);


        //flipping the ladder raycast
		//Flip(player); //switching layers flips player now.



		// Make sure that the list of objects is flipped before the player
	}

	void Flip (GameObject obj)
	{
        //for objects not being currently held: 
        if (!obj.Equals(GetComponent<MFPP.Modules.PickUpModule>().heldObject)){
            //check which layer the player has moved to, and then change object's layer, shader, and value

                if ( PlayerInLaser() )
                { //if player is now in laser world
                    SetObjectToLaser(obj); //set object to laser layer

                    obj.GetComponent<Renderer>().material.shader = rm.laserWorldShader;  //shader change is now happening in flip script
                    obj.GetComponent<Transition>().SetStart(1f); //set it fully on for laser world
                }
                else if ( PlayerInReal() )
                { //if player is now in real world
                    SetObjectToReal(obj); //set object to real layer

                    obj.GetComponent<Renderer>().material.shader = rm.realWorldShader;  //shader change is now happening in flip script
                    obj.GetComponent<Transition>().SetStart(0f); //set it fully on for real world
                }
            }
        //if the object IS being held, we do the same thing, just change layer without switching the shader (which will get switched on drop)
        else {
            
            if ( PlayerInLaser() )
            { //if player is now in laser world
                SetObjectToLaser(obj); //set object to laser layer
            }

            else if ( PlayerInReal() )
            { //if player is now in real world
                SetObjectToReal(obj); //set object to real layer
            }

        }


	}

	void FlipList (IList<GameObject> objs)
	{
		foreach (GameObject obj in objs)
		{
			Flip(obj);
		}

		if (!Input.GetMouseButton(1))
		{
			foreach (GameObject obj in objs)
			{
				rm.RemoveFromList(obj, true);
			}
			objs.Clear();
		}
	}

	bool PlayerInLaser ()
	{
		return gameObject.layer == 15;
	}
	bool PlayerInReal()
	{
		return gameObject.layer == 16;
	}
	void SetObjectToLaser(GameObject obj)
	{
		obj.layer = 10;
		obj.transform.parent = Toolbox.Instance.GetLaserWorldParent();

        if (obj.CompareTag("Sokoban")){
            GameObject child = obj.transform.GetChild(0).gameObject;
            child.layer = 10;
        }
	}
	void SetObjectToReal(GameObject obj)
	{
		obj.layer = 11;
		obj.transform.parent = Toolbox.Instance.GetRealWorldParent();
 
            if (obj.CompareTag("Sokoban")){
            GameObject child = obj.transform.GetChild(0).gameObject;
            child.layer = 11;
        }


	}
}
