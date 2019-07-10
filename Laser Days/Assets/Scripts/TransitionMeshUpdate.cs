using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMeshUpdate : Transition {

    TransitionMeshUpdateChild[] children;
    public float distance;

    protected override void Awake()
    {
        children = GetComponentsInChildren<TransitionMeshUpdateChild>();
    }

    protected override void Start()
    {
        
    }

    public override void Flip(float end, float duration)
    {
        float value = Vector3.Distance(Toolbox.Instance.GetPlayer().transform.position, this.transform.position);
        Debug.Log("distance from player " + value);

        if (value <= distance)
        {
            foreach (TransitionMeshUpdateChild c in children)
            {
                c.Check();
            }
                
        }


        childrenTransitions = GetComponentsInChildren<Transition>();   

        foreach (Transition transition in childrenTransitions)
        {
            if (!transition.gameObject.Equals(this.gameObject) && !Toolbox.Instance.EqualToHeld(transition.gameObject))
            {
                transition.StopAllCoroutines();
                transition.Flip(end, duration);
            }

        }
    }

}
