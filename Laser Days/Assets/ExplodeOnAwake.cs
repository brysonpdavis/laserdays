using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnAwake : MonoBehaviour {

    public void Explode(float intensity)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddExplosionForce(intensity + (Random.Range(-1f, 1f)), transform.parent.position, 10, 3f, ForceMode.Impulse);
    }

    //A

}
