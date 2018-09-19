using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FlippableObject : InteractableObject {

    public int timesFlipped = 0;
    public int maxFlips;

    public virtual void OnFlip() 
    {
        timesFlipped += 1;

        if (timesFlipped == maxFlips+1)
        {
            //this.gameObject.SetActive(false);
        }

        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem.Burst burst = new ParticleSystem.Burst(.025f, 50f);
            var main = particleSystem.main;
            main.startLifetime = .75f;
            particleSystem.emission.SetBurst(0, burst);
            particleSystem.Play();
        }
    }

    public virtual int TimesFlipped { get { return timesFlipped; } }
}
