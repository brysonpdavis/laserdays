using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAwake : MonoBehaviour {

	// Use this for initialization
	void Awake ()
	{
		if (Toolbox.Instance)
			Toolbox.Instance.mainCanvas = this.gameObject;
	}
}
