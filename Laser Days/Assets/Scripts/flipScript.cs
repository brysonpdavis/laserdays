﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	[SerializeField] private int envSize = 100;
	[SerializeField] public bool beginningSpace;
	private bool space;
	
	void Start () {
		space = beginningSpace;
	}

	void Update () {
        if (Input.GetMouseButtonDown(1))
		{
			GameObject heldObj = GetComponent<MFPP.Modules.PickUpModule>().heldObject;
			RaycastManager rm = GetComponent<RaycastManager>();

			if (heldObj) {
				ItemProperties props = heldObj.GetComponent<ItemProperties>();
				if (props.Check()) { // if the player and any held object can be flipped:
					props.Interaction(); // subtract any charge required
					FlipList(rm.selectedObjs); // flip any selected objects
					Flip(gameObject); // flip the player (and consequently any held object)
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
					playerCharge.PlayerInteraction();
					Flip(gameObject);
					FlipList(rm.selectedObjs);
				}
			}
		}
	}
	
	public void Flip (GameObject obj) 
	{
		// position of gameobject attached to script
		pos = obj.transform.position;

		// if in the positive space, add envSize to the new X coordinate
		if (space) {
			newX = pos.x - envSize;
		}
		// otherwise subtract it
		else {
			newX = pos.x + envSize;
		}
		obj.transform.position = new Vector3(newX, pos.y, pos.z);
		space = !space;	
	}

	void FlipList (IList<GameObject> objs)
	{
		foreach (GameObject obj in objs)
		{
			Flip(obj);
			GetComponent<RaycastManager>().RemoveFromList(obj);
		}
		objs.Clear();
	}
}
