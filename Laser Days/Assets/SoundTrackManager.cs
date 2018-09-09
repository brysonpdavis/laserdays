using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrackManager : MonoBehaviour {

    public AudioSource audioSource;
    public AudioSource secondarySource;
    public AudioSource bass;
    public MFPP.FlipClipAsset flipClip;
    public bool play;
    private int currentChord;
    private int counter = 0;

	// Use this for initialization
	void Start () {

        audioSource = GetComponent<AudioSource>();
        //secondarySource = GetComponentInChildren<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!play) { StartCoroutine(Soundtrack()); }
        play = true;



	}

    private IEnumerator Soundtrack()
    {
        //play random set chord tones
        currentChord = Random.Range(0, flipClip.defaultFlipClips.Count);
        secondarySource.clip = flipClip.defaultFlipClips[currentChord].backgroundSound;
        secondarySource.Play();
        counter += 1;

        if (counter > 4)
        {
            //play random bass tone within the set chord tone
            bass.clip = flipClip.defaultFlipClips[currentChord].GetRandomBass();
            bass.Play();
        }

        yield return new WaitForSeconds(flipClip.defaultFlipClips[currentChord].backgroundSound.length);
        play = false;
        yield return null;
    }

    public void PlayPrimary(){

        audioSource.clip = flipClip.defaultFlipClips[currentChord].GetRandomFlipClip();
        audioSource.Play();
    }


    public void PlaySecondary()
    {

        audioSource.clip = flipClip.defaultFlipClips[currentChord].GetRandomFlipSecondary();
        audioSource.Play();

    }





}
