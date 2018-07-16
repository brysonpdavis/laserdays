using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	[SerializeField] private int envSize = 100;
	[SerializeField] public bool beginningSpace;
	public bool space;
	private PlayerCharge pc;

    //sounds
    private AudioSource audioSource;
    public AudioClip[] flip;
    private AudioClip flipClip;


	void Start () {
		space = beginningSpace;
		pc = GetComponent<PlayerCharge>();
        audioSource = gameObject.GetComponent<AudioSource>();
	}

	void Update () {
    if (Input.GetMouseButtonDown(0))
		{
			GameObject heldObj = GetComponent<MFPP.Modules.PickUpModule>().heldObject;
			RaycastManager rm = GetComponent<RaycastManager>();

			if (heldObj) {
				if (pc.Check(heldObj)) { // if the player and any held object can be flipped:
					pc.ItemInteraction(heldObj); // subtract any charge required
					FlipPlayerAndThings(gameObject, heldObj, rm.selectedObjs);
					pc.UpdatePredictingSlider();
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
				}
			}
		}
	}

	void FlipPlayerAndThings (GameObject player, GameObject held, IList<GameObject> things) {
	
        space = !space;

        //change the layer that the player is on, for changing its collision detection
        if (space)
        { player.layer = 16; } //set player to real world
        else { player.layer = 15; } //set player to laser world


        if (held)
			Flip(held);
		FlipList(things);
		//Flip(player); //switching layers flips player now.



		// Make sure that the list of objects is flipped before the player
	}

	void Flip (GameObject obj)
	{

        RaycastManager rm = GetComponent<RaycastManager>();

        //for objects not being currently held: 
        if (!obj.Equals(GetComponent<MFPP.Modules.PickUpModule>().heldObject)){
            //check which layer the player has moved to, and then change object's layer, shader, and value

            GameObject player = this.gameObject;
                if (player.layer == 15)
                { //if player is now in laser world
                    obj.layer = 10; //set object to laser layer

                    //TODO: change its parent gameobject

                    obj.GetComponent<Renderer>().material.shader = rm.laserWorldShader;  //shader change is now happening in flip script
                    obj.GetComponent<Transition>().SetStart(1f); //set it fully on for laser world

                }

                else if (player.layer == 16)
                { //if player is now in real world
                    obj.layer = 11; //set object to laser layer

                    //TODO: change its parent gameobject

                    obj.GetComponent<Renderer>().material.shader = rm.realWorldShader;  //shader change is now happening in flip script
                    obj.GetComponent<Transition>().SetStart(0f); //set it fully on for real world
                }
            }
        //if the object IS being held, we do the same thing, just change layer without switching the shader (which will get switched on drop)
        else {
            
            GameObject player = this.gameObject;
            if (player.layer == 15)
            { //if player is now in laser world
                obj.layer = 10; //set object to laser layer

                //TODO: change its parent gameobject

            }

            else if (player.layer == 16)
            { //if player is now in real world
                obj.layer = 11; //set object to laser layer

                //TODO: change its parent gameobject

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
				GetComponent<RaycastManager>().RemoveFromList(obj, true);
			}
			objs.Clear();
		}
	}
}
