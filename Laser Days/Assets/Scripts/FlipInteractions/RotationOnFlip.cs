using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOnFlip : FlipInteraction

{
	public Vector3 endRotation;
    Vector3 startRotation;
    public float duration = 2f;
    float ratio = 0;
    public bool startRotationInLaser = true;

    private void Start()
    {
        startRotation = transform.localEulerAngles;

        if ((Toolbox.Instance.PlayerInReal() && startRotationInLaser) || (Toolbox.Instance.PlayerInLaser() && (!startRotationInLaser)))
            transform.localEulerAngles = endRotation;
            
    }

    public override void Interact()
    {


        StopAllCoroutines();
        StartCoroutine(RotateRoutine((1f-ratio)*duration));

    }


	IEnumerator RotateRoutine(float actualDuration)
    {
        //Debug.Log("start rotation "+ startRotation + "end rotation " + endRotation);
        float elapsedTime = 0;
        ratio = 0;
        Vector3 start = transform.localEulerAngles;
        Vector3 end;

        if (startRotationInLaser && Toolbox.Instance.PlayerInLaser() || (!startRotationInLaser && Toolbox.Instance.PlayerInReal()))
            end = startRotation;

        else
            end = endRotation;



        while (ratio < 1f)
        {
            ratio = elapsedTime / actualDuration;
            Vector3 value = Vector3.Lerp(start, end, TweeningFunctions.EaseInOut(ratio));

            transform.localEulerAngles = value;

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }



    }
}
