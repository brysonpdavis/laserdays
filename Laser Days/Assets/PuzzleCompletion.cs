﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCompletion : MonoBehaviour {

    public Spawner[] mySpawners;
    public int counter = 0;

    // Use this for initialization
	void Start () {
        mySpawners = transform.parent.GetComponentsInChildren<Spawner>();
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            counter++;
            foreach (Spawner button in mySpawners)
            {
                button.OnPuzzleCompletion();
            }

            if (counter == 1)
            {
                AudioClip clip = SoundBox.Instance.completionZone;
                AudioSource audio = SoundBox.Instance.thisSource;
                audio.clip = clip;
                audio.volume = Toolbox.Instance.soundEffectsVolume;
                audio.Play();
            }
        }


    }

}
