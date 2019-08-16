using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;


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

    private bool rotating = false;


	private int waypointIndex;

	private Transform player;

	private float elapsed;

	[SerializeField]
	private bool active;

    private Animator anim;

    public ParticleSystem hopParticles;
    [SerializeField]
    private AudioClip hopSound;
    [SerializeField]
    private AudioClip thudSound;

    Vector3 previousPlayerPosition;
    Vector3 currentLookTarget;
    float rotationTimer;
    AudioSource audio;

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
        audio = GetComponent<AudioSource>();

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
                if (!rotating)
                    StartCoroutine(LookAtTarget(player.position, rotationDuration));
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
        StopCoroutine("LookAtTarget");
            StartCoroutine(LookAtTarget(waypointsOnGround[index], rotationDuration));

        hopParticles.Play();
    }

    IEnumerator BeginJump(Vector3 endPos)
	{
		Vector3 beginPos = transform.position;
		
		float jumpElapsed = 0;

        float ratio = 0;
		
		active = false;

        MFPP.Audio.Play3D(hopSound, gameObject.transform, Toolbox.Instance.soundEffectsVolume, 1f);

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
        if (Toolbox.Instance.PlayerInLaser())
            MFPP.Audio.Play3D(thudSound, transform, Toolbox.Instance.soundEffectsVolume, 1f);

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

    bool PlayerHasMoved()
    {
        if (((previousPlayerPosition - player.position).magnitude) > .1f)
        {
            return true;
        }

        else return false;
    }

    IEnumerator LookAtTarget(Vector3 target, float duration)
    {
        rotating = true;
        float elapsedTime = 0f;
        float ratio = 0f;

        Vector3 begin = transform.position + transform.forward;
        bool playerMoved = PlayerHasMoved();

        while(elapsedTime < duration)
        {
            ratio = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            Vector3 view = Vector3.Lerp(begin, target, ratio);
            FrogLook(view);

            if (audio && playerMoved)
                audio.volume = Toolbox.Instance.soundEffectsVolume * TweeningFunctions.BackAndForth(ratio);

            yield return null;

        }
        audio.volume = 0f;
        rotating = false;
        previousPlayerPosition = player.position;

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
