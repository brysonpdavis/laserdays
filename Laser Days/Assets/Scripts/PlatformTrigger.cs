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
        platform.GetComponent<PlatformMover>().MovePlatform(start.position, end.position, time);
    }
}
