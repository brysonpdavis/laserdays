using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.AccessControl;

public class LightingMembraneZone : MonoBehaviour
{

    public LightingMembrane membrane;

    private void Start()
    {
        membrane = GetComponentInChildren<LightingMembrane>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            membrane.active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            membrane.active = false;
        }
    }

}
