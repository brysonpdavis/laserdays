using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour {

    public GameObject platform;
    public Transform start;
    public Transform end;
    public float time = 3f;
    private int counter;

    private AudioClip platformOn;
    private AudioClip platformOff;
    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SoundBox box = Toolbox.Instance.GetPlayer().GetComponent<SoundBox>();
        platformOn = box.platformOn;
        platformOff = box.platformOff;

    }


    private void OnTriggerEnter(Collider other)
    {
        counter += 1;
        if (counter ==1)
        {
            //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated
            platform.GetComponent<PlatformMover>().MovePlatform(start.position, end.position, time);

            //sound effect
            audioSource.clip = platformOn;
            audioSource.Play();

        }
    }


    private void OnTriggerExit(Collider other)
    {
        counter -= 1;
        if (counter == 0)
        {
            //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated
            platform.GetComponent<PlatformMover>().MovePlatform(end.position, start.position, time);
            audioSource.clip = platformOff;
            audioSource.Play();
        }
    }

}
