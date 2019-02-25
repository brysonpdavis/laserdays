using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionBurst : MonoBehaviour {

    public int particleCount = 20;
    public float radialSpeed = 0.0f;
    public float lifeMultiplier = 1.0f;

    private ParticleSystem pS;

	void Start () {
        pS = GetComponent<ParticleSystem>();
	}
	

    public void DoBusrt () {
        var mainModule = pS.main;
        mainModule.maxParticles = particleCount;
        mainModule.loop = false;

        var minMax = mainModule.startLifetime;
        mainModule.startLifetime = new ParticleSystem.MinMaxCurve(minMax.constantMin * lifeMultiplier, minMax.constantMax * lifeMultiplier);
        

        var velocityModule = pS.velocityOverLifetime;
        velocityModule.radial = radialSpeed;

        var emissionModule = pS.emission;
        var internalTime = pS.time;
        var thisBurst = new ParticleSystem.Burst(internalTime + Time.fixedDeltaTime, (short)particleCount, (short)particleCount, 1, 0f);
        emissionModule.SetBursts(new ParticleSystem.Burst[] { thisBurst });
        emissionModule.rateOverTime = 0.0f;
        
    }
}
