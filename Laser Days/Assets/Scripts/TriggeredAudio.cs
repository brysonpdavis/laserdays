using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredAudio : MonoBehaviour {

    AudioSource audio;
    public AudioSource whisperTone;
    private AudioSource highFreq;
    public static AudioSource Instance;
    public float volume = 1f;
    public float ambientPercentage;


    private void Start()
    {
        audio = GetComponent<AudioSource>();

        if (Instance != null && Instance != audio)
        {
            Destroy(gameObject);
        }

        else Instance = audio;


        Toolbox.Instance.GetPlayer().GetComponentInChildren<SoundTrackManager>().mute = true;
        highFreq = OpeningSongSingleton.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audio.maxDistance = 500;
            audio.spatialBlend = 0f;
            audio.rolloffMode = AudioRolloffMode.Custom;

            StartCoroutine(VolumeFadeOther(whisperTone, 0f, 1f, 0f));
            StartCoroutine(VolumeFadeSelf(1f, 3f, 1f));

            audio.time = highFreq.time;
            audio.Play();
        }
    }

    private void Update()
    {
        ambientPercentage = 1- AmbientSound.AmbientPercentage();
        audio.volume = volume * ambientPercentage;

    }

    IEnumerator VolumeFadeOther(AudioSource _AudioSource, float _EndVolume, float _FadeLength, float waitTime)
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

    IEnumerator VolumeFadeSelf(float _EndVolume, float _FadeLength, float waitTime)
    {

        float _StartVolume = volume;
        float _StartTime = Time.time;

        while (Time.time < (_StartTime + _FadeLength))
        {

            float value = _StartVolume + ((_EndVolume - _StartVolume) * ((Time.time - _StartTime) / _FadeLength));
            volume = value;
            Debug.Log("FADE " + volume);
            yield return null;
        }

        //if (_EndVolume == 0f) { _AudioSource.Stop(); }
    }


}
