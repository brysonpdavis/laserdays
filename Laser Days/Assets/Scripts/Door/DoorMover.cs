using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMover : MonoBehaviour {

    public bool jammed;
    public DoorController controller;
    public bool leftDoor;



	// Use this for initialization
	void Start () {
        controller = GetComponentInParent<DoorController>();
	}
	
    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(MoveDoorCoroutine(true, controller.duration));
    }

    public void Close ()
    {
        StopAllCoroutines();
        StartCoroutine(MoveDoorCoroutine(false, controller.duration));

    }

    public void Jam ()
    {
        StopAllCoroutines();
    }

    private IEnumerator MoveDoorCoroutine(bool direction, float duration)
    {

        float end;
        float start = transform.localPosition.x;
        float elapsedTime = 0;
        float scaledDuration;


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
            scaledDuration = duration * ((Mathf.Abs(start)/.8f));
        }


        Vector3 newposition = new Vector3 (start, 0f, 0f);
        float ratio = elapsedTime / scaledDuration;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / scaledDuration;
            newposition.x = Mathf.Lerp(start, end, ratio);
            this.transform.localPosition = newposition;
            yield return null;
        }
        yield return null;
    }

}
