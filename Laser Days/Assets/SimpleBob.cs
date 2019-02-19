using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBob : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position += new Vector3(0f, Mathf.Sin(Time.fixedTime) * 0.002f, 0f);
	}
}
