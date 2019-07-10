using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RegionOptimization : MonoBehaviour
{

	public float checkInterval = 2f; // interval between distance checks in seconds

	public float activateDistance = 100f;
	
	public GameObject inactiveGeometry;

	public GameObject activeLevel;

	public string regionName;

	private GameObject player;

	[SerializeField]
	private bool active = true;

	private float timeSinceCheck;

	private bool childrenActive;

	private GameObject root;

	private GameObject parent;

	[SerializeField]
	private float playerDistance;
	
	Vector3 transformPosition;
	
	Vector3 playerPosition;


	private void Awake()
	{
		root = Toolbox.Instance.regionController;
		
		parent = root.transform.Find(regionName).gameObject;

		if (parent)
			parent.GetComponent<MajorRegions>().childrenRegions.Add(this);
	}
	
	private void Start ()
	{
		active = true;
		player = Toolbox.Instance.GetPlayer();
		playerDistance = GetDistance2D();
		childrenActive = !(playerDistance < activateDistance);
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
		playerDistance = GetDistance2D();
		if (playerDistance < activateDistance)
		{
			if (!childrenActive)
			{
				ActivateObjects();
			}
		}
		else
		{
			if (childrenActive)
			{
				DeactivateObjects();
			}
		}
	}


	private float GetDistance2D()
	{
		transformPosition = transform.position;
		playerPosition = player.transform.position;
		
		return Vector2.Distance(
			new Vector2(playerPosition.x, playerPosition.z),
			new Vector2(transformPosition.x, transformPosition.z)
			);
	}

	public bool GetActive()
	{
		return active;
	}

	public void Deactivate()
	{
		SetToActive(false);
	}

	public void DeactivateObjects()
	{
		activeLevel.SetActive(false);
		childrenActive = false;
	}

	public void Activate()
	{
		SetToActive(true);
	}

	public void ActivateObjects()
	{
		activeLevel.SetActive(true);
		childrenActive = true;
	}

	public void SetToActive(bool setToStatus)
	{
		active = setToStatus;
		activeLevel.SetActive(setToStatus);
		childrenActive = setToStatus;

	}
}
