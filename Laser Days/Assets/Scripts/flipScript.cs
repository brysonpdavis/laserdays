using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipScript : MonoBehaviour {

	private bool space;
	private Vector3 pos;
	private float newX;
	public int envSize;
	public bool inputEnabled;
	


	// Use this for initialization
	void Start () {
		space = true;
		inputEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1") && inputEnabled) {
			Flip();
		}
	}

	void Flip () {
			pos = transform.position;

			if (space) {
				newX = pos.x + envSize;
			}
			else {
				newX = pos.x - envSize;
			}
			transform.position = new Vector3(newX, pos.y, pos.z);
			space = !space;		
	}
}
