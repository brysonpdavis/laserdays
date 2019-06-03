﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	[SerializeField] private int envSize = 100;
	public bool space;
	private PlayerCharge pc;
    private flipburst flipburst;
    public IList<EyeThatSees> eyeThatSeesList;

    //sounds
    private AudioSource audioSource;
    public AudioSource audioSourceSecondary;

    private MFPP.FlipClipAsset flipSounds;
    private SoundTrackManager soundTrack;

    public AudioClip flipFailClip;
    public bool flippedThisFrame = false;
    public GameObject ring;
    private GameObject currentRing;
    TransitionCollider transitionCollider;

	RaycastManager rm;
    private string SoundtrackButton = "Submit";


    private void Awake()
    {
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


		pc = GetComponent<PlayerCharge>();
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
        if (Input.GetButtonDown("Switch") && Time.timeScale > 0.0f && CheckEyes())
        {
            GameObject heldObj = GetComponent<MFPP.Modules.PickUpModule>().heldObject;

            if (heldObj) //first check to make sure the object that's held is flippable
            {
                if (!heldObj.GetComponent<InteractableObject>().Flippable)
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

			if (heldObj) {
                
				if (pc.Check(heldObj)) { // if the player and any held object can be flipped:
					pc.ItemInteraction(heldObj); // subtract any charge required
					FlipPlayerAndThings(gameObject, heldObj, rm.selectedObjs);
					pc.UpdatePredictingSlider();
                    flippedThisFrame = true;
				}
				else {
					
                    // make a sound effect letting player
					// know that they don't have enough charge
                    Toolbox.Instance.SetVolume(audioSource);
                    audioSource.clip = flipFailClip;
                    audioSource.Play();

				}
			}
			else {
				if (pc.CheckPlayerCharge())
				{
					pc.PlayerInteraction();
					FlipPlayerAndThings(gameObject, heldObj, rm.selectedObjs);
					pc.UpdatePredictingSlider();
                    flippedThisFrame = true;
				}
			}
		}

        if (Input.GetButtonDown(SoundtrackButton))
        {
            if (soundTrack.gameObject.activeSelf) { soundTrack.gameObject.SetActive(false); }
            else { soundTrack.gameObject.SetActive(true); }
        }

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

		if (!Input.GetMouseButton(1))
		{
			foreach (GameObject obj in objs)
			{
                rm.RemoveFromList(obj, true, true);
			}
			objs.Clear();
		}

        else
        {
            foreach (GameObject obj in objs)
            {
                obj.GetComponent<Renderer>().material.SetInt("_IsSelected", 1);
            }
        }
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
            return false;

        else return true;
    }
}
