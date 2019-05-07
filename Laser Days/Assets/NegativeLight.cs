using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeLight : MonoBehaviour {

    public Vector4 c = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
    public float intensity = 1f;
	// Use this for initialization
	void Start () {
        var lite = GetComponent<Light>();
        lite.color = c * intensity;
	}
	
	// Update is called once per frame
    [ExecuteInEditMode]
	void Update () {
        var lite = GetComponent<Light>();
        lite.color = c * intensity;
    }
}
