using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOnCollision : MonoBehaviour {

    public bool onTriggerEnter = true;
    public bool onCollisionEnter;
    public float scrollspeed;

    Material rendermat;
    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        rendermat = GetComponent<Renderer>().material;
    }

    private void FixedUpdate()
    {
        rendermat.SetFloat("_Elapsed", rendermat.GetFloat("_Elapsed") + scrollspeed);
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
