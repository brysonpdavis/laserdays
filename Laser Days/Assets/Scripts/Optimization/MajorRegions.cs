using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MajorRegions : MonoBehaviour
{

	public bool active = true;
	
	public List<RegionOptimization> childrenRegions;

	private float timeSinceCheck = 0f;

	public float checkInterval;

	public float activateDistance = 100;

	private bool childrenActive;

	private GameObject player;

	private static List<MajorRegions> allRegions;

	private void Awake()
	{
		MakeListAndAdd();
	}

	private void Start()
	{
		
		player = Toolbox.Instance.GetPlayer();
	}

	private void Update () {
		if (active)
		{
			DistanceCheckIfInterval();
		}
	}

	private void DistanceCheckIfInterval()
	{
		timeSinceCheck += Time.deltaTime;
			
		if (timeSinceCheck >= checkInterval)
		{
			timeSinceCheck = 0;
			DistanceCheck();
		}

	}

	private void DistanceCheck()
	{
		if (GetDistance2D() < activateDistance)
		{
			if (!childrenActive)
			{
				ActivateChildren();
			}

		}
		else
		{

			if (childrenActive)
			{
				DeactivateChildren();
			}

		}
	}

	private void DeactivateChildren()
	{
		childrenActive = false;
		
		foreach (RegionOptimization child in childrenRegions)
		{
			child.Deactivate();
		}
	}
	
	private void ActivateChildren()
	{
		childrenActive = true;

		foreach (RegionOptimization child in childrenRegions)
		{
			child.Activate();
		}
	}

	private float GetDistance2D()
	{
		Vector3 transformPosition = transform.position;
		Vector3 playerPosition = player.transform.position;
		
		return Vector2.Distance(
			new Vector2(playerPosition.x, playerPosition.z),
			new Vector2(transformPosition.x, transformPosition.z)
		);
	}

	public bool GetActive()
	{
		return active;
	}
	
	public static void AllRegionsDistanceCheck()
	{
		foreach (MajorRegions region in allRegions)
		{
			if (region.active)
				region.DistanceCheck();
		}
	}
	
	private void OnDestroy()
	{
		allRegions.Remove(this);
	}

	private void MakeListAndAdd()
	{
		if (allRegions == null)
		{
			allRegions = new List<MajorRegions>();
		}
		
		allRegions.Add(this);
		
		


	}

    // Handles drawing gizmo to visualize the control point in scene view 
    private void DrawGizmo(bool selected)
    {
        if(selected)
        {
	        var green = new Color(0.1f, 0.8f, 0.4f, 0.2f);
	        Gizmos.color = green;
	        Gizmos.matrix = transform.localToWorldMatrix;
	        Gizmos.DrawSphere(Vector3.zero, activateDistance);
	        Gizmos.DrawWireSphere(Vector3.zero, activateDistance);
        }
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
