using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCollider : MonoBehaviour
{
    public float mySpeed = 20;
    float speed = .4f;
    Collider collider;
    bool direction;

    private void Start()
    {
        collider = GetComponent<Collider>();
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
    }

    public void FlipTransitions(bool dir)
    {
        StopAllCoroutines();
        StartCoroutine(FlipTransitionRoutine());
        direction = dir;
    }

    private IEnumerator FlipTransitionRoutine()
    {

        float elapsedTime = 0;
        float ratio = elapsedTime / mySpeed;
        //int property = Shader.PropertyToID("_D7A8CF01");
        Vector3 startpoint;
        Vector3 endpoint;

        startpoint = new Vector3(0f, 0f, 0f);
        endpoint = new Vector3(100f, 100f, 100f);


        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / mySpeed;
            //float value = Mathf.Lerp(startpoint, endpoint, ratio);
            Vector3 value = Vector3.Lerp(startpoint, endpoint, ratio);

            collider.transform.localScale = value;

            yield return null;
        }
    }


}