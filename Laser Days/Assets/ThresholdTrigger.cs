using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdTrigger : MonoBehaviour {

    public AudioClip customSound;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        GameObject player = Toolbox.Instance.GetPlayer();
        audioSource = player.GetComponent<AudioSource>();

        if (!customSound){
            customSound = player.GetComponent<SoundBox>().threshold;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        audioSource.clip = customSound;
        audioSource.Play();

    }

}
