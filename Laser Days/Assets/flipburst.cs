using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flipburst : MonoBehaviour {

    protected ParticleSystem particleSystem;

	// Use this for initialization
	void Start () {
        particleSystem = GetComponent<ParticleSystem>();
	}
	
	public void Boom()
    {

        ParticleSystem.Burst burst = new ParticleSystem.Burst(.025f, 20);

        var main = particleSystem.main;
        main.startLifetime = 2f;
        //particleSystem.main.startLifetime = .5f;
        particleSystem.emission.SetBurst(0, burst);
        particleSystem.Play();
    }
}
