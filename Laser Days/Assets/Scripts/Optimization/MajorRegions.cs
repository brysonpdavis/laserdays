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
}
