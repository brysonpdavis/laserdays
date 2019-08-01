using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Policy;
using UnityEngine.Experimental.GlobalIllumination;

public class CrabWalk : MonoBehaviour {

    Rigidbody rb;
    Transform renderChild;

    public float speed = 1f;
    public float currentSpeed;
    public float speedLimit = 10f;

    public float stuckChecktime = 1f;
    public float StuckSensitivity = 0.1f;

    public float walkingCheckTime = 5f;

    public float minRotation = 5f;
    public float maxRotation = 10f;
    public float rotationDuration = 1f;

    Vector3 lastCheckPos;
    float elapsedTimeStuck;

    float elapsedTimeWalking;
    public bool held = false;
    AudioSource audio;

    public ParticleSystem dustTrail;

    [SerializeField]
    private bool stuck;
    [SerializeField]
    private bool rotating;
    [SerializeField]
    float targetRotation;
    [SerializeField]
    float completedRotation;
    [SerializeField]
    float rotationStep;


    void Start () 
    {
        rb = GetComponent<Rigidbody>();
        renderChild = transform.GetChild(0).transform;
        lastCheckPos = transform.position;
        currentSpeed = speed;
        audio = GetComponent<AudioSource>(); 
	}

    public void OnHold()
    {
        held = true;
        currentSpeed = 0;
        transform.GetChild(0).localRotation = Quaternion.Euler(Vector3.zero);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        dustTrail.Stop();
    }

    public void OnDrop()
    {
        held = false;
        currentSpeed = speed;
        rb.constraints = RigidbodyConstraints.None;
        rb.freezeRotation = true;
        dustTrail.Play();
    }

    void FixedUpdate()
    {
        CheckAudio();
        if (!held)
        {
            // Rotating takes priority and walking will not happen in rotation mode 
            if (rotating)
            {
                rb.velocity = Vector3.zero;

                if (completedRotation < targetRotation)
                {
                    completedRotation += rotationStep;
                    Vector3 newEuler = transform.eulerAngles + new Vector3(0, rotationStep, 0);
                    rb.MoveRotation(Quaternion.Euler(newEuler));
                }
                else
                {
                    rotating = false;
                }
            }
            else
            {
                // Walk along the forward vector
                rb.AddForce(transform.forward * currentSpeed, ForceMode.VelocityChange);
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, speedLimit);

                MoveToGround();

                if (elapsedTimeWalking > walkingCheckTime)
                {
                    completedRotation = 0f;
                    targetRotation = Random.Range(minRotation, maxRotation);

                    rotationStep = (targetRotation * Time.fixedDeltaTime) / rotationDuration;
                    rotating = true;
                    elapsedTimeWalking = 0f;
                }

                // Check if the crab is stuck every time the elapsed time exceeds the check timer
                if (elapsedTimeStuck > stuckChecktime)
                {
                    CheckStuck();
                    elapsedTimeStuck = 0f;
                }

                elapsedTimeStuck += Time.fixedDeltaTime;
                elapsedTimeWalking += Time.fixedDeltaTime;

                // If stuck put the crab into rotation mode with a random target rotation
                if (stuck)
                {
                    completedRotation = 0f;
                    targetRotation = Random.Range(minRotation, maxRotation);

                    rotationStep = (targetRotation * Time.fixedDeltaTime) / rotationDuration;

                    rotating = true;
                    stuck = false;
                }


            }
        }
    }

    void CheckAudio()
    {
        if (Toolbox.Instance.PlayerInLaser())
            audio.mute = false;
        else
            audio.mute = true;
    }

    // Check if the object has not moved within the past check period
    void CheckStuck()
    {
        if (Vector3.Distance(transform.position, lastCheckPos) < StuckSensitivity)
        {
            stuck = true;
        }
        else
        {
            stuck = false;
        }

        lastCheckPos = transform.position;
    }

    // Snap main rigid body to ground plain and align chid to normal
    void MoveToGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 5f, LayerMaskController.Laser))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
            renderChild.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            renderChild.eulerAngles = new Vector3(renderChild.eulerAngles.x, transform.eulerAngles.y + 180f, renderChild.eulerAngles.z);
        }

    }
}
