using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMeshUpdateChild : MonoBehaviour {

    public int flipCountActivate;
    public int currentFlips = 0;

    private void Start()
    {
        if (currentFlips < flipCountActivate)
        {
            GetComponent<Transition>().enabled = false;
            GetComponent<Renderer>().enabled = false;
        }

    }

    public void Check()
    {
        currentFlips += 1;
        if (currentFlips == flipCountActivate)
            Activate();
    }

	public void Activate()
    {
        GetComponent<Transition>().enabled = true;
        GetComponent<Renderer>().enabled = true;
    }
}
