using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Poly2Tri;
using UnityScript.Scripting.Pipeline;
using UnityEditor.ShaderGraph;

public class SokoJet : MonoBehaviour {

    private LineRenderer LR;
    private Vector3[] points;
    private Vector3[] deltas;

    public int numPoints = 4;
    public float TailLength = 1f;
    public float Jitter = 0.01f;

    private Transform parent;
    private Vector3 parPos;
    private Vector3 oldParPos;

    private int frameticker = 0;
    private float bobble = 0f;
	
    // Use this for initialization
	void Start () {
        LR = GetComponent<LineRenderer>();

        parent = transform.parent.transform;
        parPos = parent.position;
        oldParPos = parent.position;

        LR.positionCount = numPoints;
        points = new Vector3[numPoints];
        deltas = new Vector3[numPoints];


        Vector3 init = parent.position + this.transform.position;

        for (int i = 0; i < numPoints; i ++){
            points[i] = new Vector3(0f, 0 - (i * (TailLength/numPoints)), 0f);
            deltas[i] = Vector3.zero;
        }

        LR.SetPositions(points);


	}

    // Update is called once per frame

    void Update()
    {

        {
            parPos = parent.position;
            Vector3 diff = parPos - oldParPos;



            for (int i = deltas.Length - 1; i >= 2; i--)
            {
                deltas[i] = deltas[i - 1];
            }


            deltas[1] = new Vector3(diff.x + Random.Range(-Jitter, Jitter), diff.y, diff.z + Random.Range(-Jitter, Jitter));
            //deltas[1] = diff;




            //points[0] = new Vector3(points[0].x + diff.x, points[0].y, points[0].z + diff.z);


            for (int i = 1; i < numPoints; i++)
            {
                //float x =  - diff.x * i/numPoints;
                //float z =  - diff.z * i/numPoints;
                points[i] = new Vector3(-deltas[i].x, points[i].y, -deltas[i].z);
            }



            LR.SetPositions(points);

            oldParPos = parPos;
        }

    }
}
