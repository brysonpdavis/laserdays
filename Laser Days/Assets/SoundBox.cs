using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBox : MonoBehaviour {

    protected SoundBox() { } // guarantee this will be always a singleton only - can't use the constructor!

    public AudioSource thisSource;
    public static SoundBox instance = null;
    public static SoundBox Instance = null;

   // public AudioClip[] flipClips;
   // public AudioClip[] flipClipsSecondary;
    public MFPP.FlipClipAsset currentFlipPalette;

    //public AudioClip[,] audioClipsTest;

    public AudioClip flipFail;
    public AudioClip sokobanClickOn;
    public AudioClip sokobanClickOff;
    public AudioClip sokobanShift;
    public AudioClip sokoban2x2Running;

    public AudioClip pickup;
    public AudioClip selection;
    public AudioClip deselect;


    public AudioClip platformOn;
    public AudioClip  platformTriggered;
    public AudioClip platformOff;

    public AudioClip threshold;
    public AudioClip[] booster;

    public AudioClip completionZone;

    public AudioClip pop;

    public AudioClip platformStart;
    public AudioClip platformEnd;
    public AudioClip platformRunning;

    public AudioClip sokobanDrag;

    public AudioClip doorActive;

    public AudioClip narrationSound;
    

    private void Awake()
    {
        // DontDestroyOnLoad(this.gameObject);

        Instance = this;


        //Todo: have it delete the old instance, earlier this was breaking it for some reason
        //{
        //    //Check if instance already exists
        //    if (instance == null)
        //    {
        //        //if not, set instance to this
        //        instance = this;
        //        DontDestroyOnLoad(this.gameObject);
        //        Debug.Log("I AM INTIATED");
        //    }


        //    //If instance already exists and it's not this:
        //    else if (instance != this)

        //        //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
        //        Destroy(gameObject);
        //}


    }

    public void Start()
    {
        thisSource = GetComponent<AudioSource>();
    }

    public AudioClip PlayBoost()
    {
        return booster[Random.Range(0, booster.Length)];
    }


}
