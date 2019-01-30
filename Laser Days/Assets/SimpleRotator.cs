using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{


    public float rotX;
    public float rotY;
    public float rotZ;

    private float add;

    private Material mat;
   


    // Use this for initialization
    void Start()
    {


        //rotX = 0f;
        //rotY = 10f;
        //rotZ = 0f;
        mat = GetComponent<Renderer>().material;
        mat.SetFloat("_c",0f);
        add = Mathf.PI  / 180;

    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(rotX * Time.deltaTime, rotY * Time.deltaTime, rotZ * Time.deltaTime);
        
        mat.SetFloat("_c", mat.GetFloat("_c") + add);

    }
}