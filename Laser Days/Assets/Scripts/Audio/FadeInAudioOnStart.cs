using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAudioOnStart : MonoBehaviour {

    [SerializeField]
    private float fadeTime;

    private AudioSource _audio;

	// Use this for initialization
	void Start () {
        _audio = GetComponent<AudioSource>();

        StartCoroutine(DoFadeIn());
	}
	
    private IEnumerator DoFadeIn()
    {
        var elapsedTime = 0f;
        var ratio = elapsedTime / fadeTime;

        while (elapsedTime < fadeTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / fadeTime;
            _audio.volume = ratio;
        }

        _audio.volume = 1;
    }
}
