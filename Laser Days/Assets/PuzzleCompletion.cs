using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCompletion : MonoBehaviour {

    public Spawner[] mySpawners;

    // Use this for initialization
	void Start () {
        mySpawners = transform.parent.GetComponentsInChildren<Spawner>();
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Spawner button in mySpawners)
            {
                button.OnPuzzleCompletion();
            }
        }
    }

}
