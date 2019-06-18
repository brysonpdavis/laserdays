using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerConnector : MonoBehaviour {

    // Speed at which the LineWalker object moves along the path
    public float speed = 0.2f;

    // Transform data of platform, door, or other goop to connect to
    public Transform ConnectionTarget;

    // Array of control points that determine curve of the conncetor path
    public Vector3 conrolPoint;

    // Public vector is in local space for ease of use -> but the walker need world space
    private Vector3 worldSpaceControlPoint;

    // Where to start the path - usually transform position of this goop
    private Vector3 origin;

    // Internal time value used to loop the walker 
    private float internalTime;

    // Object that walks along path - should be empty object with a particle system as its child
    private GameObject walker;

    // Particle system variables to update the look
    private ParticleSystem particles;
    private ParticleSystemRenderer particlesRenderer;

    // Materials to control colors
    private Material goopMat;
    private Material partMat;

    // To store the colors
    private Color restColor;
    private Color activeColor;

    // Determine if this has been initialized
    private bool created;

    // The three states of the particle system
    // Public so that the trigger itself can set state through public methods 
    public enum State {Waiting, Entered, Active}

    // The current state
    private State currentState = State.Waiting;


	// Ininiliaze a connector
	public void CreateConnector() {

        if(ConnectionTarget) 
        {
            origin = this.transform.parent.transform.position;

            walker = transform.GetChild(0).gameObject;

            particles = walker.GetComponent<ParticleSystem>();
            particlesRenderer = walker.GetComponent<ParticleSystemRenderer>();

            worldSpaceControlPoint = this.transform.TransformPoint(conrolPoint);

            created = true;

        } else 
        {
            Debug.Log("Not created because no target transform.");
        }

    }

 
    // Handles the location of the particle emitter based on its state
    private void FixedUpdate () {

        if(created)
        {
            switch (currentState)  {

                case State.Waiting :

                    walker.transform.position = origin;
                    break;

                case State.Active :

                    // End of loop -> Stop emmision -> move to start -> set loop time to 0 -> start emmision
                    if (internalTime >= 1f)
                    {
                        particles.Stop();
                        walker.transform.position = origin;
                        internalTime = 0f;
                        particles.Play();
                    }

                    // Get destination from target -> walk the walker along -> increment time  
                    else
                    {

                        Vector3 destination = ConnectionTarget.position;
                        Debug.Log(destination);
                        walker.transform.position = GetPoint(origin, worldSpaceControlPoint, destination, internalTime);
                        internalTime += Time.fixedDeltaTime * speed;
                    }

                    break;
            }

            Debug.LogWarning(currentState);
        }
	}

    // Gets point along a quadratic bezier curve
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    // Sets state for controlling the FixedUpdate and the look of the particle system
    public void SetState(State s)
    {
        currentState = s;

        switch (s){
            case State.Waiting :
                ChangeColor(restColor);
                break;
            case State.Active :
                ChangeColor(activeColor);
                break;

        }  
    }

    public Color MinColor(float a, Color b)
    {
        return new Color(Mathf.Min(a, b.r), Mathf.Min(a, b.g), Mathf.Min(a, b.b), 1f);
    }

    public Color MaxColor(float a, Color b)
    {
        return new Color(Mathf.Max(a, b.r), Mathf.Max(a, b.g), Mathf.Max(a, b.b), 1f);
    }

    public void ChangeColor(Color col)
    {
        particlesRenderer.material.SetColor("_Color", col);
    }

    public void SetWorld(bool isReal)
    {
        float val = isReal ? 1f : 0f;
        particlesRenderer.material.SetFloat("_Real", val);
    }

    public void SetColors(Color colA, Color colB)
    {
        restColor = colA * 1.5f;
        activeColor = colB * 0.5f;
    }

    // Handles drawing gizmo to visualize the control point in scene view 
    private void DrawGizmo(bool selected)
    {
        // Draw control point gizmo 
        var blue = new Color(0.2f, 0.2f, 0.5f, 1.0f);
        var purple = new Color(0.7f, 0.3f, 0.7f, 1.0f);
        var col = selected ? purple : blue;
        col.a = selected ? 1.0f : 0.25f;
        Gizmos.color = col;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 newCenter = conrolPoint;
        Vector3 newEnd = this.transform.InverseTransformPoint(ConnectionTarget.position);
        Gizmos.DrawSphere(newCenter, 0.2f);
        Gizmos.DrawSphere(newEnd, 0.2f);
        Gizmos.DrawWireSphere(newCenter, 0.2f);
        Gizmos.DrawWireSphere(newEnd, 0.2f);
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
