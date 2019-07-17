using System.Collections;
using System.Collections.Generic;
using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventSystem))]

public class FirstSelectedFix : MonoBehaviour {


	public static void OpenMenu()
	{
		EventSystem es = EventSystem.current;

		GameObject temp = es.firstSelectedGameObject;

		es.SetSelectedGameObject(null);
		
		es.SetSelectedGameObject(temp);
		
		temp.GetComponent<Button>().OnSelect(null);

		RewiredStandaloneInputModule rsim = es.GetComponent<RewiredStandaloneInputModule>();

		if (rsim)
		{
			if (ControlManager.GetControllerState() == ControlManager.ControllerState.KeyboardAndMouse)
				rsim.allowMouseInput = true;
			else
				rsim.allowMouseInput = false;
		}

	}

}
