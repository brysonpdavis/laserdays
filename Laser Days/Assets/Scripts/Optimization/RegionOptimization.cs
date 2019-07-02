using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RegionOptimization : MonoBehaviour
{

	public float checkInterval = 2f; // interval between distance checks in seconds

	public float activateDistance = 100f;

	public bool isBase = false; // set to true if direct parent of children to set to far/near versions
	
	public RegionOptimization[] childrenRegions;

	public GameObject inactiveGeometry;

	public GameObject activeLevel;

	private GameObject player;

	private bool active;

	private float timeSinceCheck;

	private bool childrenActive;


	private void Start ()
	{
		player = Toolbox.Instance.GetPlayer();
		childrenActive = !(GetDistance2D() < activateDistance);
		timeSinceCheck = 0;
		DistanceCheck();
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
			if (childrenActive)
				DeactivateChildren();
		}
		else
		{
			if (!childrenActive)
				ActivateChildren();
		}
	}

	private void DeactivateChildren()
	{
		childrenActive = false;

		foreach (RegionOptimization child in childrenRegions)
		{
			child.SetToActive(false);
		}
	}
	
	private void ActivateChildren()
	{
		childrenActive = true;
		
		foreach (RegionOptimization child in childrenRegions)
		{
			child.SetToActive(true);
		}
	}

	private float GetDistance2D()
	{
		Vector3 transformPosition =  transform.TransformPoint(GetComponent<BoxCollider>().center);
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

	public void SetToActive(bool setToStatus)
	{
		active = setToStatus;

		if (isBase)
		{
			if (setToStatus)
			{
				inactiveGeometry.SetActive(true);
				activeLevel.SetActive(false);
			}
			else
			{
				inactiveGeometry.SetActive(false);
				activeLevel.SetActive(true);
			}
		}
	}
}
