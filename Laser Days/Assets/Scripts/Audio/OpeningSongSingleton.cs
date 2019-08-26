using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningSongSingleton : MonoBehaviour {

    public static AudioSource Instance;
    public static OpeningSongSingleton InstanceObject;
    AudioSource audio;
    float ambientPercentage;
    public float volume = 1f;
    [SerializeField] private float fadeDuration = 2;
	// Use this for initialization
	void Awake () {
        audio = GetComponent<AudioSource>();

        if (audio)
        {
            if (Instance != null && Instance != audio)
            {
                Destroy(gameObject);
            }

            else
            {
                InstanceObject = this;
                Instance = audio;
            }
        }

        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
/*
        ambientPercentage = 1 - AmbientSound.AmbientPercentage();
        audio.volume = volume * ambientPercentage;
*/

    }

    public static void FadeThenDestroy()
    {
        InstanceObject.StartCoroutine(InstanceObject.Fade());
    }

    public IEnumerator Fade()
    {
        var ratio = 0f;

        var elapsedTime = 0f;

        while (ratio < 1)
        {
            yield return null;
            
            elapsedTime += Time.deltaTime;

            ratio = elapsedTime / fadeDuration;

            audio.volume = 1 - ratio;
        }
        
        Destroy(gameObject);
    }
}
