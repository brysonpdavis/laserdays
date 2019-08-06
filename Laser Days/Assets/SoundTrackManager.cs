﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundTrackManager : MonoBehaviour {

    public AudioSource audioSource;
    public AudioSource LaserChords;
    public AudioSource RealChords;
    public AudioSource bass;
    public MFPP.FlipClipAsset flipClip;
    public float chordFadeDuration = 3f;
    public bool play;
    public bool mute;
    private int currentChord;
    public int counter = 0;
    public Slider mainSlider;

    private IEnumerator chordFade;
    private float globalVolume;
    private float ambientPercentage;
    private float realLevel;
    private float laserLevel;



	// Use this for initialization
	void Start () {

        audioSource = GetComponent<AudioSource>();
        mainSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        //secondarySource = GetComponentInChildren<AudioSource>();
        globalVolume = mainSlider.value;
        ambientPercentage = AmbientSound.AmbientPercentage();
        if (Toolbox.Instance.PlayerInLaser())
        {
            laserLevel = 1;
            realLevel = 0;
        }
        else
        {
            laserLevel = 0;
            realLevel = 1;
        }
    }
    
    public void ValueChangeCheck()
    {
        SetVolume();   
    }

    private void OnDisable()
    {
        play = false;
    }

    // Update is called once per frame
    void Update () {
        if (!mute && !play) { StartCoroutine(Soundtrack()); 
            play = true;
        }

        if (!mute && play)
        {
            SetVolume();
        }

	}

    public void SetVolume()
    {
        globalVolume = mainSlider.value;
        ambientPercentage =  AmbientSound.AmbientPercentage();

        audioSource.volume = globalVolume * (1 - ambientPercentage);
        LaserChords.volume = globalVolume * (1 - ambientPercentage) * laserLevel;
        RealChords.volume = globalVolume * (1 - ambientPercentage) * realLevel;
        bass.volume = globalVolume * (1 - ambientPercentage);
    }

    private IEnumerator Soundtrack()
    {

        if (counter <= 8)
        {
            //play random set chord tones
            currentChord = Random.Range(0, flipClip.defaultFlipClips.Count);
            LaserChords.clip = flipClip.defaultFlipClips[currentChord].backgroundSound;
            RealChords.clip = flipClip.defaultFlipClips[currentChord].backgroundSoundAlt;
            LaserChords.Play();
            RealChords.Play();
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
            LaserChords.clip = flipClip.section2[currentChord].backgroundSound;
            RealChords.clip = flipClip.section2[currentChord].backgroundSoundAlt;
            LaserChords.Play();
            RealChords.Play();
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
                yield return new WaitForSeconds(3);
                counter = 0;
            }
            yield return null;
        }

    }


    public void PlayPrimary(){

        MFPP.Audio.Play(flipClip.defaultFlipClips[currentChord].GetRandomFlipClip(), Toolbox.Instance.soundEffectsVolume, 1f);
    }


    public void PlaySecondary()
    {
        if (counter <= 8)
        {
            MFPP.Audio.Play(flipClip.defaultFlipClips[currentChord].GetRandomFlipSecondary(), Toolbox.Instance.soundEffectsVolume, 1f);

        }
        else
        { 
            MFPP.Audio.Play(flipClip.section2[currentChord].GetRandomFlipSecondary(), Toolbox.Instance.soundEffectsVolume, 1f);
         }

    }

    public void FadeBetween(bool direction)
    {
        if (chordFade != null)
            StopCoroutine(chordFade);
        chordFade = ChordFade(direction);
        StartCoroutine(chordFade);
    }
    
    private IEnumerator ChordFade(bool direction)
    {
        float elapsedTime = 0;
        float ratio = 0;

        float realChordStart = realLevel;
        float laserChordStart = laserLevel;
        float realEnd;
        float laserEnd;

        if (direction)
        {
            realEnd = 1f;
            laserEnd = 0f;
        }
        else
        {
            laserEnd = 1f;
            realEnd = 0f;
        }

        while (ratio < 1f)
        {
            ratio = elapsedTime / chordFadeDuration;
            float realValue = Mathf.Lerp(realChordStart, realEnd, ratio);
            float laserValue = Mathf.Lerp(laserChordStart, laserEnd, ratio);

            realLevel = realValue;
            laserLevel = laserValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }



}
