using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWiggle : MonoBehaviour
{
    public float xMag = 0.0003f;
    public float yMag = 0.0001f;
    public float zMag = 0.0005f;

    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        transform.position += new Vector3(Mathf.Sin(Time.fixedTime) * xMag, Mathf.Sin(Time.fixedTime) * yMag, Mathf.Cos(Time.fixedTime) * zMag);

    }
}