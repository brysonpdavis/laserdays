using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    
    public bool isGroup;
    public List<GameObject> triggers = new List<GameObject>();
    public PlatformMover[] platformMovers;

    [ColorUsageAttribute(true, true)]
    public Color PassiveColor = new Color(0f, 0f, 0f, 0f);

    [ColorUsageAttribute(true, true)]
    public Color ActiveColor = new Color(0f, 0f, 0f, 0f);

    private void Start()
    {
        platformMovers = GetComponentsInChildren<PlatformMover>();
    }

    public void StopPlatforms()
    {
        foreach (PlatformMover platform in platformMovers)
        {
            platform.StopAllCoroutines();
         //   platform.PlatformObjectSelectable();
          //  Debug.Log(platform.name);
        }
    }
}

