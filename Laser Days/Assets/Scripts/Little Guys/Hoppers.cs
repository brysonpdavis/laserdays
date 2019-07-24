using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoppers : MonoBehaviour
{
	[SerializeField]
	private float checkInterval = 0.25f;

	[SerializeField]
	private GameObject[] waypoints;

	[SerializeField]
	private float activateDistance = 3f;

	[SerializeField]
	private float jumpDuration = 1f;

	[SerializeField]
	private float jumpHeight = 2f;

	[SerializeField] 
	private bool repeat = true;

	private int waypointIndex;

	private Transform player;

	private float elapsed;

	[SerializeField]
	private bool active;

	void Start ()
	{
		player = Toolbox.Instance.GetPlayer().transform;
		elapsed = 0;
		waypointIndex = 0;
		active = true;
	}
	
	void Update ()
	{
		if (active) 
		{
			if (elapsed > checkInterval)
			{
				elapsed = 0;

				if (Toolbox.Instance.PlayerInLaser() && Vector3.Distance(player.position, transform.position) < activateDistance)
				{
					Hop();
				}
			}
			else
			{
				elapsed += Time.deltaTime;
			}
		}		
	}

	void Hop()
	{
		StartCoroutine(BeginJump(waypoints[waypointIndex].transform.position));
	}

	IEnumerator BeginJump(Vector3 endPos)
	{
		Vector3 beginPos = transform.position;
		
		float elapsed = 0;

		float ratio = 0;
		
		active = false;

		while (elapsed < jumpDuration)
		{
			yield return null;

			elapsed += Time.deltaTime;

			ratio = elapsed / jumpDuration;

			transform.position = MathParabola.Parabola(beginPos, endPos, jumpHeight,(TweeningFunctions.EaseMiddle(ratio) + ratio) / 2f); 
			//Vector3.Lerp(beginPos, endPos, TweeningFunctions.EaseInOutCubic(elapsed/jumpDuration));
		}

		transform.position = endPos;

		waypointIndex++;

		if (repeat && waypointIndex == waypoints.Length)
		{
			waypointIndex = 0;
		}

		if (waypointIndex < waypoints.Length)
		{
			//Debug.LogError("Going to next");
			active = true;
		}
	}
}
