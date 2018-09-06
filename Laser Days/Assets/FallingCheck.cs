using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCheck : MonoBehaviour {

    private CharacterController character;
    private Rigidbody rigidbody;
    public bool grounded;
    public Vector3 vel;

	// Use this for initialization
	void Start () {
        character = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {


        vel = character.velocity;

        if (character.isGrounded)
        {
            grounded = true;
        }

        else { grounded = false; }

	}
}
