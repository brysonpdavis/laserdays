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

    private Material RenderMat;


    private void Start()
    {
        platformContainer.GetComponent<PlatformController>().triggers.Add(this.gameObject);

        audioSource = GetComponent<AudioSource>();
        SoundBox box = Toolbox.Instance.GetPlayer().GetComponent<SoundBox>();
        platformOn = box.platformOn;
        platformOff = box.platformOff;

        platformTriggers = transform.parent.GetComponentsInChildren<PlatformTrigger>();
        totalTriggers = platformTriggers.Length;

        RenderMat = GetComponent<Renderer>().material;
        RenderMat.SetColor("_PassiveColor", platformContainer.GetComponent<PlatformController>().PassiveColor);
        RenderMat.SetColor("_ActiveColor", platformContainer.GetComponent<PlatformController>().ActiveColor);


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
        RenderMat.SetInt("_isCollide", 1);


        foreach (PlatformTrigger trigger in platformTriggers) {

            if (trigger.on) {
                checkNumber += 1;
            }
        }

        //make sure all necessary triggers are selected. 
        if (checkNumber==totalTriggers)
        {
            MovePlatformToEnd();

            foreach (PlatformTrigger trigger in platformTriggers)
            {
                trigger.RenderMat.SetInt("_isActive0", 1);
                trigger.RenderMat.SetInt("_isActive1", 1);

            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
        counter -= 1;
        if (counter == 0)
        {
            MovePlatformToStart();
            RenderMat.SetInt("_isCollide", 0);

            foreach (PlatformTrigger trigger in platformTriggers)
            {
                trigger.RenderMat.SetInt("_isActive0", 0);
                trigger.RenderMat.SetInt("_isActive1", 0);

            }

            foreach (PlatformTrigger trigger in platformTriggers)
            {
                trigger.moving = false;

            }

            on = false;
        }
    }


    public void MovePlatformToEnd () 
    {

        foreach (PlatformMover platformSingle in platform)
        {
            Transform end = platformSingle.end;
            Vector3 start = platformSingle.start;
            platformSingle.IndicatorOn();


            foreach (PlatformTrigger trigger in platformTriggers)
            {
                trigger.moving = true;
            }

            //make sure either we're going up, or if we're going down that there's nobody beneath
            if ((end.position.y >= start.y) || platformSingle.mainGuard.GetComponent<PlatformGuard>().stuckSokoban.Count == 0)
            {
                //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated
                platformSingle.MovePlatform(start, end.position, time);
            }


        }


    }

    public void MovePlatformToStart () 
    {
        GetComponent<Renderer>().material.SetInt("_IsSelected", 0);

        //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated

        foreach (PlatformMover platformSingle in platform)
        {
            Transform end = platformSingle.end;
            Vector3 start = platformSingle.start;
            platformSingle.StopAllCoroutines();
            platformSingle.IndicatorOff();


            //check to be sure either we're goin up or we're going down and nothing is stuck beneath the platform
            if (( start.y >= end.position.y) || platformSingle.mainGuard.GetComponent<PlatformGuard>().stuckSokoban.Count == 0) 
            {
                platformSingle.MovePlatform(end.position, start, time);

            }
        }

        foreach (PlatformTrigger trigger in platformTriggers)
        {
            trigger.moving = false;

        }

        audioSource.clip = platformOff;
        audioSource.Play();
    }



}
