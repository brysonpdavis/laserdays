﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenEye : MonoBehaviour
{

    public bool isActive = false;
    [SerializeField] private Transform player;
    private float startingAngleY;
    private ParticleSystem particleSystem;
    [HideInInspector] public Vector3 currentPlayerPoint;
    AudioSource audio;
    public float plantCheckRadius;
    private List<SpawnableMutation> mutations;
    private SpawnableMutation target; 
    private static List<GardenEye> allBots;
    private Transform patrolPoint;
    private float timePassed;

    private float patrolDistance = 10f;
    public float shrinkTime;

    public float turnTime;
    private float timeCounter = 0;
    private Vector3 playerAdditionValue = new Vector3 (0f, 1.5f, 0f);


    private bool plantRoutineRunning = false;
    private bool snapViewRunning = false;
    bool lookingAtPlayer = false;
    SimpleBob bob;
    public EyeBeam beam;

    public enum BotState
    {
        Following,
        Speaking,
        Destroying,
        Turning
    }

    public BotState state;

    void Start()
    {
        if (allBots == null)
        {
            allBots= new List<GardenEye>();
        }

        mutations = new List<SpawnableMutation>();

        allBots.Add(this);
        patrolPoint = gameObject.transform;


        player = Toolbox.Instance.GetPlayer().transform;

        particleSystem = GetComponentInChildren<ParticleSystem>();
        audio = GetComponent<AudioSource>();
        audio.maxDistance = GetComponent<SphereCollider>().radius;
        beam = GetComponentInChildren<EyeBeam>();

        mutations = new List<SpawnableMutation>();
        bob = GetComponent<SimpleBob>();


    }

    private void OnTriggerEnter(Collider other)
    {
        //BuildPlantList();
        if (other.CompareTag("Player"))
        {
            isActive = true;

            Toolbox.Instance.SetVolume(audio);
            if (audio)
                audio.mute = false;

            BeamActivate();
        }
    }

    public static void AddMutationToBots(SpawnableMutation mut)
    {
        if (allBots == null)
            allBots = new List<GardenEye>();

        foreach (GardenEye bot in allBots)
        {
            // for each drone, if the mutation is in its patrol area, add that mutation to its list of mutations
            if (CalcDistance2D(bot.patrolPoint, mut.transform) < bot.patrolDistance)
            {
                bot.mutations.Add(mut);
                if (bot.state == BotState.Following)
                {
                    bot.NewTarget();
                }
            }
        }
    }

    bool MutationCanBeSeen(GameObject mut)
    {
        RaycastHit hit;

        Vector3 smallForward = new Vector3(0f, 0f, -.5f);

        if (Physics.Raycast(transform.position + smallForward, mut.transform.position - (transform.position + smallForward), out hit, patrolDistance, LayerMaskController.RealWithTransOnly))
        {
            //Debug.DrawRay(transform.position + smallForward, mut.transform.position - (transform.position + smallForward), Color.red, 10f);
            //Debug.LogError(hit.collider.gameObject.name);
            if (hit.collider.gameObject.Equals(mut))
                return true;
            else 
                return false;
            
        }
        else
            return false;
    }

    public static void RemoveMutationFromBots(SpawnableMutation mut)
    {
        foreach (GardenEye bot in allBots)
        {
            bot.mutations.Remove(mut);

            if (bot.target == mut && bot.state == BotState.Following)
            {
                bot.NewTarget();
            }
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
            if (tempDist < minDist && MutationCanBeSeen(guy.gameObject)) //&& Vector3.Distance(guy.transform.position, patrolPoint) < patrolDistance)
            {
                minDist = tempDist;
                tempObject = guy;
            }
        }

        target = tempObject;

        if (target)
        {
            //approach the new target
            StartCoroutine(PlantRoutine(target.gameObject));
        }
        else
        {
            //look to the player again
            if (state != BotState.Following || state != BotState.Destroying)
            {
                state = BotState.Turning;
                StartCoroutine(SnapView(player.position + playerAdditionValue, turnTime, true));
            }

        }
    }

    static float CalcDistance2D(Transform thing1, Transform thing2)
    {
        if (!thing1 || !thing2)
            return float.MaxValue;

        Vector3 pos1 = thing1.position;
        Vector3 pos2 = thing2.position;

        return Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            //BeamReset();
            if (audio)
                audio.mute = true;

            BeamDeactivate();
            plantRoutineRunning = false;
        }
    }

    void Update()
    {

        switch (state)
        {
            case BotState.Following:
                transform.LookAt(player.position + playerAdditionValue);
                break;

            case BotState.Speaking:
                break;

            case BotState.Destroying:
                if (timePassed < target.deathLength)
                {
                    timePassed += Time.deltaTime;
                }
                else
                {
                    timePassed = 0;
                    RemoveMutationFromBots(target);
                    NewTarget();
                }

                break;

            case BotState.Turning:
                break;

            default:
                break;
                
        }
    }

    public void PlayerInteraction()
    {
        StopAllCoroutines();
        if (state == BotState.Destroying && target)
            RemoveMutationFromBots(target);
        plantRoutineRunning = false;
        state = BotState.Speaking;
        Vector3 position = Toolbox.Instance.GetPlayer().transform.position + playerAdditionValue;
        StartCoroutine(SnapView(position, turnTime, true));
        bob.bob = true;
        lookingAtPlayer = true;
    }


    public void PlayerDeactivate()
    {
        StopAllCoroutines();
        lookingAtPlayer = false;
        state = BotState.Following;
        bob.bob = false;
        NewTarget();
    }

    private IEnumerator PlantRoutine(GameObject nextPlant)
    {
            //yield return new WaitForSeconds(nextPlant.GetComponent<SpawnableMutation>().growthLength);    
            SpawnableMutation mutation = nextPlant.GetComponent<SpawnableMutation>();
            //if (mutation.lifePhase == SpawnableMutation.LifeCycle.Growth)
                    //yield return new WaitForSeconds(mutation.growthLength);


            StartCoroutine(SnapView(nextPlant.transform.position, turnTime, false));
            state = BotState.Turning;
            yield return new WaitForSeconds(turnTime);
            state = BotState.Destroying;
            mutation.lifePhase = SpawnableMutation.LifeCycle.Death;
            plantRoutineRunning = false;
            yield return null;
    }




    private IEnumerator SnapView(Vector3 current, float duration, bool toPlayer)
    {
        snapViewRunning = true;
        float elapsedTime = 0;
        float ratio = 0;
        Vector3 start = transform.position + transform.forward;//beamEnd.position;//= new Vector3(transform.position.x, transform.position.y, transform.position.z + beam.transform.localScale.z);
        Vector3 end = current;
        float beamStart = beam.transform.localScale.z;


        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            Vector3 view = Vector3.Lerp(start, end, TweeningFunctions.EaseOutCubic(ratio));
            transform.LookAt(view);
            elapsedTime += Time.deltaTime;
            SetBeamLength(current);

            if (toPlayer)
            {
                float length = Mathf.Lerp(beamStart, 0f, TweeningFunctions.EaseOutCubic(ratio));
                beam.SetLength(length);   
            }

            yield return null;
        }

        snapViewRunning = false;

        if (toPlayer && state != BotState.Speaking)
            state = BotState.Following;
    }

    private IEnumerator ResetBeamLength(float duration)
    //for after it's looked at the player
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            elapsedTime += Time.deltaTime;

            //doing lerp for focal scale
            float start = beam.transform.localScale.z;
            float length = Mathf.Lerp(start, 0f, TweeningFunctions.EaseOutCubic(ratio));
            beam.SetLength(length);

            yield return null;
        }
    }

    void SetBeamLength(Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);

        beam.SetLength(dist);

    }

    void SetBeamWidth(float width)
    {
        beam.SetWidth(width);
    }

    public void BeamActivate()
    {
        beam.UnmuteBeam();
    }

    public void BeamDeactivate()
    {
        beam.MuteBeam();
    }




}
