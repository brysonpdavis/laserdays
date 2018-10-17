using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {

    public DoorController controller;
    public bool active = false;
    private int counter = 0;
    private Material RenderMat;
    private AudioSource audio;
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        RenderMat = GetComponent<Renderer>().material;


        if (!controller)
            controller = GetComponentInParent<DoorController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clickable"))
        {
            counter += 1;
            active = true;

            if (counter == 1)
            {
                RenderMat.SetInt("_isCollide", 1);
                controller.OpenAll();
                audio.clip = SoundBox.Instance.platformOn;
                Toolbox.Instance.SetVolume(audio);
                audio.Play();
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clickable"))
        {
            counter -= 1;
            if (counter == 0)
            {
                RenderMat.SetInt("_isCollide", 0);
                active = false;

                audio.clip = SoundBox.Instance.platformOff;
                Toolbox.Instance.SetVolume(audio);
                audio.Play();


                if (CheckOtherTriggers())
                    controller.CloseAll();

            }
        }
    }

    private bool CheckOtherTriggers ()
    {
        int triggerCheck = 0;
        foreach (DoorTrigger trig in controller.doorTriggers)
        {
            if (trig.active)
                triggerCheck += 1;
        }

        if (triggerCheck == 0)
            return true;

        else return false;
            
    }


}
