using System.Collections;
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
    public GameObject plantContainer;
    public float plantCheckRadius;
    IList<GameObject> plantList;


    //public int particleCount = 15;
    //public float radialSpeed = 0.0f;
    //public float lifeMultiplier = 1.0f;

    private Vector3 lastTarget;
    public Vector3 hitPoint;

    public float shrinkTime;


    public float waitTime;
    public float lerpTime;
    private float timeCounter = 0;
    public float focusedScale;
    public float unfocusedScale;
    private Vector3 previousPosition;

    private bool plantRoutineRunning = false;
    private bool snapViewRunning = false;
    private bool widenFocusRunning = false;
    bool lookingAtPlayer = false;

    [HideInInspector]
    private LayerMask currentLayerMask;

    public EyeBeam beam;


    void Start()
    {
        player = Toolbox.Instance.GetPlayer().transform;
        plantList = new List<GameObject>();
        Debug.Log(plantList.Count);

        particleSystem = GetComponentInChildren<ParticleSystem>();
        audio = GetComponent<AudioSource>();
        audio.maxDistance = GetComponent<SphereCollider>().radius;
        beam = GetComponentInChildren<EyeBeam>();

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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            BeamReset();
            if (audio)
                audio.mute = true;

            BeamDeactivate();
            plantRoutineRunning = false;
        }
    }

    private void FixedUpdate()
    {
        if (isActive && !plantRoutineRunning && !lookingAtPlayer)// && (plantList.Count>0))
            StartCoroutine(PlantRoutine());

    }

    public void PlayerInteraction()
    {
        StopAllCoroutines();
        plantRoutineRunning = false;
        Vector3 position = Toolbox.Instance.GetPlayer().transform.position + new Vector3 (0f, 1.5f, 0f);
        StartCoroutine(SnapView(transform.position, position, 1f));
        StartCoroutine(ResetBeamLength(1.5f));
        lookingAtPlayer = true;
    }

    public void PlayerDeactivate()
    {
        StopAllCoroutines();
        lookingAtPlayer = false;  
    }

    void BuildPlantList()
    {
        plantList.Clear();


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, plantCheckRadius);

        foreach (Collider child in hitColliders)
        {
            if (child.gameObject.CompareTag("Plant"))
            {
                plantList.Add(child.gameObject);
            }
        }

        Debug.Log("PLANT LIST LENGTH!!!  " + plantList.Count);
    }


    private IEnumerator PlantRoutine()
    {

            plantRoutineRunning = true;
            BuildPlantList();

            if (plantList.Count == 0)
                yield return null;


            GameObject nextPlant = plantList[(int)Random.Range(0, plantList.Count)];
            plantList.Remove(nextPlant);

            StartCoroutine(SnapView(lastTarget, nextPlant.transform.position, lerpTime));

            yield return new WaitForSeconds(lerpTime);

            float elapsedTime = 0;
            float ratio = 0;
            Vector3 startingScale = nextPlant.transform.localScale;

            while (ratio < 1f)
            {
                if (nextPlant)
                {


                    ratio = elapsedTime / shrinkTime;
                    Vector3 scaleDown = Vector3.Lerp(startingScale, new Vector3(.05f, .05f, .05f), TweeningFunctions.EaseOutCubic(ratio));
                    nextPlant.transform.localScale = scaleDown;
                    elapsedTime += Time.deltaTime;
                    transform.LookAt(nextPlant.transform.position);
                    SetBeamLength(nextPlant.transform.position);

                    float beamWidth = Mathf.Lerp(unfocusedScale, focusedScale, ratio);
                    beam.SetWidth(beamWidth);
                }
                

                yield return null;
            }

        Destroy(nextPlant);

        if (plantList.Count == 0)
        {
            BeamReset();
        }

            plantRoutineRunning = false;
    }




    private IEnumerator SnapView(Vector3 old, Vector3 current, float duration)
    {
        snapViewRunning = true;
        float elapsedTime = 0;
        float ratio = 0;

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            Vector3 view = Vector3.Lerp(old, current, TweeningFunctions.EaseOutCubic(ratio));
            transform.LookAt(view);
            elapsedTime += Time.deltaTime;
            SetBeamLength(hitPoint);

            float beamWidth = Mathf.Lerp(focusedScale, unfocusedScale, ratio);
            beam.SetWidth(beamWidth);

            yield return null;
        }

        lastTarget = current;
        snapViewRunning = false;
    }

    private IEnumerator ResetBeamLength(float duration)
    //for after it's looked at the player
    {
        widenFocusRunning = true;
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

        widenFocusRunning = false;

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

    public void BeamReset()
    {
        StopAllCoroutines();
        StartCoroutine(ResetBeamLength(lerpTime * 2f));
    }



}
