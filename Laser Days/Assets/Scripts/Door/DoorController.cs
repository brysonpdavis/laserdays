using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {
    public DoorMover[] doorMovers;
    public DoorTrigger[] doorTriggers;
    public float duration = 2f;

    public DoorIndicator[] doorIndicators;

    public bool active = false;

    [ColorUsageAttribute(true, true)]
    public Color RestingColor = new Color(0f, 0f, 0f, 0f);

    [ColorUsageAttribute(true, true)]
    public Color ActiveColor = new Color(0f, 0f, 0f, 0f);

    [ColorUsageAttribute(true, true)]
    public Color ShimmerColor = new Color(0f, 0f, 0f, 0f);

    // Use this for initialization
    private void Awake()
    {
        doorMovers = GetComponentsInChildren<DoorMover>();
        doorTriggers = GetComponentsInChildren<DoorTrigger>();
        doorIndicators = GetComponentsInChildren<DoorIndicator>();

    }

    private void Start()
    {


        foreach (DoorIndicator indicator in doorIndicators)
        {
            indicator.SetColors(RestingColor, ActiveColor, ShimmerColor);
        }
    }

    public void OpenAll()
    {
        active = true;
        foreach (DoorIndicator indicator in doorIndicators)
        {
            indicator.On();
        }
        foreach (DoorMover door in doorMovers)
        {
            door.Open();
        }
    }

    public void CloseAll()
    {
        active = false;
        foreach (DoorIndicator indicator in doorIndicators)
        {
            indicator.Off();
        }
        foreach (DoorMover door in doorMovers)
        {
            if (!door.jammed)
            {
                door.Close();
            }
        }
    }


}
