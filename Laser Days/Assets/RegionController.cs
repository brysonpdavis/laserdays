using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionController : MonoBehaviour {

	// Use this for initialization
	void Awake()
	{
		Toolbox.Instance.regionController = this.gameObject;
	} 
}
