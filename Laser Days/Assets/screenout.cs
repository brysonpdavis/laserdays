using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenout : MonoBehaviour {

    private int count = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            count += 1;
            ScreenCapture.CaptureScreenshot("/Users/seamusedson/Desktop/" + count + ".png", 1);
        }
    }


}
