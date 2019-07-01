using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifter : MonoBehaviour
{


    public enum ShiftType { Scale, Stretch, Jump, Rotate};
    public ShiftType type;
    bool state;
    public float duration = 1f;

    public void Activate()
    {
        if (type == ShiftType.Scale)
            Scale();
        else if (type == ShiftType.Stretch)
            Stretch();
        else if (type == ShiftType.Jump)
        {
            Jump();
            Scale();
        }
        else if (type == ShiftType.Rotate)
            Rotate();
    }

    void Scale()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleRoutine());
    }

    void Jump()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 1f, transform.localPosition.z);
    }

    void Rotate()
    {
        StopAllCoroutines();
        StartCoroutine(RotateRoutine());
    }

    void Stretch()
    {
        StopAllCoroutines();
        StartCoroutine(StretchRoutine());
    }

    IEnumerator ScaleRoutine()
    {
        state = !state;

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        float endpoint;
        if (state)
            endpoint = 2f;
        else
            endpoint = 1f;

        float startpoint = transform.localScale.x;
            

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, TweeningFunctions.EaseInOut(ratio));

            transform.localScale = new Vector3(value, value, value);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }


    IEnumerator StretchRoutine()
    {
        state = !state;

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        float endpoint;
        if (state)
            endpoint = 2f;
        else
            endpoint = 1f;

        float startpoint = transform.localScale.x;


        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, TweeningFunctions.EaseInOut(ratio));

            transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator RotateRoutine()
    {

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        float start = transform.localEulerAngles.y;


        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(start, (start + 90f)%360, TweeningFunctions.EaseInOut(ratio));

            Debug.Log(value);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, value, transform.localEulerAngles.z);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

}


	
