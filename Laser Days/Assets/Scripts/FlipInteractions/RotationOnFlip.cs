using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOnFlip : FlipInteraction

{
	public Vector3 endRotation;
    Vector3 startRotation;
    public float duration = 2f;
    float ratio = 0f;
    public bool startRotationInLaser = true;
    public bool check;
    public TweeningFunctions.TweenType tween = TweeningFunctions.TweenType.EaseInOut;
    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        startRotation = transform.localEulerAngles;

        if ((Toolbox.Instance.PlayerInReal() && startRotationInLaser) || (Toolbox.Instance.PlayerInLaser() && (!startRotationInLaser)))
            transform.localEulerAngles = endRotation;
            
    }

    public override void Interact()
    {
        if (check)
            Debug.Log("interacting with rotation");
        StopAllCoroutines();
        //Debug.Log("clamped ratio: " + Mathf.Clamp(ratio, 0f, 1f));
        //Debug.Log("ratio " + ratio);

        if (ratio < .05f)
            ratio = 1f;

        StartCoroutine(RotateRoutine((ratio) * duration));

    }


	IEnumerator RotateRoutine(float actualDuration)
    {
        //Debug.Log("start rotation "+ startRotation + "end rotation " + endRotation + "actualduration " + actualDuration);
        float elapsedTime = 0f;
        ratio = 0f;
        Vector3 start = transform.localEulerAngles;
        Vector3 end;

        if (startRotationInLaser && Toolbox.Instance.PlayerInLaser() || (!startRotationInLaser && Toolbox.Instance.PlayerInReal()))
            end = startRotation;

        else
            end = endRotation;



        while (ratio < 1f)
        {
            ratio = elapsedTime / actualDuration;
            Vector3 value = Vector3.Lerp(start, end, TweeningFunctions.Tween(tween, ratio));

            transform.localEulerAngles = value;

            if (audio)
                audio.volume = TweeningFunctions.BackAndForth(ratio);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ratio = 0f;

    }
}
