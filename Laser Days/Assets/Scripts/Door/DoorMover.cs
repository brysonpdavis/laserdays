using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMover : MonoBehaviour {

    public bool jammed;
    public DoorController controller;
    public bool leftDoor;
    private AudioSource audio;


	// Use this for initialization
	void Start () {
        controller = GetComponentInParent<DoorController>();
        gameObject.AddComponent<AudioSource>();
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.clip = SoundBox.Instance.doorActive;
        audio.loop = true;
	}
	
    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(MoveDoorCoroutine(true, controller.duration));
    }

    public void Close ()
    {
        audio.mute = true;
        StopAllCoroutines();
        StartCoroutine(MoveDoorCoroutine(false, controller.duration));

    }

    public void Jam ()
    {
        StopAllCoroutines();
        audio.mute = true;
        //audio.Stop();
    }

    private IEnumerator MoveDoorCoroutine(bool direction, float duration)
    {

        float end;
        float start = transform.localPosition.x;
        float elapsedTime = 0;
        float scaledDuration;

        audio.volume = Toolbox.Instance.soundEffectsVolume;
        audio.mute = false;
        audio.Play();

        if (direction) //meaning we're opening
        {
            if (leftDoor)
            {
                end = .8f;
                scaledDuration = Mathf.Abs(duration * ((start - 0.8f) / .8f));
            }

            else
            {
                end = -.8f;
                scaledDuration = Mathf.Abs(duration * ((start + 0.8f) / .8f));
            }


        }
        else //door is closing
        {
            end = 0f;
            scaledDuration = duration * ((Mathf.Abs(start) / .8f));
        }


        Vector3 newposition = new Vector3(start, 0f, 0f);
        float ratio = elapsedTime / scaledDuration;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / scaledDuration;
            newposition.x = Mathf.Lerp(start, end, TweeningFunctions.EaseIn(ratio));
            this.transform.localPosition = newposition;

            yield return null;
            if ((gameObject.layer != 17) && !(gameObject.layer + 5 == Toolbox.Instance.GetPlayer().layer))
            {
                audio.mute = true;
            }
            else
                audio.mute = false;


        }
        yield return null;
        audio.mute = true;
        yield return new WaitForSeconds(.5f);
        audio.Stop();
    }

}
