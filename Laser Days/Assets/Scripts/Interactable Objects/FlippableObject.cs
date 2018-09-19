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
    }

    public virtual int TimesFlipped { get { return timesFlipped; } }
}
