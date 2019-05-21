using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionChildrenOnly : Transition {

    protected override void Awake()
    {
        childrenTransitions = GetComponentsInChildren<Transition>();   
    }

    protected override void Start()
    {
    }

    public override void Flip(float end, float duration)
    {
        foreach (Transition transition in childrenTransitions)
        {
            if (!transition.gameObject.Equals(this.gameObject) && !Toolbox.Instance.EqualToHeld(transition.gameObject))
            {
                transition.StopAllCoroutines();
                transition.Flip(end, duration);
            }
                
        }
    }

    public override void SetStart(float value)
    {
        
    }
}
