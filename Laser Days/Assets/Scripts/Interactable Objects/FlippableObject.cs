using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FlippableObject : InteractableObject
{

    [HideInInspector] public int timesFlipped = 0;
    [HideInInspector] public int maxFlips;
    public float secondaryFlipDuration = 1f;
    private IEnumerator flipTransition;


    public virtual void OnFlip()
    {
        timesFlipped += 1;

        if (timesFlipped == maxFlips + 1)
        {
            //this.gameObject.SetActive(false);
        }

        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem.Burst burst = new ParticleSystem.Burst(.025f, 50f);
            var main = particleSystem.main;
            main.startLifetime = .75f;
            particleSystem.emission.SetBurst(0, burst);
            particleSystem.Play();
        }

            float start = material.GetFloat("_TransitionStateB");
            if (player.gameObject.layer == 15)
            {
                Debug.Log("gromie!");
                float scaledDuration = secondaryFlipDuration * (1f - start);
                StopAllCoroutines();
                StartCoroutine(flipTransitionRoutine(start, 1, scaledDuration));
            }
            else
            {
                Debug.Log("homie!");
                float scaledDuration = secondaryFlipDuration * start;
                StopAllCoroutines();
                StartCoroutine(flipTransitionRoutine(start, 0, scaledDuration));
            }



        //StartCoroutine(flipTransition);


    }

    public virtual int TimesFlipped { get { return timesFlipped; } }


    IEnumerator flipTransitionRoutine(float startpoint, float endpoint, float duration)
    {

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, ratio);

            material.SetFloat("_TransitionStateB", value);
            RendererExtensions.UpdateGIMaterials(mRenderer);

            yield return null;
        }
    }
}

