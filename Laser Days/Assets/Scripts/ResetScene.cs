using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour {

	public string spawnName;
	public string sceneName;

    public AudioSource audio;
    public float fadeDuration = .3f;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.volume = Toolbox.Instance.soundEffectsVolume;
    }

    public void Activate()
    {
        Toolbox.Instance.LoadScene(sceneName, spawnName);
    }

    public void Play()
    {
        audio.volume = Toolbox.Instance.soundEffectsVolume;
        StopAllCoroutines();
        audio.Play();
    }

    public void Deactivate()
    {
        //audio.Stop();
        StartCoroutine(VolumeFade(audio, 0f, fadeDuration));
    }


    IEnumerator VolumeFade(AudioSource _AudioSource, float _EndVolume, float _FadeLength)
    {

        float _StartVolume = _AudioSource.volume;
        float _StartTime = Time.time;

        while (Time.time < _StartTime + _FadeLength)
        {

            _AudioSource.volume = _StartVolume + ((_EndVolume - _StartVolume) * ((Time.time - _StartTime) / _FadeLength));
            yield return null;

        }

        if ( _EndVolume == 0f) { _AudioSource.Stop(); }

    }
}
