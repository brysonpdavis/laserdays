﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneChangeTeleport : MonoBehaviour {

    float fadeDuration;
    AudioSource audio;

    //[SerializeField] private AudioSource[] audioSources;
    [SerializeField] private string sceneName;
    [SerializeField] private string spawnName;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            LoadScene();
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        fadeDuration = audio.clip.length;
    }

    private void LoadScene()
    {
        audio.Play();
        Toolbox.Instance.LoadScene(sceneName, spawnName);
        StartCoroutine(FadeOutVolume());
    }

    IEnumerator FadeOutVolume()
    {
        AudioSource playerAudio = Toolbox.Instance.GetSoundTrackAudio();

        float audioStart = playerAudio.volume;

        float elapsedTime = 0;
        float ratio = elapsedTime / fadeDuration;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeDuration;

            // audio fadeout
            float volumeFade = Mathf.Lerp(audioStart, 0f, ratio);

            OpeningSongSingleton.Instance.volume = volumeFade;
            TriggeredAudio.Instance.volume = volumeFade;

            Toolbox.Instance.GetPlayer().GetComponentInChildren<SoundTrackManager>().mute = false;
            
            yield return null;
        }
        
    }


}