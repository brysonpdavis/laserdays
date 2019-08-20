using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternTrigger : MonoBehaviour {

    bool done = false;
    public Light light;
    public AudioClip clip;

    private void OnTriggerEnter(Collider other)
    {
        if (!done && other.gameObject.CompareTag("Player"))
        {
            done = true;
            AudioSource audio = GetComponent<AudioSource>();

            audio.clip = clip;
            audio.Play();
            light.intensity += .5f;
        }
    }
}
