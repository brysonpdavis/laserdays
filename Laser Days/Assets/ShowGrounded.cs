using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGrounded : MonoBehaviour {
    private CharacterController character;
    public bool grounded;


    // Use this for initialization
    void Start()
    {
        character = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (character.isGrounded) { grounded = true; }
        else 
            grounded = false;
	}
}
