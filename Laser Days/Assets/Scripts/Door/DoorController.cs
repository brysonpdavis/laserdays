using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {
    public DoorMover[] doorMovers;
    public float duration = 2f;

    public bool active = false;

    // Use this for initialization
    private void Awake()
    {
        doorMovers = GetComponentsInChildren<DoorMover>();
    }

    public void OpenAll()
    {
        active = true;
        foreach (DoorMover door in doorMovers)
        {
            door.Open();
        }
    }

    public void CloseAll()
    {
        active = false;
        foreach (DoorMover door in doorMovers)
        {
            if (!door.jammed)
            {
                door.Close();
            }
        }
    }


}
