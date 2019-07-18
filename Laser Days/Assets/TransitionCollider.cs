using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Media;

public class TransitionCollider : MonoBehaviour
{
    public float growthSpeed = 1f;
    public float speed = .4f;
    public float endSize = 500000f;
    SphereCollider collider;
    bool direction;

    private void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Transition>() && !other.GetComponent<Transition>().shared)
        {

            other.GetComponent<Transition>().StopAllCoroutines();

            if (direction)
                other.GetComponent<Transition>().Flip(0f, speed);
            else
                other.GetComponent<Transition>().Flip(1f, speed);

        }

        if(other.GetComponent<UI_Transition>())
        {
            other.GetComponent<UI_Transition>().StopAllCoroutines();

            if (direction)
                other.GetComponent<UI_Transition>().Flip(0f, speed);
            else
                other.GetComponent<UI_Transition>().Flip(1f, speed);
        }

        //if (other.GetComponent<Shifter>())
        //other.GetComponent<Shifter>().Activate();

        if (other.GetComponent<FlipInteraction>())
            other.GetComponent<FlipInteraction>().Interact();
    }

    public void FlipTransitions(bool dir)
    {
        StopAllCoroutines();
        StartCoroutine(FlipTransitionRoutine());
        direction = dir;
    }

    private IEnumerator FlipTransitionRoutine()
    {
        collider.isTrigger = true;
        float elapsedTime = 0;
        float ratio = elapsedTime / growthSpeed;
        //int property = Shader.PropertyToID("_D7A8CF01");
        Vector3 startpoint;
        Vector3 endpoint;

        startpoint = new Vector3(0.000001f, 0.000001f, 0.000001f);
        endpoint = new Vector3(endSize, endSize, endSize);

        float start = .00001f;
        float end = 500f;
        collider.enabled = false;
        yield return new WaitForEndOfFrame();

        collider.enabled = true;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / growthSpeed;
            //float value = Mathf.Lerp(startpoint, endpoint, ratio);
            Vector3 value = Vector3.Lerp(startpoint, endpoint, (ratio));

            float radialValue = Mathf.Lerp(start, end, ratio);
            collider.transform.localScale = value;
            //collider.radius = radialValue;
            yield return null;
        }

        //collider.transform.localScale = new Vector3(500000f, 500000f, 500000f);;
    }


}