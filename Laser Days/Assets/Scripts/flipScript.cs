using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	public int envSize;
	
	void Update () {
        if (Input.GetMouseButtonDown(1))
		{
			RaycastManager rm = GetComponent<RaycastManager>();
			GameObject heldObj = rm.heldObject;


			if (heldObj) {
				ItemProperties props = heldObj.GetComponent<ItemProperties>();
				if (props.Check()) {
					props.Interaction();
					Flip(gameObject);
					FlipList(rm.selectedObjs);
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
