using System;
using UnityEngine;

public class SoftBoundary : MonoBehaviour
{
    [SerializeField]
    private float speedMultiplier;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MFPP.Player.SetBoundaryMultiplier(speedMultiplier);
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MFPP.Player.SetBoundaryMultiplier(1f);
        }
    }
}