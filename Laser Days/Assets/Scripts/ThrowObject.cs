using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
    public Transform player;
    public Transform playerCam;
    public float throwForce = 10;
    bool beingCarried = false;
    public bool selected = false;
    public AudioClip[] soundToPlay;
    private AudioSource audio;
    public int dmg;
    private bool touched = false;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (beingCarried)
        {
            if (touched)
            {
                Drop();
                touched = false;
            }

            if (Input.GetMouseButtonDown(2))
            {
                Drop();
                GetComponent<Rigidbody>().AddForce(playerCam.forward * throwForce);
                
                RandomAudio();
                // uncomment RandomAudio() when we have sounds implemented
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Drop();
            }
        }
    }

    void RandomAudio()
    {
        if (audio.isPlaying){
            return;
        }
        
        audio.clip = soundToPlay[Random.Range(0, soundToPlay.Length)];
        audio.Play();

    }

    void OnTriggerEnter()
    {
        if (beingCarried)
        {
            touched = true;
        }
    }

    public void PickUp()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.parent = playerCam;
        GetComponentInParent<RaycastManager>().heldObject = this.gameObject;
        beingCarried = true;
    }

    public void Drop()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponentInParent<RaycastManager>().heldObject = null;
        transform.parent = null;
        beingCarried = false;
    }
}