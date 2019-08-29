using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//[RequireComponent(typeof(UniqueId))]

abstract public class HoldableObject : SelectableObject, IHoldable
{
    //protected ItemProperties itemProperties;
    public enum ObjectType { Clickable, SingleWorldClickable, Sokoban1x1, Sokoban2x2, Morph, Wall, WallSliderX, WallSliderZ, FloorBouncer, LinkedPair, Null };
    
    protected Rigidbody rigidbody;
    protected MFPP.Player player;
    protected Camera mainCamera;
    protected MFPP.Modules.PickUpModule pickUp;
    protected float currentPositionVelocity = 10f;
    protected ParticleTransitionBurst[] particleTransitionBursts;
    protected bool hasTransitionParticles = false;
    protected Vector3 latestPosition;
    protected AudioSource audio;

    [FormerlySerializedAs("pickupDistance")] 
    [SerializeField] 

    public ObjectType objectType;
    public bool inMotion;

    private float multiplier = 1f;
    public bool recentlySelected = false;
    [HideInInspector] public bool beenPickedUp = false;
    
    public virtual void Start()
    {
        base.Start();
        
        player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        pickUp = Toolbox.Instance.GetPickUp();
        audio = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        //if (rigidbody)
        //{
        //    rigidbody.isKinematic = true;
        //}

        inMotion = false;

        particleTransitionBursts = GetComponentsInChildren<ParticleTransitionBurst>();
        if(particleTransitionBursts.Length>0)
        {
            hasTransitionParticles = true;
        }

    }


    public void ResetWalk()
    {
        //resets player's walking speed movement, jump
        player.Movement.Speed = 3;
        player.Movement.Jump.Power = 4.2f;
        //player.Footstep.Asset = originalFootstep;
        player.GetComponent<MFPP.Modules.BobModule>().BobSpeed = 3f;
    }

    public void HoldPosition()
    {
        Debug.Log("holding position!");

        Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * _activateDistance; //pickUp.MaxPickupDistance;
        rigidbody.angularVelocity *= 0.5f;
        rigidbody.velocity = ((floatingPosition - rigidbody.transform.position) * (currentPositionVelocity*multiplier));
    }
    
    public virtual bool Flippable { get { return false; } }

    protected virtual bool HasBeenPickedUp{ get { return false; } }

    public virtual void DoPickup()
    {
        if (_action)
        {
            _action.PickedUp();
        }
        
        Pickup();
    }

    public abstract void Pickup();

    public abstract void Drop();

    public abstract void InteractingIconHover();

    public virtual void SetType()
    {
        objectType = ObjectType.Clickable;
    }

    protected virtual void StopShimmerRoutine() {} //used in flippable

    public virtual void LoadShader(bool real)
    {
        if (real)
            ShaderUtility.ShaderToReal(_material);

        else
            ShaderUtility.ShaderToLaser(_material);
    }

    protected virtual bool AmHeldObj()
    {
        if (pickUp.heldObject && pickUp.heldObject.Equals(this.gameObject))
        {
            return true;
        }
        else return false;
    }

    protected virtual void CheckBouncer()
    {
        //check to make sure which icon for the bouncer to use - only called using sokoban 2x2, and wall bouncer x, y
        if (GetComponentInChildren<Booster>())
        {
            Booster booster = GetComponentInChildren<Booster>();
            if (booster.affectsObjects && booster.affectsPlayer)
                _iconContainer.SetBothBounce();
            else if (booster.affectsObjects && !booster.affectsPlayer)
                _iconContainer.SetObjectBounce();
            else if (booster.affectsPlayer && !booster.affectsObjects)
                _iconContainer.SetPlayerBounce();
        }
        else
            _iconContainer.SetInteractHover();
        
    }

    protected virtual void CheckMovement()
    {
        if (selected && !latestPosition.Equals(transform.position))
        {
            //Debug.Log(rigidbody.velocity.magnitude * Toolbox.Instance.soundEffectsVolume);
            audio.volume = (rigidbody.velocity.magnitude * Toolbox.Instance.soundEffectsVolume);
            //audio.volume = (latestPosition - transform.position).magnitude * Toolbox.Instance.soundEffectsVolume * 50f; //volume * movement;
            audio.mute = false;
        }

        else 
        {
            audio.mute = true;

        }
    }
}