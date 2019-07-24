using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredAudio : MonoBehaviour {

    AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        Toolbox.Instance.GetPlayer().GetComponent<SoundTrackManager>().mute = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audio.maxDistance = 500;
            audio.spatialBlend = 0f;
            audio.rolloffMode = AudioRolloffMode.Custom;
        }
    }


}
