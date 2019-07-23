using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class GardenDrones : MonoBehaviour
{
	private static List<GardenDrones> allDrones;

	private List<SpawnableMutation> mutations;
	
	private SpawnableMutation target;

	private Vector3 targetPosition;

	[SerializeField]
	private Transform patrolPoint;
	
	[SerializeField] 
	private float patrolDistance;

	[SerializeField] 
	private float waitDuration = 1f;

	[SerializeField] 
	private float speed;

	[SerializeField]
	private Transform[] waypoints;

	private DroneState state;

	private float timePassed;

	private int waypointIndex;

	private AudioSource audio;

	[SerializeField]
	private GameObject beam;
    private AudioSource beamAudio;

	[SerializeField]
	private float beamTime;

	
	public enum DroneState
	{
		Waiting,
		Approaching,
		Destroying,
		Patrolling
	}
		
	
	public static void AddMutationToDrones(SpawnableMutation mut)
	{
		if (allDrones == null)
			allDrones = new List<GardenDrones>();
		
		foreach (GardenDrones drone in allDrones)
		{
			// for each drone, if the mutation is in its patrol area, add that mutation to its list of mutations
			if (CalcDistance2D(drone.patrolPoint, mut.transform) < drone.patrolDistance)
			{
				drone.mutations.Add(mut);
				if (drone.state == DroneState.Patrolling)
				{
					drone.NewTarget();
				}
			}
		}
	}

	public static void RemoveMutationFromDrones(SpawnableMutation mut)
	{
		foreach (GardenDrones drone in allDrones)
		{
			drone.mutations.Remove(mut);

			if (drone.target == mut && drone.state == DroneState.Approaching)
			{
				drone.NewTarget();
			}
		}
	}
	

	// Use this for initialization
	void Start ()
	{
		if (allDrones == null)
		{
			allDrones = new List<GardenDrones>();
		}

		mutations = new List<SpawnableMutation>();
		
		allDrones.Add(this);

		state = DroneState.Patrolling;

		timePassed = 0;
		
		BeamOff();
		
		audio = GetComponent<AudioSource>();
        beamAudio = beam.GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
		
		switch (state)
		{
			case DroneState.Waiting:
/*
				if (timePassed < waitDuration)
				{
					timePassed += Time.deltaTime;
				}
				else
				{
					state = DroneState.Patrolling;
					timePassed = 0;
					BeamOff();
				}
*/

				break;
			
			case DroneState.Destroying:
				if (timePassed < target.deathLength )
				{
					timePassed += Time.deltaTime;
				}
				else
				{
					timePassed = 0;
					RemoveMutationFromDrones(target);
					BeamOff();
				}

				break;
			
			default:
				break;
		}

		if (Toolbox.Instance.PlayerInReal())
			audio.mute = false;
		
        else
			audio.mute = true;

	}

	private void FixedUpdate()
	{
		switch (state)
		{
			case DroneState.Approaching:
				
				transform.LookAt(target.transform);
				transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
				
				transform.position += transform.forward * Time.deltaTime * speed;

				if (CalcDistance2D(transform, target.transform) < 0.1)
				{
					state = DroneState.Waiting;
					BeamOn();
				}
				
				break;
			
			case DroneState.Patrolling:

/*
				transform.LookAt(waypoints[waypointIndex]);
				transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
*/

				transform.position += transform.forward * Time.deltaTime * speed;


				if ( CalcDistance2D(transform, waypoints[waypointIndex]) < 0.1 )
				{
					waypointIndex++;
					if (waypointIndex == waypoints.Length) waypointIndex = 0;
					
					StartCoroutine(TurnToLook(waypoints[waypointIndex].gameObject));
				}
				
				break;
			
			default:
				break;
		}
	}

	void NewTarget()
	{
		float minDist = float.MaxValue;
		SpawnableMutation tempObject = null;
		float tempDist;
		
		foreach (SpawnableMutation guy in mutations)
		{
			tempDist = CalcDistance2D(guy.transform, transform);
			if ( tempDist < minDist ) //&& Vector3.Distance(guy.transform.position, patrolPoint) < patrolDistance)
			{
				minDist = tempDist;
				tempObject = guy;
			}
		}

		target = tempObject;
		
		if (target)
		{
			//approach the new target
			StartCoroutine(TurnToLook(target.gameObject));
		}
		else
		{
			//start patrolling
			StartCoroutine(TurnToLook(waypoints[waypointIndex].gameObject));
		}	
	}

	static float CalcDistance2D(Transform thing1, Transform thing2)
	{
		Vector3 pos1 = thing1.position;
		Vector3 pos2 = thing2.position;
		
		return Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
	}

	void BeamOn()
	{
		//beam.SetActive(true);
		StartCoroutine(BeamGrow());
	}

	IEnumerator BeamGrow()
	{
		float end_scale = Mathf.Abs(transform.position.y - target.transform.position.y);
		float duration = beamTime;
		float elapsed = 0;
		float ratio = 0;
		
		beam.transform.localScale = new Vector3(beam.transform.localScale.x, beam.transform.localScale.y, 0);

		while (elapsed < duration)
		{
			yield return null;

			elapsed += Time.deltaTime;
			ratio = elapsed / duration;
			
			beam.transform.localScale = new Vector3(beam.transform.localScale.x, beam.transform.localScale.y,  end_scale * TweeningFunctions.EaseIn(ratio));

		}

		beam.transform.localScale = new Vector3(beam.transform.localScale.x, beam.transform.localScale.y, end_scale);
		target.lifePhase = SpawnableMutation.LifeCycle.Death;
		state = DroneState.Destroying;
	}

	void BeamOff()
	{
		StartCoroutine(BeamShrink());
	}

	IEnumerator BeamShrink()
	{
		float end_scale = beam.transform.localScale.z;
		float duration = beamTime;
		float elapsed = 0;
		float ratio = 0;
		
		beam.transform.localScale = new Vector3(beam.transform.localScale.x, beam.transform.localScale.y, end_scale);

		while (elapsed < duration)
		{
			yield return null;

			elapsed += Time.deltaTime;
			ratio = elapsed / duration;
			
			beam.transform.localScale = new Vector3(beam.transform.localScale.x, beam.transform.localScale.y, end_scale - (TweeningFunctions.EaseIn(ratio) * end_scale));

		}

		beam.transform.localScale = new Vector3(beam.transform.localScale.x, beam.transform.localScale.y, 0);
		
		//beam.SetActive(false);
		NewTarget();

	}

	IEnumerator TurnToLook(GameObject targetObject)
	{
		state = DroneState.Waiting;
		
		float ratio = 0;
		float duration = waitDuration;
		float elapsed = 0;
		Quaternion begin = transform.rotation;
		
		transform.LookAt(targetObject.transform);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

		Quaternion end = transform.rotation;

		transform.rotation = begin;
		
		while (ratio < 1)
		{
			yield return null;

			elapsed += Time.deltaTime;
			ratio = elapsed / duration;

			transform.rotation = Quaternion.Slerp(begin, end,TweeningFunctions.EaseOut(ratio));
		}

		if (targetObject.GetComponent<SpawnableMutation>())
		{
			state = DroneState.Approaching;

			target = targetObject.GetComponent<SpawnableMutation>();
		}
		else
		{
			state = DroneState.Patrolling;
		}
	}

	void StopLocalCoroutines()
	{
		StopCoroutine("TurnToLook");
		StopCoroutine("BeamGrow");
		StopCoroutine("BeamShrink");
	}
}
