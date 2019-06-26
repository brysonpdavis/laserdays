using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFPP;

public class MorphPlatform : MonoBehaviour
{

	private bool on;

	private float lastPos;

	private float deltaPos;

    public Vector3 point;

    private Transform parentTrans;

	private GameObject player;

	private CharacterController cc;

    private Player playerScript;
	
	public float bounceFactor = 1f;

    public float maxMove = 0.5f;

    private float logMaxMove = 0f;

	void Start ()
	{
		player = Toolbox.Instance.player;

		cc = player.GetComponent<CharacterController>();

        playerScript = player.GetComponent<Player>();

        parentTrans = transform.parent;

		on = false;

	}
	
	
	private void FixedUpdate()
	{

        Debug.Log("Platform on is" + on);
		if (on)
		{
			deltaPos = transform.position.y - lastPos;

            lastPos = transform.position.y;

            //player.transform.position += deltaPos > 0 ? new Vector3(0, deltaPos * 3, 0) : Vector3.zero;


            if (deltaPos > 0f)
			{
                float mag = Mathf.Clamp(0.6f * bounceFactor * deltaPos, 0f, maxMove);
                if(mag > logMaxMove)
                {
                    Debug.LogWarning("New Max log: " + mag);
                    logMaxMove = mag;
                }
               //cc.Move(new Vector3(0f, 1f, 0f) * mag);
               
                playerScript.AddImpulse(new Vector3(0f, 1f, 0f) * (deltaPos));
			}
			
		} 

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			on = true;
			lastPos = transform.position.y;

		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			on = false;
        }
	}


    private void DrawGizmo(bool selected)
    {
        var col = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        col.a = selected ? 0.3f : 0.7f;
        Gizmos.color = col;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawSphere(point, 0.2f);
        col.a = selected ? 1.0f : 0.5f;
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
