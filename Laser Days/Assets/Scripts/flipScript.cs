using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	[SerializeField] private int envSize = 100;
	[SerializeField] public bool beginningSpace;
	private bool space;
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
		if (held)
			Flip(held);
		FlipList(things);
		Flip(player);

		space = !space;
		// Make sure that the list of objects is flipped before the player
	}

	void Flip (GameObject obj)
	{
		// position of gameobject being flipped
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

		if (flip.Length > 0)
		{
			//play sound from range
			int index = Random.Range(0, flip.Length - 1);
			flipClip = flip[index];
			audioSource.clip = flipClip;
			audioSource.Play();
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
				GetComponent<RaycastManager>().RemoveFromList(obj);
			}
			objs.Clear();
		}
	}
}
