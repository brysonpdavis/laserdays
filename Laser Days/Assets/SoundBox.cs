using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBox : Singleton<SoundBox> {

    public AudioSource thisSource;

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

    public void Start()
    {
        thisSource = GetComponent<AudioSource>();
    }

    public AudioClip PlayBoost()
    {
        return booster[Random.Range(0, booster.Length)];
    }


}
