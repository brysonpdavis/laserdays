using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphPlatform : MonoBehaviour
{

	private bool on;

	private float lastPos;

	private float deltaPos;

	private GameObject player;

	private CharacterController cc;
	
	public float bounceFactor = 1f;

	void Start ()
	{
		player = Toolbox.Instance.player;

		cc = player.GetComponent<CharacterController>();

		on = false;

	}
	
	
	private void FixedUpdate()
	{
		
		if (on)
		{
			deltaPos = transform.position.y - lastPos;

			lastPos = transform.position.y;

			//player.transform.position += deltaPos > 0 ? new Vector3(0, deltaPos * 3, 0) : Vector3.zero;

			if (deltaPos > 0)
			{
				cc.Move(new Vector3(0, deltaPos * bounceFactor * 0.6f, 0));
			}
			
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			on = true;
			lastPos = transform.position.y;

		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			on = false;
		}
	}
}
