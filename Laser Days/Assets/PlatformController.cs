using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    
    public bool isGroup;
    public List<GameObject> triggers = new List<GameObject>();
    public PlatformMover[] platformMovers;
    //public LineRenderer[] lines;


    [ColorUsageAttribute(true, true)]
    public Color PassiveColor = new Color(0f, 0f, 0f, 0f);

    [ColorUsageAttribute(true, true)]
    public Color ActiveColor = new Color(0f, 0f, 0f, 0f);

    private void Awake()
    {
        
        platformMovers = GetComponentsInChildren<PlatformMover>();

        foreach (PlatformMover plat in platformMovers)
        {
            plat.platformContainer = this.gameObject;
        }

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

