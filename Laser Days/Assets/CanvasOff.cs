using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOff : MonoBehaviour {

    public GameObject canvas;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (canvas.activeInHierarchy)
                canvas.SetActive(false);
            else
                canvas.SetActive(true);
        }
	}
}
