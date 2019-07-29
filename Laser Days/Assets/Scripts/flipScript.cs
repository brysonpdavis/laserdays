using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class flipScript : MonoBehaviour {
    public bool forcedFlip = true;
    private Vector3 pos;
	private float newX;
	[SerializeField] private int envSize = 100;
	public bool space;
    private flipburst flipburst;
    public IList<EyeThatSees> eyeThatSeesList;

    //sounds
    private AudioSource audioSource;
    public AudioSource audioSourceSecondary;

    private MFPP.FlipClipAsset flipSounds;
    private SoundTrackManager soundTrack;
    public bool canFlip;
    public AudioClip flipFailClip;
    public bool flippedThisFrame = false;
    public GameObject ring;
    private GameObject currentRing;
    TransitionCollider transitionCollider;

	RaycastManager rm;
    private string SoundtrackButton = "Submit";

    public delegate void FailedFlip();
    public static event FailedFlip OnFailedFlip;

    private MutationSpawner mutationSpawner;

    private void Awake()
    {
        flippedThisFrame = false;
        forcedFlip = false;

        if (this.gameObject.layer == 16)
        {space = true;}

        if (this.gameObject.layer == 15)
        { space = false; }
    }

    void Start () {

        //make sure player is seeing ladders in correct world
        if (space)
        {
            GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 262144; //only see ladders in real world
        } 
        else { GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 524288; //only see ladders in laser world
        }

        //Debug.Log(Camera.main.fieldOfView);

        mutationSpawner = GetComponent<MutationSpawner>();
        audioSource = GetComponent<AudioSource>();
		rm = GetComponent<RaycastManager>();
        SoundBox box = SoundBox.Instance;
        flipSounds = box.currentFlipPalette;
        flipFailClip = box.flipFail;
        soundTrack = GetComponentInChildren<SoundTrackManager>();
        flipburst = GetComponentInChildren<flipburst>();
        transitionCollider = GetComponentInChildren<TransitionCollider>();
        eyeThatSeesList = new List<EyeThatSees>();

	}



	void Update () {
        flippedThisFrame = false;

        if (ControlManager.Instance.GetButtonDown("Switch") && ! Toolbox.Instance.GetPauseMenu().activeSelf && CheckEyes() && canFlip)
        {
            FlipAttempt();
        }

        if (forcedFlip) {
            flippedThisFrame = true;
            forcedFlip = false;
        }
	}

    public void ForceFlip() {
        forcedFlip = true;
    }

    public bool CanFlip() {
        return ControlManager.Instance.GetButtonDown("Switch") && !Toolbox.Instance.GetPauseMenu().activeSelf && CheckEyes();
    }

    public void FlipAttempt()
    {
        GameObject heldObj = GetComponent<MFPP.Modules.PickUpModule>().heldObject;
        flippedThisFrame = true;
        if (heldObj) //first check to make sure the object that's held is flippable
        {
            if (!heldObj.GetComponent<HoldableObject>().Flippable)
            {
                GetComponent<MFPP.Modules.PickUpModule>().PutDown();
                heldObj = null;
            }


            //if the player is holding an object that can be flipped, make sure it is able to
            FlippableObject flippable = null;
            if (heldObj && heldObj.GetComponent<FlippableObject>())
            {
                flippable = heldObj.GetComponent<FlippableObject>();
                if (!flippable.MaxFlipCheck(false))
                {
                    GetComponent<MFPP.Modules.PickUpModule>().PutDown();
                    heldObj = null;
                }
            }
        }
        
        FlipPlayerAndThings(gameObject, heldObj, rm.selectedObjs);
        flippedThisFrame = true;
         
    }

	void FlipPlayerAndThings (GameObject player, GameObject held, IList<GameObject> things) {

        GetComponent<MFPP.Player>().collisionLayers = null;
        bool selectedObjects = false;
        if (things.Count > 0) { selectedObjects = true; }

        flipburst.Boom();

        space = !space;

        //change the layer that the player is on, for changing its collision detection
        if (space)
        { player.layer = 16;  //set player to real world
                GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 262144; //only see ladders in real world
           // Camera.main.GetComponent<CameraTransition>().Flip(true);
            GetComponent<SkyboxTransition>().Flip(true);
            transitionCollider.FlipTransitions(true);

            mutationSpawner.SpawnMutations();

        } 
        else { player.layer = 15; //set player to laser world
            GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 524288; //only see ladders in laser world
                                                                                      //   Camera.main.GetComponent<CameraTransition>().Flip(false);
            GetComponent<SkyboxTransition>().Flip(false);
            transitionCollider.FlipTransitions(false);


        } 




        if (held)
			Flip(held);
		FlipList(things);
        Toolbox.Instance.FlipSharedMaterials(space);
        currentRing = Instantiate(ring);
        currentRing.transform.position = transform.position;



        //play secondary sound when there is a held object or are selected objects
        if (held || (selectedObjects) ){
            // audioSourceSecondary.clip = GetComponent<SoundBox>().currentFlipPalette.defaultFlipClips.GetRandomFlipSecondary();
            //audioSourceSecondary.Play();

            if (soundTrack)
            {
                soundTrack.PlaySecondary();
            }
        }

        else {
            //play random flip sound!
            //  audioSource.clip = GetComponent<SoundBox>().currentFlipPalette.defaultFlipClips.GetRandomFlipClip();
            // audioSource.Play();
            if (soundTrack)
            {
                soundTrack.PlayPrimary();
            }
        }



	}

	void Flip (GameObject obj)
	{
        obj.GetComponent<FlippableObject>().OnFlip();
	}

	void FlipList (IList<GameObject> objs)
	{
		foreach (GameObject obj in objs)
		{
			Flip(obj);
		}

			foreach (GameObject obj in objs)
			{
                rm.RemoveFromList(obj, true, true);
			}
			objs.Clear();

	}

	bool PlayerInLaser ()
	{
		return gameObject.layer == 15;
	}
	bool PlayerInReal()
	{
		return gameObject.layer == 16;
	}
	void SetObjectToLaser(GameObject obj)
	{
		obj.layer = 10;
		obj.transform.parent = Toolbox.Instance.GetLaserWorldParent();

	}
	void SetObjectToReal(GameObject obj)
	{
		obj.layer = 11;
		obj.transform.parent = Toolbox.Instance.GetRealWorldParent();
	}

    bool CheckEyes()
    {
        int check = 0;
        foreach (EyeThatSees eye in eyeThatSeesList)
        {
            if (eye.blockingFlip)
                check += 1;
        }

        if (check > 0)
        {
            if (OnFailedFlip != null)
            {
                OnFailedFlip();
            }

            return false;

        }
            
        else return true;
    }
}
