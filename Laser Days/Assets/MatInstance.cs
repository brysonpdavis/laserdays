using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatInstance : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var r = GetComponent<Renderer>();
        r.material = r.sharedMaterial;
	}

}
