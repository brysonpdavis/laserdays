using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipAbilityTrigger : MonoBehaviour {

    public bool allowFlip = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (allowFlip)
                Toolbox.Instance.GetFlip().canFlip = true;
            else
                Toolbox.Instance.GetFlip().canFlip = false;
        }
            
    }
}
