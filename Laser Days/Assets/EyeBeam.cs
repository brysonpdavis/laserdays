using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBeam : MonoBehaviour {


    //public float length;

    //private void Update()
    //{
    //    transform.localScale = new Vector3(1f, 1f, length);
    //}

    public void SetLength (float len)
    {
        Vector3 beam = new Vector3(1f, 1f, len);
        transform.localScale = beam;
    }
}
