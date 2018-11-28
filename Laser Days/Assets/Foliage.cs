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
        Color[] colors;        

        // create new colors array where the colors will be created.
        if (mesh.colors.Length > 0)
        {

            colors = mesh.colors;
        } else 
        {
            colors = new Color[vertices.Length];
        }




        var rx = Random.Range(0f, 1f);
        var ry = Random.Range(0f, 1f);

        for (int i = 0; i < vertices.Length; i++)
        {

            var nr = (colors[i].r + rx) % 1;
            var ng = (colors[i].g + ry) % 1;

            colors[i] = new Color(nr, ng, 0, 0);
        }  
        mesh.colors = colors;
    }
}
