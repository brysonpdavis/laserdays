﻿using System.Collections;
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
    public float ScrollSpeed = 0.4f;
    public bool on = false;
    public bool moving = false;

    private AudioClip platformOn;
    private AudioClip platformOff;
    private AudioSource audioSource;
    private Material RenderMat;

    private BasinTriggerIndicator basinIndicator;

    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        SoundBox box = Toolbox.Instance.GetPlayer().GetComponent<SoundBox>();
        platformOn = SoundBox.Instance.platformOn;
        platformOff = SoundBox.Instance.platformOff;

        if (this.gameObject.layer == platformContainer.layer)
        {
            platformTriggers = platformContainer.GetComponentsInChildren<PlatformTrigger>();
            totalTriggers = platformTriggers.Length;
        }

        else 
        {
            platformTriggers = platformContainer.GetComponentsInChildren<PlatformTrigger>();
            //redundent code temporarily, all triggers are in platform container folder now
            //leaving this in case anything breaks
            //platformTriggers = transform.parent.GetComponentsInChildren<PlatformTrigger>();
            totalTriggers = platformTriggers.Length;
        }


        RenderMat = GetComponent<Renderer>().material;
        RenderMat.SetInt("_Animated", 1);
        RenderMat.SetColor("_RestingColor", platformContainer.GetComponent<PlatformController>().RestingColor);
        RenderMat.SetColor("_ActiveColor", platformContainer.GetComponent<PlatformController>().ActiveColor);


        if (platformContainer){
            platform = platformContainer.GetComponentsInChildren<PlatformMover>();
        }
        else {
            platform = transform.parent.GetComponentsInChildren<PlatformMover>();

        }

        basinIndicator = GetComponentInChildren<BasinTriggerIndicator>();
    }

	private void Update()
	{
        var temp = RenderMat.GetFloat("_Elapsed");
        temp += (Time.deltaTime * ScrollSpeed);
        RenderMat.SetFloat("_Elapsed", temp);
	}


	private void OnTriggerEnter(Collider other)
    {
        TriggerOn(other);
    }


    private void OnTriggerExit(Collider other)
    {
        TriggerOff(other);
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
            if (((end.position.y >= start.y) && platformSingle.mainGuard.GetComponent<PlatformGuard>().breakingObjectsAbove.Count == 0)
                || platformSingle.mainGuard.GetComponent<PlatformGuard>().stuckSokoban.Count == 0)
            {
                //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated
                platformSingle.MovePlatform(start, end.position, time);
            }

        }


    }

    public void MovePlatformToStart () 
    {

        //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated

        foreach (PlatformMover platformSingle in platform)
        {
            Transform end = platformSingle.end;
            Vector3 start = platformSingle.start;
            platformSingle.StopAllCoroutines();
            platformSingle.IndicatorOff();


            //check to be sure either we're goin up or we're going down and nothing is stuck beneath the platform
            if ((( start.y >= end.position.y) && platformSingle.mainGuard.GetComponent<PlatformGuard>().breakingObjectsAbove.Count == 0) 
                || platformSingle.mainGuard.GetComponent<PlatformGuard>().stuckSokoban.Count == 0)
            {
                platformSingle.MovePlatform(end.position, start, time);

            }
        }

        foreach (PlatformTrigger trigger in platformTriggers)
        {
            trigger.moving = false;

        }

        Toolbox.Instance.SetVolume(audioSource);
        audioSource.clip = platformOff;
        audioSource.Play();
    }

    public void TriggerOn(Collider other)
    {
        if (other.tag == "Clickable" || other.tag == "Player" || other.tag == "Platform" || other.tag == "NoTouch" || other.tag == "MorphArm")
        {

            counter += 1;
            on = true;


            if (counter == 1)
            {
                //sound effect
                Toolbox.Instance.SetVolume(audioSource);
                audioSource.clip = platformOn;
                audioSource.Play();
            }

            int checkNumber = 0;
            RenderMat.SetInt("_isCollide", 1);

            if (GetComponentInChildren<PuddleTrigger>())
            {
                GetComponentInChildren<PuddleTrigger>().Activate();
            }

            ScrollSpeed *= -0.5f;
            if (basinIndicator) { basinIndicator.Collide(); }


            foreach (PlatformTrigger trigger in platformTriggers)
            {

                if (trigger.on)
                {
                    checkNumber += 1;
                }
            }

            //make sure all necessary triggers are selected. 
            if (checkNumber == totalTriggers)
            {
                MovePlatformToEnd();

                foreach (PlatformTrigger trigger in platformTriggers)
                {
                    //trigger.RenderMat.SetInt("_isActive0", 1);
                    //trigger.RenderMat.SetInt("_isActive1", 1);
                    if (trigger.basinIndicator)
                    {
                        trigger.basinIndicator.Activate();
                    }
                }
            }
        }  
    }
    public void TriggerOff(Collider other)
    {
        if (other.tag == "Clickable" || other.tag == "Player" || other.tag == "Platform" || other.tag == "MorphArm" || other.tag == "NoTouch")
        {

            counter -= 1;
            if (counter == 0)
            {
                MovePlatformToStart();
                RenderMat.SetInt("_isCollide", 0);

                if (GetComponentInChildren<PuddleTrigger>())
                {
                    GetComponentInChildren<PuddleTrigger>().Deactivate();
                }


                ScrollSpeed *= -2f;
                if (basinIndicator) { basinIndicator.UnCollide(); }

                foreach (PlatformTrigger trigger in platformTriggers)
                {
                    trigger.RenderMat.SetInt("_isActive0", 0);
                    trigger.RenderMat.SetInt("_isActive1", 0);
                    if (trigger.basinIndicator)
                    {
                        trigger.basinIndicator.Deactivate();
                    }
                }

                foreach (PlatformTrigger trigger in platformTriggers)
                {
                    trigger.moving = false;
                }

                on = false;
            }
        }
    }

}
