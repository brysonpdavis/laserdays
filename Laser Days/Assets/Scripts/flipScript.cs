using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]

public class flipScript : MonoBehaviour {
	private Vector3 pos;
	private float newX;
	[SerializeField] private int envSize = 100;
	public bool space;
	private PlayerCharge pc;

    //sounds
    private AudioSource audioSource;
    public AudioSource audioSourceSecondary;

    private MFPP.FlipClipAsset flipSounds;
    private SoundTrackManager soundTrack;

    public AudioClip flipFailClip;
    public bool flippedThisFrame = false;

	RaycastManager rm;


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
        SoundBox box = GetComponent<SoundBox>();
        flipSounds = box.currentFlipPalette;
        flipFailClip = box.flipFail;
        soundTrack = GetComponentInChildren<SoundTrackManager>();
	}

	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject heldObj = GetComponent<MFPP.Modules.PickUpModule>().heldObject;

            if (heldObj) //first check to make sure the object that's held is flippable
            {
                if (heldObj.GetComponent<ItemProperties>().unflippable)
                {
                    GetComponent<MFPP.Modules.PickUpModule>().PutDown();
                    heldObj = null;
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
	}

	void FlipPlayerAndThings (GameObject player, GameObject held, IList<GameObject> things) {

        GetComponent<MFPP.Player>().collisionLayers = null;
        bool selectedObjects = false;
        if (things.Count > 0) { selectedObjects = true; }

        space = !space;

        //change the layer that the player is on, for changing its collision detection
        if (space)
        { player.layer = 16;  //set player to real world
                GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 262144; //only see ladders in real world
           // Camera.main.GetComponent<CameraTransition>().Flip(true);
        } 
        else { player.layer = 15; //set player to laser world
            GetComponent<MFPP.Modules.LadderModule>().LadderLayerMask.value = 524288; //only see ladders in laser world
         //   Camera.main.GetComponent<CameraTransition>().Flip(false);

        } 




        if (held)
			Flip(held);
		FlipList(things);



        //play secondary sound when there is a held object or are selected objects
        if (held || (selectedObjects) ){
            // audioSourceSecondary.clip = GetComponent<SoundBox>().currentFlipPalette.defaultFlipClips.GetRandomFlipSecondary();
            //audioSourceSecondary.Play();

            soundTrack.PlaySecondary();
        }

        else {
            //play random flip sound!
            //  audioSource.clip = GetComponent<SoundBox>().currentFlipPalette.defaultFlipClips.GetRandomFlipClip();
            // audioSource.Play();
            soundTrack.PlayPrimary();
        }



	}

	void Flip (GameObject obj)
	{
        ItemProperties.ObjectType type = obj.GetComponent<ItemProperties>().objectType;
        //for objects not being currently held: 

        if (!obj.Equals(GetComponent<MFPP.Modules.PickUpModule>().heldObject)){

            //check which layer the player has moved to, and then change object's layer, shader, and value
            if (PlayerInLaser())
            { //if player is now in laser world
                SetObjectToLaser(obj); //set object to laser layer


                if (type == ItemProperties.ObjectType.Morph) 
                {
                    // if it's a morph obj EITHER on or off do the transition on the object
                    obj.GetComponent<MorphController>().OnFlip(true);

                }


                    //otherise set the object to approperiate shader automaticalh
                    obj.GetComponent<Renderer>().material.shader = rm.laserWorldShader;  //shader change is now happening in flip script
                    obj.GetComponent<Transition>().SetStart(1f); //set it fully on for laser world
                    obj.GetComponent<Renderer>().material.SetInt("_IsSelected", 0);

            }
                else if ( PlayerInReal() )
                { //if player is now in real world
                    SetObjectToReal(obj); //set object to real layer

                if (type == ItemProperties.ObjectType.Morph)
                {
                    // if it's a morph obj EITHER on or off do the transition on the object
                    obj.GetComponent<MorphController>().OnFlip(false);

                }
                    obj.GetComponent<Renderer>().material.shader = rm.realWorldShader;  //shader change is now happening in flip script
                    obj.GetComponent<Transition>().SetStart(0f); //set it fully on for real world
                    obj.GetComponent<Renderer>().material.SetInt("_IsSelected", 0);
                
                }
            }
        //if the object IS being held, we do the same thing, just change layer without switching the shader (which will get switched on drop)
        else {
            
            if ( PlayerInLaser() )
            { //if player is now in laser world
                SetObjectToLaser(obj); //set object to laser layer

                if (type == ItemProperties.ObjectType.Morph)
                {
                    // if it's a morph obj EITHER on or off do the transition on the object
                    obj.GetComponent<MorphController>().OnFlip(true);

                }
            }

            else if ( PlayerInReal() )
            { //if player is now in real world
                SetObjectToReal(obj); //set object to real layer

                if (type == ItemProperties.ObjectType.Morph)
                {
                    // if it's a morph obj EITHER on or off do the transition on the object
                    obj.GetComponent<MorphController>().OnFlip(false);

                }
            }

        }


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
				rm.RemoveFromList(obj, true);
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
        ItemProperties.ObjectType type = obj.GetComponent<ItemProperties>().objectType;

        if ((type == ItemProperties.ObjectType.Sokoban1x1) || (type == ItemProperties.ObjectType.Sokoban2x2)){
            GameObject child = obj.transform.GetChild(0).gameObject;
            child.layer = 10;
        }

	}
	void SetObjectToReal(GameObject obj)
	{
		obj.layer = 11;
		obj.transform.parent = Toolbox.Instance.GetRealWorldParent();
        ItemProperties.ObjectType type = obj.GetComponent<ItemProperties>().objectType;

 
        if ((type == ItemProperties.ObjectType.Sokoban1x1) || (type == ItemProperties.ObjectType.Sokoban2x2))
        {
            GameObject child = obj.transform.GetChild(0).gameObject;
            child.layer = 11;
        }

	}
}
