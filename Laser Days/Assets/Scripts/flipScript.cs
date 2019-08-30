using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

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
    MFPP.Modules.PickUpModule _pickupModule;

	RaycastManager rm;
    private string SoundtrackButton = "Submit";

    public delegate void FailedFlip();
    public static event FailedFlip OnFailedFlip;

    private MutationSpawner mutationSpawner;

    private ImageEffectEyes eyesEffect;

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

/*
        //make sure player is seeing ladders in correct world
        if (space)
        {
            GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 262144; //only see ladders in real world
        } 
        else { GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 524288; //only see ladders in laser world
        }
*/

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
        _pickupModule = GetComponent<MFPP.Modules.PickUpModule>();

        eyesEffect = Camera.main.GetComponent<ImageEffectEyes>();  

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

        if (IsSeenByEyes())
        {
            eyesEffect.isSeen = true;
        }
        else eyesEffect.isSeen = false;
	}

    public void ForceFlip() {
        forcedFlip = true;
    }

    public bool CanFlip() {
        return ControlManager.Instance.GetButtonDown("Switch") && !Toolbox.Instance.GetPauseMenu().activeSelf && CheckEyes();
    }

    public void FlipAttempt()
    {
        GameObject heldObj = _pickupModule.heldObject;
        flippedThisFrame = true;

        if (heldObj)
        {
            IFlippable _flippable = heldObj.GetComponent<ReticleObject>() as IFlippable;

            if (_flippable == null || !_flippable.MaxFlipCheck(false))
            {
                _pickupModule.PutDown();
                heldObj = null;
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
           // Camera.main.GetComponent<CameraTransition>().Flip(true);
            GetComponent<SkyboxTransition>().Flip(true);
            transitionCollider.FlipTransitions(true);

            mutationSpawner.SpawnMutations();
            
            AmbientSound.CrossfadeAllToReal();

        } 
        else { player.layer = 15; //set player to laser world
            GetComponent<SkyboxTransition>().Flip(false);
            transitionCollider.FlipTransitions(false);
            
            AmbientSound.CrossfadeAllToLaser();
        } 

        if (held)
			Flip(held);
		FlipList(things);
        Toolbox.Instance.FlipSharedMaterials(space);
        currentRing = Instantiate(ring);
        currentRing.transform.position = transform.position;


        if (soundTrack)
        {
            //play secondary sound when there is a held object or are selected objects
            if (held || (selectedObjects))
            {
                    soundTrack.PlaySecondary();
            }

            else
            {
                soundTrack.PlayPrimary();
            }

            soundTrack.FadeBetween(space);
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

    public bool IsSeenByEyes()
    {
        int check = 0;
        foreach (EyeThatSees eye in eyeThatSeesList)
        {
            if (eye.blockingFlip)
                check += 1;

        }

        if (check > 0)
        {

            return true;

        }

        else return false;
    }
}
