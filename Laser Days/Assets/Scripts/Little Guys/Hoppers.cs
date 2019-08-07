using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Media;

public class Hoppers : MonoBehaviour
{
	[SerializeField]
    private float distanceCheckInterval = 0.25f;

    [SerializeField]
    private float rotationLookInterval = 0.25f;

    public float rotationDuration = 0.2f;

    [SerializeField]
	private GameObject[] waypoints;

    private Vector3[] waypointsOnGround;

	[SerializeField]
	private float activateDistance = 3f;

	[SerializeField]
	private float jumpDuration = 1f;

    [SerializeField]
    private float squashDuration = 1f;

    private float halfHeight;

    [SerializeField]
	private float jumpHeight = 2f;

	[SerializeField] 
	private bool repeat = true;

	private int waypointIndex;

	private Transform player;

	private float elapsed;

	[SerializeField]
	private bool active;

    private Animator anim;

    public ParticleSystem hopParticles;

    Vector3 lastLookTarget;
    Vector3 currentLookTarget;
    float rotationTimer;

	void Start ()
	{
		player = Toolbox.Instance.GetPlayer().transform;
		elapsed = 0;
        rotationTimer = 0;
		waypointIndex = 0;
        halfHeight = GetComponent<Renderer>().bounds.extents.y;
		active = true;
        anim = GetComponent<Animator>();
        waypointsOnGround = InitWayPoints(waypoints);

    }
	
	void Update ()
	{
        if (active)
        {
            //FrogLook(player.position);

            if (elapsed > distanceCheckInterval)
            {
                elapsed = 0;

                if (Toolbox.Instance.PlayerInLaser() && Vector3.Distance(player.position, transform.position) < activateDistance)
                {
                    anim.Play("PreJump");
                }
            }
            else
            {
                elapsed += Time.deltaTime;
            }

            if (rotationTimer > rotationLookInterval)
            {
                rotationTimer = 0;
                StartCoroutine(LookAtTarget(lastLookTarget, player.position, rotationDuration));
            }
            else
            {
                rotationTimer += Time.deltaTime;
            }

        }

    }

	public void Hop()
	{
        int index = waypointIndex;
        StartCoroutine(BeginJump(waypointsOnGround[index]));

        StartCoroutine(LookAtTarget(lastLookTarget, waypointsOnGround[index], rotationDuration));

        hopParticles.Play();
    }

    IEnumerator BeginJump(Vector3 endPos)
	{
		Vector3 beginPos = transform.position;
		
		float jumpElapsed = 0;

        float ratio = 0;
		
		active = false;

        while (jumpElapsed < jumpDuration)
		{

			jumpElapsed += Time.deltaTime;

			ratio = jumpElapsed / jumpDuration;

			transform.position = MathParabola.Parabola(beginPos, endPos, jumpHeight,(TweeningFunctions.EaseMiddle(ratio) + ratio) / 2f);
            //Vector3.Lerp(beginPos, endPos, TweeningFunctions.EaseInOutCubic(elapsed/jumpDuration));

            yield return null;
        }

		transform.position = endPos;
        hopParticles.Play();

        waypointIndex++;

        rotationTimer = 0f;


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

    IEnumerator LookAtTarget(Vector3 old, Vector3 target, float duration)
    {
        float elapsedTime = 0f;
        float ratio = 0f;

        while(elapsedTime < duration)
        {
            ratio = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            Vector3 view = Vector3.Lerp(old, target, ratio);
            FrogLook(view);
            yield return null;

        }

        lastLookTarget = target;

    }

    void FrogLook (Vector3 target)
    {
        transform.LookAt(target);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }


    // Snaps all the way points to the ground - so that if we can place the gameobject waypoint in the scene loosely 
    Vector3[] InitWayPoints(GameObject[] gameobjects)
    {
        Vector3[] hopTargets = new Vector3[gameobjects.Length];
        for (int i = 0; i < gameobjects.Length; i++)
        {
            hopTargets[i] = SnapWaypointToGround(gameobjects[i]);
        }
        return hopTargets;
    }

    // Snaps a waypoints gameobject to the ground - so that if we can place the gameobject waypoint in the scene loosely 
    Vector3 SnapWaypointToGround(GameObject waypoint)
    {
        RaycastHit hit;
        if (Physics.Raycast(waypoint.transform.position, Vector3.down, out hit, 2f, LayerMaskController.Everything))
        {
            return hit.point; //+ new Vector3(0f, halfHeight, 0f);
        }
        else
        {
            return waypoint.transform.position;
        }
    }

}
