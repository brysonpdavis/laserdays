using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class FlippableObject : HoldableObject, IFlippable
{
    public int timesFlipped = 0;
    public bool canFlip = true;
    public int maxFlips;
    public bool destroyOnLastFlip;
    public float secondaryFlipDuration = 1f;
    private IEnumerator flipTransition;
    private float scaledDuration;
    public bool slowPickup = false;
    public Color shimmerColor;
    public Transform sceneContainer;
    public Transform realTransform;
    public Transform laserTransform;
    public float transitionStateB;


    public override void Start()
    {
        base.Start();
        
        //happens on Interactable Object's start, makes sure all objs start on correct world color

        if (this.gameObject.layer == 10) { SetMaterialFloatProp("_TransitionStateB", 1); }

        else { SetMaterialFloatProp("_TransitionStateB", 0); }

        if (_selectionRenderChange) { _selectionRenderChange.SetFlippable(); }
        SetMaterialFloatProp("_Flippable", 1f);
        SetMaterialFloatProp("_Shimmer", 1f);
        SetMaterialFloatProp("_onHold", 0f);
        SetMaterialFloatProp("_EdgeThickness", Toolbox.Instance.EdgeGlowThickness());
        shimmerColor = _material.GetColor("_ShimmerColor");

        if (GetComponentInChildren<Core>())
        {
            GetComponentInChildren<Core>().SetColor(shimmerColor);
        }

        RendererExtensions.UpdateGIMaterials(_renderer);

        realTransform = sceneContainer.Find("Real");
        laserTransform = sceneContainer.Find("Laser");

/*
        if (!slowPickup && !(objectType==ObjectType.Wall || objectType == ObjectType.LinkedPair))
            rigidbody.isKinematic = false;
*/

        //if(GetComponent<Rigidbody>()){
        //    var rb = GetComponent<Rigidbody>();
        //    rb.constraints = RigidbodyConstraints.None;
        //   rb.isKinematic = false;
        //    rb.useGravity = true;

        //}


    }

    public virtual void OnFlip()
    {
        Debug.Log("in onflip");
        if (MaxFlipCheck(true))
        {
            Debug.Log("max flip check true");

            ParticleEffect();
            ColorTransition();
            LayerSwitch();
            FlipCore(true);
        }
    }

    public virtual void ForcedFlip()
    {
        if (Toolbox.Instance.EqualToHeld(gameObject))
            pickUp.PutDown();
        
        if (MaxFlipCheck(true))
        {
            ForcedTransitionEffect();
            //SelfLayerSwitch();
            //FlipCore(false);
        }
    }

    public void Update()
    {
        if (selected || recentlySelected)
        {
            float g = _material.GetFloat("_Elapsed");
            SetMaterialFloatProp("_Elapsed", g + Time.deltaTime);

        }

    }

    protected bool AbleToFlip { get { return MaxFlipCheck(false); }} 

    public virtual int TimesFlipped { get { return timesFlipped; } }

    protected void LayerSwitch()
    {
        if (!AmHeldObj())
        {
            if (Toolbox.Instance.PlayerInReal())
            {
                ShaderUtility.ShaderToReal(_material);
                GetComponent<Transition>().SetStart(0f); //set it fully on for laser world
            }
            else
            {
                ShaderUtility.ShaderToLaser(_material);
                GetComponent<Transition>().SetStart(1f); //set it fully on for laser world
            }

        }

/*
        HoldableObject.ObjectType type = objectType;
        if (Toolbox.Instance.PlayerInLaser())
        {
            SetObjectToLaser(); //set object to laser layer
            if (type == HoldableObject.ObjectType.Morph)
            {
                GetComponent<MorphController>().OnFlip(true);
            }

            if (!AmHeldObj())
            {
                //set the object to approperiate shader unless currently held
                //if new shader - do new thing, else do old

                ShaderUtility.ShaderToLaser(material);

                GetComponent<Transition>().SetStart(1f); //set it fully on for laser world
                //GetComponent<Transition>().SetStart(1f - (renderer.material.GetFloat("_TransitionState")));
            }

        }
        else if (Toolbox.Instance.PlayerInReal())
        {
            SetObjectToReal(); //set object to real layer
            if (type == HoldableObject.ObjectType.Morph)
            {
                GetComponent<MorphController>().OnFlip(false);
            }

            if (!AmHeldObj())
            {
                ShaderUtility.ShaderToReal(material);
                GetComponent<Transition>().SetStart(0f); //set it fully on for real world]
                //GetComponent<Transition>().SetStart((renderer.material.GetFloat("_TransitionState")));
            }
        }

        if (type == HoldableObject.ObjectType.LinkedPair)
        {
            //do the inverselayer switch on partner!
            GetComponent<LinkedPair>().SwitchPartner(Toolbox.Instance.PlayerInLaser());
        }
*/
    }

/*
    protected void SelfLayerSwitch()
    {
        bool goingToReal;

        if (gameObject.layer.Equals(10))

        {
            SetObjectToReal();
            goingToReal = true;  
        }

        else
        {
            SetObjectToLaser();

            goingToReal = false;
        }

        HoldableObject.ObjectType type = objectType;

        if (type == HoldableObject.ObjectType.Morph)
            {
            
            GetComponent<MorphController>().OnFlip(goingToReal);
            }
    }
*/

    protected void ForcedTransitionEffect()
    {
        Transition transition = GetComponent<Transition>();
        Core core = GetComponentInChildren<Core>();
            

        float direction = 1f;
        if (gameObject.layer == 11)
        {
            
            if (Toolbox.Instance.PlayerInLaser())
            {
                Debug.Log("1");
                transition.SetStart(_material.GetFloat("_TransitionState"));
                direction = 1f;
                if (core)
                {
                    ShaderUtility.ShaderToLaser(core.gameObject.GetComponent<Renderer>().material);
                    core.gameObject.GetComponent<Transition>().StopAllCoroutines();
                    core.gameObject.GetComponent<Transition>().SetStart(0f);
                    core.gameObject.GetComponent<Transition>().Flip(1f, Toolbox.Instance.globalFlipSpeed);
                }
            }

            else
            {
                Debug.Log("2");

                transition.SetStart(1f - _material.GetFloat("_TransitionState"));
                direction = 0f;

                if (core)
                {
                    ShaderUtility.ShaderToLaser(core.gameObject.GetComponent<Renderer>().material);
                    core.gameObject.GetComponent<Transition>().StopAllCoroutines();
                    core.gameObject.GetComponent<Transition>().SetStart(1f);
                    core.gameObject.GetComponent<Transition>().Flip(0f, Toolbox.Instance.globalFlipSpeed);
                }
            }

            Debug.Log("going to laser");
            ShaderUtility.ShaderToLaser(_material);
                
        }
        else
        {

            if (Toolbox.Instance.PlayerInLaser())
            {
                Debug.Log("3");

                transition.SetStart(_material.GetFloat("_TransitionState"));
                direction = 1f;


                if (core)
                {
                    ShaderUtility.ShaderToReal(core.gameObject.GetComponent<Renderer>().material);
                    core.gameObject.GetComponent<Transition>().StopAllCoroutines();
                    core.gameObject.GetComponent<Transition>().SetStart(0f);
                    core.gameObject.GetComponent<Transition>().Flip(1f, Toolbox.Instance.globalFlipSpeed);
                }
            }

            else
            {
                Debug.Log("4");

                transition.SetStart(1f - _material.GetFloat("_TransitionState"));
                direction = 0f;
                if (core)
                {
                    ShaderUtility.ShaderToReal(core.gameObject.GetComponent<Renderer>().material);
                    core.gameObject.GetComponent<Transition>().StopAllCoroutines();
                    core.gameObject.GetComponent<Transition>().SetStart(1f);
                    core.gameObject.GetComponent<Transition>().Flip(0f, Toolbox.Instance.globalFlipSpeed);
                }
            }

            Debug.Log("going to real");
            ShaderUtility.ShaderToReal(_material);

                
        }
            
        transition.Flip(direction, Toolbox.Instance.globalFlipSpeed);

    }

    void SetObjectToLaser()
    {
        this.gameObject.layer = 10;
        this.transform.parent = laserTransform;     //Toolbox.Instance.GetLaserWorldParent();
    }
    void SetObjectToReal()
    {
        this.gameObject.layer = 11;
        this.transform.parent = realTransform;  //Toolbox.Instance.GetRealWorldParent();
    }

    public bool MaxFlipCheck(bool CurrentlyFlipping)
    {
        if (canFlip)
        {

            if (CurrentlyFlipping)
                timesFlipped += 1;

            if (maxFlips == 0)
                return true;

            else if ((maxFlips == (timesFlipped - 1)) && CurrentlyFlipping)
            {

                if (destroyOnLastFlip)
                {
                    TurnOffSelf();
                    return false;
                }

                else 
                {
                    //do some other animation here for it being unable to flip!
                }
                Debug.Log("LAST FLIP! Can do something here :)");
                return true;
            }

            else if ((maxFlips != 0) && timesFlipped > (maxFlips))
            {
                return false;
            }

            else
                return true;
        }
        else
            return false;
    }

    public override void LoadShader(bool real)
    {
        base.LoadShader(real);
        GetComponentInChildren<Core>().Flip(!real);

    }

    void ParticleEffect()
    {
        if (hasTransitionParticles)
        {
            foreach (ParticleTransitionBurst burst in particleTransitionBursts)
            {
                burst.TransitionBurst();
            }
        }
    }

    void ColorTransition()
    {
        float start = _material.GetFloat("_TransitionStateB");
        //Debug.Log("start" + start);
        if (player.gameObject.layer == 15)
        {
            //Debug.Log("gromie!");
            scaledDuration = secondaryFlipDuration * (1f - start);
            StopAllCoroutines();
            StartCoroutine(flipTransitionRoutine(start, 1, scaledDuration, "_TransitionStateB"));
            StartCoroutine(ShimmerRoutine(scaledDuration));

        }
        else
        {
            //Debug.Log("homie!");
            scaledDuration = secondaryFlipDuration * start;
            StopAllCoroutines();
            StartCoroutine(flipTransitionRoutine(start, 0, scaledDuration, "_TransitionStateB"));
            StartCoroutine(ShimmerRoutine(scaledDuration));

        }
    }

    public void AddToTransitionList(TransitionController controller)
    {
        if (!controller.transitions.Contains(GetComponent<Transition>()))
            controller.transitions.Add(GetComponent<Transition>());

        if (GetComponentInChildren<Core>())
        {
            Transition core = GetComponentInChildren<Core>().gameObject.GetComponent<Transition>();
            if (!controller.transitions.Contains(core))
                controller.transitions.Add(core);
        }
    }

    public void RemoveFromTransitionList(TransitionController controller)
    {
        if (controller.transitions.Contains(GetComponent<Transition>()))
            controller.transitions.Remove(GetComponent<Transition>());

        if (GetComponentInChildren<Core>())
        {
            Transition core = GetComponentInChildren<Core>().gameObject.GetComponent<Transition>();
            if (controller.transitions.Contains(core))
                controller.transitions.Remove(core);
        }
    }


    IEnumerator flipTransitionRoutine(float startpoint, float endpoint, float duration, string floatName)
    {
        float elapsedTime = 0;
        this.recentlySelected = true;

        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {

            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(startpoint, endpoint, ratio);

            SetMaterialFloatProp(floatName, value);
            transitionStateB = value;


            //RendererExtensions.UpdateGIMaterials(mRenderer);

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
                SetMaterialFloatProp("_onHold", 1f);
                SetMaterialFloatProp("_Shimmer", 1f);
                RendererExtensions.UpdateGIMaterials(_renderer);
                recentlySelected = false;              
                yield break;
            }
            elapsedTime += Time.fixedDeltaTime;
            ratio = elapsedTime / duration;

            //shimmer stuff
            float start = 1f;
            float end = 0f;

            if (!pickUp.heldObject || !this.gameObject.Equals(pickUp.heldObject))
            {

                float shimmerValue = Mathf.Lerp(start, end, ratio);
                SetMaterialFloatProp("_onHold", shimmerValue);
                SetMaterialFloatProp("_Shimmer", shimmerValue);
                RendererExtensions.UpdateGIMaterials(_renderer);

            }
            yield return new WaitForFixedUpdate();
        }

        this.recentlySelected = false;
        if (!pickUp.heldObject || !this.gameObject.Equals(pickUp.heldObject))
        {
            SetMaterialFloatProp("_onHold", 0f);
            SetMaterialFloatProp("_Shimmer", 1f);
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

    public void TurnOffSelf()
    {
        if (Toolbox.Instance.EqualToHeld(gameObject))
            pickUp.PutDown();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        //float end;
        //if (gameObject.layer == 10)
        //    end = 1f;
        //else
            //end = 0f;

        //StopAllCoroutines();
        //Debug.Log(material.GetFloat("_TransitionState") + "end " + end);
        //StartCoroutine(flipTransitionRoutine(material.GetFloat("_TransitionState"), end, 1f, "_TransitionState"));

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
    }
}

