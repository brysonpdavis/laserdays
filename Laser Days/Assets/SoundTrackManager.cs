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

        if (counter <= 8)
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
    

    //second section
    else {
        //play random set chord tones
            currentChord = Random.Range(0, flipClip.section2.Count);
            secondarySource.clip = flipClip.section2[currentChord].backgroundSound;
            secondarySource.Play();
            counter += 1;

            if (counter > 12)
            {
                //play random bass tone within the set chord tone
                bass.clip = flipClip.section2[currentChord].GetRandomBass();
                bass.Play();
            }

            yield return new WaitForSeconds(flipClip.section2[currentChord].backgroundSound.length);
            play = false;
            if (counter == 20) {
                counter = 0;
            }
            yield return null;
        }

    }


    public void PlayPrimary(){

        if (counter < 8)
        {
            audioSource.clip = flipClip.defaultFlipClips[currentChord].GetRandomFlipClip();
        }

        else
        { audioSource.clip = flipClip.section2[currentChord].GetRandomFlipClip(); }

        audioSource.Play();
    }


    public void PlaySecondary()
    {
        if (counter < 8)
        {
            audioSource.clip = flipClip.defaultFlipClips[currentChord].GetRandomFlipSecondary();

        }
        else
        { audioSource.clip = flipClip.section2[currentChord].GetRandomFlipSecondary(); }

        audioSource.Play();

    }





}
