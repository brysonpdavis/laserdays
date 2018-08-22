using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasEnabler : MonoBehaviour {
    public GameObject canvas;
	// Use this for initialization
	void Awake () {
        canvas.SetActive(true);
    }

}
