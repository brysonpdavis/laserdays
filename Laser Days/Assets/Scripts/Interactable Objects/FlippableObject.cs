using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FlippableObject : InteractableObject
{

    [HideInInspector] public int timesFlipped = 0;
    [HideInInspector] public int maxFlips;
    public float secondaryFlipDuration = 1f;
    private IEnumerator flipTransition;
    private float scaledDuration;
    public Color shimmerColor;


    protected override void AfterStart()
    {
        //happens on Interactable Object's start, makes sure all objs start on correct world color

        if (this.gameObject.layer == 10) { material.SetFloat("_TransitionStateB", 1); }

        else { material.SetFloat("_TransitionStateB", 0); }

        if (selectionRenderChange) { selectionRenderChange.SetFlippable(); }
        material.SetInt("_Flippable", 1);
        material.SetFloat("_Shimmer", 1f);
        material.SetFloat("_onHold", 0f);
        material.SetFloat("_EdgeThickness", Toolbox.Instance.EdgeGlowThickness());
        shimmerColor = material.GetColor("_ShimmerColor");

        if (GetComponent<ParticleSystem>())
        {
            Material particleMat = GetComponent<ParticleSystemRenderer>().material;
            //shimmerColor
            particleMat.SetColor("_ParticleColor", shimmerColor);
        }

        if (GetComponentInChildren<Core>())
        {
            GetComponentInChildren<Core>().SetColor(shimmerColor);
        }

        RendererExtensions.UpdateGIMaterials(mRenderer);
    }

    public virtual void OnFlip()
    {
        MaxFlipCheck();
        ParticleEffect();
        ColorTransition();
        LayerSwitch();
        FlipCore(true);
    }

    public void Update()
    {
        if (selected || recentlySelected)
        {
            float g = material.GetFloat("_Elapsed");
            material.SetFloat("_Elapsed", g + Time.deltaTime);

        }

    }

    public virtual int TimesFlipped { get { return timesFlipped; } }

    protected void LayerSwitch()
    {
        InteractableObject.ObjectType type = objectType;

        if (Toolbox.Instance.PlayerInLaser())
        {
            SetObjectToLaser(); //set object to laser layer
            if (type == InteractableObject.ObjectType.Morph)
            {
                GetComponent<MorphController>().OnFlip(true);
            }

            if (!AmHeldObj())
            {
                //set the object to approperiate shader unless currently held
                material.shader = raycastManager.laserWorldShader;
                GetComponent<Transition>().SetStart(1f); //set it fully on for laser world
            }

        }
        else if (Toolbox.Instance.PlayerInReal())
        {
            SetObjectToReal(); //set object to real layer
            if (type == InteractableObject.ObjectType.Morph)
            {
                GetComponent<MorphController>().OnFlip(false);
            }

            if (!AmHeldObj())
            {
                material.shader = raycastManager.realWorldShader;
                GetComponent<Transition>().SetStart(0f); //set it fully on for real world
            }
        }
    }

    void SetObjectToLaser()
    {
        this.gameObject.layer = 10;
        this.transform.parent = Toolbox.Instance.GetLaserWorldParent();
    }
    void SetObjectToReal()
    {
        this.gameObject.layer = 11;
        this.transform.parent = Toolbox.Instance.GetRealWorldParent();
    }

    void MaxFlipCheck()
    {
        timesFlipped += 1;

        if (timesFlipped == maxFlips + 1)
        {
            //this.gameObject.SetActive(false);
        }
    }

    void ParticleEffect()
    {
        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem.Burst burst = new ParticleSystem.Burst(.025f, 50f);
            var main = particleSystem.main;
            main.startLifetime = .75f;
            particleSystem.emission.SetBurst(0, burst);
            particleSystem.Play();
        }
    }

    void ColorTransition()
    {
        float start = material.GetFloat("_TransitionStateB");
        //Debug.Log("start" + start);
        if (player.gameObject.layer == 15)
        {
            //Debug.Log("gromie!");
            scaledDuration = secondaryFlipDuration * (1f - start);
            StopAllCoroutines();
            StartCoroutine(flipTransitionRoutine(start, 1, scaledDuration));
            StartCoroutine(ShimmerRoutine(scaledDuration));

        }
        else
        {
            //Debug.Log("homie!");
            float scaledDuration = secondaryFlipDuration * start;
            StopAllCoroutines();
            StartCoroutine(flipTransitionRoutine(start, 0, scaledDuration));
            StartCoroutine(ShimmerRoutine(scaledDuration));

        }
    }


    IEnumerator flipTransitionRoutine(float startpoint, float endpoint, float duration)
    {

        float elapsedTime = 0;
        this.recentlySelected = true;

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


    IEnumerator ShimmerRoutine(float duration)
    {
        if (!Toolbox.Instance.EqualToHeld(this.gameObject)) {selected = false;}
        float elapsedTime = 0;
        this.recentlySelected = true;
        float ratio = elapsedTime / duration;


        while (ratio < 1f)
        {
            if (selected)
            {
                material.SetFloat("_onHold", 1f);
                material.SetFloat("_Shimmer", 1f);
                RendererExtensions.UpdateGIMaterials(mRenderer);
                recentlySelected = false;              
                yield break;
            }
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;

            //shimmer stuff
            float start = 1f;
            float end = 0f;

            if (!pickUp.heldObject || !this.gameObject.Equals(pickUp.heldObject))
            {

                float shimmerValue = Mathf.Lerp(start, end, ratio);
                material.SetFloat("_onHold", shimmerValue);
                material.SetFloat("_Shimmer", shimmerValue);
                RendererExtensions.UpdateGIMaterials(mRenderer);

            }
            yield return null;
        }

        this.recentlySelected = false;
        if (!pickUp.heldObject || !this.gameObject.Equals(pickUp.heldObject))
        {
            material.SetFloat("_onHold", 0f);
            material.SetFloat("_Shimmer", 1f);
        }
    }

    public virtual void FlipCore(bool onFlip)
    {
        //if (!onFlip) { Debug.Log("!onflip"); }
        //if (!AmHeldObj()) { Debug.Log("!amheldobj"); }
        //Debug.Log("called");

        if (GetComponentInChildren<Core>() && (!AmHeldObj()|| !onFlip))
            {
            //Debug.Log("going!");
                if (Toolbox.Instance.PlayerInLaser())
                {
                    GetComponentInChildren<Core>().Flip(true);
                }
                else if (Toolbox.Instance.PlayerInReal())
                {
                    GetComponentInChildren<Core>().Flip(false);
                }
            }
        }
}

