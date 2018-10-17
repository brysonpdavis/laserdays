using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBox : MonoBehaviour {

    public AudioSource thisSource;

   // public AudioClip[] flipClips;
   // public AudioClip[] flipClipsSecondary;
    public MFPP.FlipClipAsset currentFlipPalette;

    //public AudioClip[,] audioClipsTest;

    public AudioClip flipFail;
    public AudioClip sokobanClickOn;
    public AudioClip sokobanClickOff;
    public AudioClip sokobanShift;

    public AudioClip pickup;
    public AudioClip selection;
    public AudioClip deselect;


    public AudioClip platformOn;
    public AudioClip platformOff;

    public AudioClip threshold;
    public AudioClip[] booster;

    public AudioClip completionZone;

    public AudioClip pop;

    public void Start()
    {
        thisSource = GetComponent<AudioSource>();
    }

    public AudioClip PlayBoost()
    {
        return booster[Random.Range(0, booster.Length)];
    }


}
