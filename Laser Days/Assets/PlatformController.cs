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

    private void Start()
    {
        
        platformMovers = GetComponentsInChildren<PlatformMover>();
        //lines = new LineRenderer[2];

        //for (int i = 0; i < platformMovers.Length; i++)
        //{
        //    lines[i] = gameObject.AddComponent<LineRenderer>();
        //    lines[i].positionCount = 2;
        //    lines[i].SetPosition(0, platformMovers[i].gameObject.transform.position);
        //    lines[i].SetPosition(1, new Vector3(20,20,20));


        //}

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

