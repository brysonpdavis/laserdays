using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredAudio : MonoBehaviour {

    AudioSource audio;
    public AudioSource whisperTone;

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

            StartCoroutine(VolumeFade(whisperTone, 0f, 1f, 0f));
            StartCoroutine(VolumeFade(audio, 1f, 3f, 1f));
        }
    }

    IEnumerator VolumeFade(AudioSource _AudioSource, float _EndVolume, float _FadeLength, float waitTime)
    {

        float _StartVolume = _AudioSource.volume;
        float _StartTime = Time.time;

        while (Time.time < (_StartTime + _FadeLength))
        {

            _AudioSource.volume = _StartVolume + ((_EndVolume - _StartVolume) * ((Time.time - _StartTime) / _FadeLength));
            Debug.Log("FADE " + _AudioSource.volume);
            yield return null;
        }

        if (_EndVolume == 0f) { _AudioSource.Stop(); }

    }


}
