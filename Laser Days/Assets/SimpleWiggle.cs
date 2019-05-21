using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWiggle : MonoBehaviour
{
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        transform.position += new Vector3(Mathf.Sin(Time.fixedTime) * 0.0003f, Mathf.Sin(Time.fixedTime) * 0.0001f, Mathf.Cos(Time.fixedTime) * 0.0005f);

    }
}