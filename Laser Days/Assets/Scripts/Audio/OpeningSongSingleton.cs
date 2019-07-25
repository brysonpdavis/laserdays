using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningSongSingleton : MonoBehaviour {

    public static AudioSource Instance;
    AudioSource audio;
	// Use this for initialization
	void Awake () {
        audio = GetComponent<AudioSource>();

        if (audio)
        {
            if (Instance != null && Instance != audio)
            {
                Destroy(gameObject);
            }

            else Instance = audio;
        }

        transform.parent = null;
        DontDestroyOnLoad(gameObject);

	}
	
}
