using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour {

    public GameObject platformContainer;
    private PlatformMover[] platform;

    //public Transform start;
    //public Transform end;
    public float time = 3f;
    public int counter;
    private PlatformTrigger[] platformTriggers;
    public int totalTriggers;
    public bool on = false;
    public bool moving = false;

    private AudioClip platformOn;
    private AudioClip platformOff;
    private AudioSource audioSource;



    private void Start()
    {
        platformContainer.GetComponent<PlatformController>().triggers.Add(this.gameObject);

        audioSource = GetComponent<AudioSource>();
        SoundBox box = Toolbox.Instance.GetPlayer().GetComponent<SoundBox>();
        platformOn = box.platformOn;
        platformOff = box.platformOff;

        platformTriggers = transform.parent.GetComponentsInChildren<PlatformTrigger>();
        totalTriggers = platformTriggers.Length;

        if (platformContainer){
            platform = platformContainer.GetComponentsInChildren<PlatformMover>();
        }
        else {
            platform = transform.parent.GetComponentsInChildren<PlatformMover>();

        }
        

    }


    private void OnTriggerEnter(Collider other)
    {
        counter += 1;
        on = true;


        if (counter == 1){
            //sound effect
            audioSource.clip = platformOn;
            audioSource.Play();
        }

        int checkNumber = 0;

        GetComponent<Renderer>().material.SetInt("_IsSelected", 1);


        foreach (PlatformTrigger trigger in platformTriggers) {

            if (trigger.on) {
                checkNumber += 1;
            }
        }

        if (checkNumber==totalTriggers)
        {
            MovePlatformToEnd();

        }
    }


    private void OnTriggerExit(Collider other)
    {
        counter -= 1;
        if (counter == 0)
        {
            MovePlatformToStart();
        }
    }


    public void MovePlatformToEnd () 
    {

        foreach (PlatformMover platformSingle in platform)
        {

            Transform end = platformSingle.end;
            Vector3 start = platformSingle.start;

            //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated
            platformSingle.MovePlatform(start, end.position, time);

        }

        foreach (PlatformTrigger trigger in platformTriggers)
        {
            moving = true;

        }
    }

    public void MovePlatformToStart () 
    {
        GetComponent<Renderer>().material.SetInt("_IsSelected", 0);

        on = false;
        //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated

        foreach (PlatformMover platformSingle in platform)
        {
            Transform end = platformSingle.end;
            Vector3 start = platformSingle.start;
            platformSingle.StopAllCoroutines();
            platformSingle.MovePlatform(end.position, start, time);
        }

        foreach (PlatformTrigger trigger in platformTriggers)
        {
            moving = false;

        }

        audioSource.clip = platformOff;
        audioSource.Play();
    }



}
