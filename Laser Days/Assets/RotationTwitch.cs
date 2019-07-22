using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Policy;

public class RotationTwitch : MonoBehaviour {

    public int onFrames = 1;
    public float rotationScale = 8f;
    public float timeoffset;
    public float timescale;

    private int counter;

    private void Start()
    {
        float f = rotationScale * Mathf.Sin(timescale * (Time.time + timeoffset));
        transform.eulerAngles = new Vector3(f, transform.eulerAngles.y, 0f);
    }

    void Update () {
        counter++;
        if(counter == onFrames)
        {
            float f = rotationScale * Mathf.Sin(timescale * (Time.time + timeoffset));
            transform.eulerAngles = new Vector3(f, transform.eulerAngles.y, 0f);
            counter = 0;
        }
	}
}
