using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is meant to be put on the player gameobject and allows
// the player to interact with narration on screen

public class NarrationControl : MonoBehaviour
{

	[SerializeField] private GameObject map;

	private void Start()
	{
		map = Toolbox.Instance.mainCanvas.transform.Find("Map").gameObject;
	}

	void Update () {
		if (Toolbox.Instance.GetNarrationActive())
		{
			if (Input.GetKeyDown(ControlManager.CM.submit))
			{
				Toolbox.Instance.NextNarration();
			}
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			if (map.activeSelf)
			{
				map.SetActive(false);
			}
			else
			{
				map.SetActive(true);
			}
		}
	}
}
