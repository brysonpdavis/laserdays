using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is meant to be put on the player gameobject and allows
// the player to interact with narration on screen

public class NarrationControl : MonoBehaviour {

	void Update () {
		if (Toolbox.Instance.GetNarrationActive())
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				Toolbox.Instance.NextNarration();
			}
		}
	}
}
