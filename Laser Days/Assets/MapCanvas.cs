using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCanvas : MonoBehaviour
{
	private Camera mapCam;
	
	// Use this for initialization
	private void Awake()
	{
		foreach (Camera cam in Camera.allCameras )
		{
			if (cam.gameObject.name == "MapCamera")
			{
				mapCam = cam;
			}
		}
	}

	private void Start()
	{
		gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		if (mapCam)
		{
			mapCam.enabled = false;
		}
	}

	private void OnEnable()
	{
		if (mapCam)
		{
			mapCam.enabled = true;
		}
	}
}
