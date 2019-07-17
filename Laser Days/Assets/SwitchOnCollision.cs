using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnCollision : MonoBehaviour {

    public bool onTriggerEnter = true;
    public bool onCollisionEnter;
    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (onCollisionEnter)
            Attempt(collision.gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (onTriggerEnter)
            Attempt(collider.gameObject);
    }

    private void Attempt (GameObject obj)
    {
        if (obj.GetComponent<FlippableObject>())
        {
            obj.GetComponent<FlippableObject>().ForcedFlip();
        }

        else if (obj.CompareTag("Player"))
        {
            Toolbox.Instance.GetFlip().FlipAttempt();
            Toolbox.Instance.GetFlip().ForceFlip();
            Toolbox.Instance.SetVolume(audio);
            audio.Play();
        }
    }

}
