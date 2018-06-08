using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	public int envSize;
	


	// Use this for initialization
	void Start () {
	}

	void Update () {
		if (Input.GetKeyDown("return"))
		{
			GameObject heldObj = GetComponent<RaycastManager>().heldObject;
			if (heldObj) {
				ItemProperties props = heldObj.GetComponent<ItemProperties>();
				if (props.Check()) {
					Flip(gameObject);
					props.Interaction();
				}
				else {
					// make a sound effect letting player 
					// know that they don't have enough charge
				}
			}
			else {
				PlayerCharge playerCharge = GetComponent<PlayerCharge>();
				if (playerCharge.CheckPlayerCharge())
				{
					Flip(gameObject);
					playerCharge.PlayerInteraction();
				}
			}
		}
	}
	
	public void Flip (GameObject obj) 
	{
		// position of gameobject attached to script
		pos = transform.position;

		// if object is in positive space, space = true, else false
		bool space = pos.x > 0 ? true : false;

		// if in the positive space, add envSize to the new X coordinate
		if (space) {
			newX = pos.x - envSize;
		}
		// otherwise subtract it
		else {
			newX = pos.x + envSize;
		}
		transform.position = new Vector3(newX, pos.y, pos.z);
		space = !space;	
	}
}
