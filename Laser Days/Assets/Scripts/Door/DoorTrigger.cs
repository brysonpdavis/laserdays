using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {

    public DoorController controller;
    public bool active = false;
    private int counter = 0;
    public float ScrollSpeed = 0.4f;
    private Material RenderMat;
    private AudioSource audio;
    private TriggerConnector[] connectors;


    private Vector2 minMaxScrollSpeed;
   
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        RenderMat = GetComponent<Renderer>().material;


        if (!controller)
            controller = GetComponentInParent<DoorController>();
    }

    private void Start()
    {
        RenderMat = GetComponent<Renderer>().material;
        RenderMat.SetInt("_Animated", 1);
        RenderMat.SetColor("_RestingColor", controller.RestingColor);
        RenderMat.SetColor("_ActiveColor", controller.ActiveColor);
        RenderMat.SetColor("_ShimmerColor", controller.ShimmerColor);
        RenderMat.SetTexture("_TriggerMap", controller.ScrollText);

        minMaxScrollSpeed = new Vector2(ScrollSpeed * -2, ScrollSpeed * 2);

        if (GetComponentInChildren<TriggerConnector>())
        {
            connectors = GetComponentsInChildren<TriggerConnector>();
            foreach (TriggerConnector conn in connectors)
            {
                conn.CreateConnector();
                conn.SetColors(controller.RestingColor, controller.ShimmerColor);
                conn.ChangeColor(controller.RestingColor);
                conn.SetWorld(this.gameObject.layer == 11);
            }
        }
    }

    private void Update()
    {
        var temp = RenderMat.GetFloat("_Elapsed");
        temp += (Time.deltaTime * ScrollSpeed);
        RenderMat.SetFloat("_Elapsed", temp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clickable"))
        {
            counter += 1;
            active = true;

            if (counter == 1)
            {
                SetConnectorStates(TriggerConnector.State.Active);

                RenderMat.SetFloat("_isCollide", 1f);
                RenderMat.SetFloat("_isActive", 1f);
                ScrollSpeed = Mathf.Clamp(ScrollSpeed * -2, minMaxScrollSpeed.x, minMaxScrollSpeed.y);
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
                RenderMat.SetFloat("_isCollide", 0f);
                RenderMat.SetFloat("_isActive", 0f);
                ScrollSpeed *= -0.5f;
                active = false;

                SetConnectorStates(TriggerConnector.State.Waiting);

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

    private void SetConnectorStates(TriggerConnector.State s)
    {
        if ((connectors != null) && connectors.Length > 0)
        {
            foreach (TriggerConnector connector in connectors)
            {
                connector.SetState(s);
            }
        }
    }


}
