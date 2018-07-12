using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour {

    public GameObject platform;
    public Transform start;
    public Transform end;
    public int time = 3;


    private void OnTriggerEnter(Collider other)
    {
        //make sure that the platform is at the same position as either the start or end position, otherwise it won't be activated
        platform.GetComponent<PlatformMover>().MovePlatform(start.position, end.position, time);
    }
}
