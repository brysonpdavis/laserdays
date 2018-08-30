using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    
    public bool isGroup;
    public List<GameObject> triggers = new List<GameObject>();
    public PlatformMover[] platformMovers;
    private void Start()
    {
        platformMovers = GetComponentsInChildren<PlatformMover>();
    }
}

