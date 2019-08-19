using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLoopMode : MonoBehaviour {
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundTrackManager.Instance.loopMode = false;
        }
    }

}
