using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleTrigger : MonoBehaviour {

    private PlatformTrigger trigger;
    private Material RenderMat;
    public float ScrollSpeed = 0.4f;


    private void Awake()
    {
        trigger = GetComponentInParent<PlatformTrigger>();
        RenderMat = GetComponent<Renderer>().material;
    }
    private void OnTriggerEnter(Collider other)
    {
        trigger.TriggerOn(other);
    }

    private void OnTriggerExit(Collider other)
    {
        trigger.TriggerOff(other);
    }

    public void Activate()
    {
        ScrollSpeed *= -0.5f;
        RenderMat.SetInt("_isCollide", 1);
    }

    public void Deactivate()
    {
        ScrollSpeed *= -2f;
        RenderMat.SetInt("_isCollide", 0);
    }

    private void Update()
    {
        var temp = RenderMat.GetFloat("_Elapsed");
        temp += (Time.deltaTime * ScrollSpeed);
        RenderMat.SetFloat("_Elapsed", temp);
    }
}
