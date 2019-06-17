using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBob : MonoBehaviour {

    public bool AnimTest = false;
    public float timeScale = 1f;
    public float magnitude = 0.002f;
    public bool bob = true;
    private Material mat;
	// Use this for initialization
	void Start () {

        if(AnimTest)
        {
            mat = GetComponent<Renderer>().sharedMaterial;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (bob)
        {
            
            transform.position += new Vector3(0f, Mathf.Sin(Time.fixedTime * timeScale) * magnitude, 0f);
        }
        if (AnimTest)
        {
            float e = mat.GetFloat("_Elapsed");
            mat.SetFloat("_Elapsed", e + 0.02f);
        }
	}
}
