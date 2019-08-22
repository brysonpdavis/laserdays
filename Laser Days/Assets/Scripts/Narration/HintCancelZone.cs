using System;
using UnityEngine;

public class HintCancelZone : MonoBehaviour
{

    [SerializeField]
    private TextNarration _target;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _target.CancelHint();
            NarrationController.CancelNarration();
        }
    }
}
