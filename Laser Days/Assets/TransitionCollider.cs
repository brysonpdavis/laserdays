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
        Transition transition = other.GetComponent<Transition>();

        if (transition && !transition.shared)
        {

            transition.StopAllCoroutines();

            if (direction)
                transition.Flip(0f, setCoroutineSpeed);
            else
                transition.Flip(1f, setCoroutineSpeed);

        }

        UI_Transition uI_Transition = other.GetComponent<UI_Transition>();
        if(uI_Transition)
        {
            uI_Transition.StopAllCoroutines();

            if (direction)
                uI_Transition.Flip(0f, setCoroutineSpeed);
            else
                uI_Transition.Flip(1f, setCoroutineSpeed);
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