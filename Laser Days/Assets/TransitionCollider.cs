using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Media;

public class TransitionCollider : MonoBehaviour
{
    private float growthSpeed = 1f;
    public float initialGrowthSpeed = 1f;
    public float growthAcceleration = 8f;
    public float setCoroutineSpeed = .4f;
    public float maxSize = 250;



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
                other.GetComponent<Transition>().Flip(0f, setCoroutineSpeed);
            else
                other.GetComponent<Transition>().Flip(1f, setCoroutineSpeed);

        }

        if(other.GetComponent<UI_Transition>())
        {
            other.GetComponent<UI_Transition>().StopAllCoroutines();

            if (direction)
                other.GetComponent<UI_Transition>().Flip(0f, setCoroutineSpeed);
            else
                other.GetComponent<UI_Transition>().Flip(1f, setCoroutineSpeed);
        }

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
        // Create scale value
        // init it to zero 
        // and set collider to zero scale 
        float scale = 0f;
        collider.transform.localScale = new Vector3(scale, scale, scale);

        // Set init growth speed
        growthSpeed = initialGrowthSpeed;

        collider.isTrigger = true;

        collider.enabled = false;

        yield return new WaitForEndOfFrame();

        collider.enabled = true;

        while (scale < maxSize)
        {
            scale += growthSpeed;

            growthSpeed *= growthAcceleration;

            collider.transform.localScale = new Vector3(scale, scale,scale);

            yield return new WaitForFixedUpdate();
        }

        //collider.transform.localScale = new Vector3(500000f, 500000f, 500000f);;
    }


}