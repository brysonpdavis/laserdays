using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Foliage : MonoBehaviour {


    private bool initilalized = false;

    // Use this for initialization
    void Start()


    {
        //MeshRenderer r = GetComponent<MeshRenderer>();

        //if(gameObject.layer == 10)
        //{
        //    r.sharedMaterial = Toolbox.Instance.GetRealFoliageMaterial();
        //} else if
        //(gameObject.layer == 11)
        //{
        //    r.sharedMaterial = Toolbox.Instance.GetLaserFoliageMaterial();
        //}


        if(!initilalized){
            initilalized = true;
            SetRandVertexValue();
        }



      

    }

    private void SetRandVertexValue()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];


        var rx = Random.Range(0f, 1f);
        var ry = Random.Range(0f, 1f);
        for (int i = 0; i < vertices.Length; i++)
            colors[i] = new Color(rx, ry, 0, 0);
            
        mesh.colors = colors;
    }
}
