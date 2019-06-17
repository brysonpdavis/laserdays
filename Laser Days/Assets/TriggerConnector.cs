using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEngine.Experimental.GlobalIllumination;

public class TriggerConnector : MonoBehaviour {

    public float speed = 0.2f;
    private float internalTime;

    private Vector3 start;
    private Vector3 dest;
    public Vector3 CenterConrolPoint;

    private Transform[] platformTransforms;

    private LineRenderer lr;
    private ParticleSystem ps;

    private ParticleSystemRenderer psR;

    private GameObject lineWalker;
    private Material goopMat;
    private Material partMat;

    private Color restColor;
    private Color activeColor;

    private bool created;

    public enum State {Waiting, Entered, Active}

    private State currentState = State.Waiting;


	// Use this for initialization
	public void CreateConnector (PlatformMover[] platforms) {

        start = this.transform.parent.transform.position;
        platformTransforms = new Transform[platforms.Length];

        for (int i = 0; i < platforms.Length; i++)
        {
            platformTransforms[i] = platforms[i].transform;
        }

        dest = platformTransforms[0].position;

        lr = GetComponent<LineRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        psR = GetComponentInChildren<ParticleSystemRenderer>();

        changeColor(restColor);

        lineWalker = transform.GetChild(0).gameObject;

        created = true;
    }

    [ExecuteInEditMode ]


	// Update is called once per frame
	void Update () {

        if(created)
        {
            switch (currentState)  {

                case State.Waiting :

                    lineWalker.transform.position = start;
                    break;

                case State.Active :

                    // Stop emmision -> move to start -> set loop time to 0 -> start emmision
                    if (internalTime >= 1f)
                    {
                        ps.Stop();
                        lineWalker.transform.position = start;
                        internalTime = 0f;
                        ps.Play();

                    }
                    // Get destination and center -> set position ->  
                    else
                    {
                        dest = platformTransforms[0].position;
                        Vector3 center = this.transform.TransformPoint(CenterConrolPoint);
                        lineWalker.transform.position = GetPoint(start, center, dest, internalTime);
                        internalTime += Time.fixedDeltaTime * speed;
                    }

                    break;
            }
        }
	}

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    Vector3 Walk(Vector3 origin, Vector3 mid, Vector3 destination, float t)
    {
        t = Mathf.Clamp01(t);
        return Vector3.Lerp(origin, destination, t);
    }


    public void SetState(State s)
    {
        currentState = s;

        switch (s){
            case State.Waiting :
                changeColor(restColor);
                break;
            case State.Active :
                changeColor(activeColor);
                break;

        }  
    }

    private void changeColor(Color col)
    {
        psR.material.SetColor("_Color", col);
    }

    public void SetColors(Color colA, Color colB)
    {
        restColor = colA;
        activeColor = colB;
    }

    private void DrawGizmo(bool selected)
    {
        var col = new Color(0.2f, 0.2f, 0.7f, 1.0f);
        col.a = selected ? 0.3f : 0.7f;
        Gizmos.color = col;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 newCenter = CenterConrolPoint;
        Gizmos.DrawSphere(newCenter, 0.2f);
        col.a = selected ? 1.0f : 0.5f;
        Gizmos.color = col;
        Gizmos.DrawWireSphere(newCenter, 0.2f);
    }

    public void OnDrawGizmos()
    {
        DrawGizmo(false);
    }
    public void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }
}
