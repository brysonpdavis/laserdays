using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour {

	void OnGUI()
	{
		if(GUI.Button(new Rect(10, 100, 100,30), "Save"))
		{GameController.control.Save();}
		if (GUI.Button(new Rect( 10, 140, 100, 30), "Load"))
		{GameController.control.Load();}
	}
}
