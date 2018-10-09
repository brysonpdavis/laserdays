using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class InteractableObject : MonoBehaviour
{
    //protected ItemProperties itemProperties;
    public enum ObjectType { Clickable, Sokoban1x1, Sokoban2x2, Morph, Wall, WallSliderX, WallSliderZ, Null };
    [HideInInspector]public ObjectType objectType;
    [HideInInspector]public string itemName;


    protected Rigidbody rigidbody;
    protected MFPP.Player player;
    protected IconContainer iconContainer;
    protected RaycastManager raycastManager;
    protected Renderer renderer;
    protected Camera mainCamera;
    protected MFPP.Modules.PickUpModule pickUp;
    protected float currentPositionVelocity = 10f;
    protected AudioSource audioSource;
    protected ParticleSystem particleSystem;
    protected SelectionRenderChange selectionRenderChange;

    private float multiplier;
    public bool selected = false;
    public bool recentlySelected = false;

    //from item properties
    protected Renderer mRenderer;
    protected Material material;
    public Shader selectedShader;
    [HideInInspector]public bool inMotion = false;
    [HideInInspector] public bool isKey = false;
    [HideInInspector] public string key = null;


    [HideInInspector] public int value;

    [HideInInspector] public bool objectCharge = true;
    //public bool secondaryLock;
    [HideInInspector] public bool beenPickedUp = false;
    [HideInInspector] public bool boost = false;




    private void Awake()
    {
        SetType();

        //sets all of the shaders to correct flip state [toggle that affects how selection looks]
        mRenderer = GetComponent<Renderer>();
        material = mRenderer.material;
        material.SetInt("_Flippable", 0);
        material.SetFloat("_onHold", 0f);

    }

    void Start()
    {
        player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        iconContainer = Toolbox.Instance.GetIconContainer();
        raycastManager = Toolbox.Instance.GetRaycastManager();
        pickUp = Toolbox.Instance.GetPickUp();

        if (GetComponent<Rigidbody>())
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
        }
        mainCamera = Camera.main;
        renderer = GetComponent<Renderer>();
        if (GetComponent<AudioSource>()) { audioSource = GetComponent<AudioSource>(); }
        if (GetComponent<SelectionRenderChange>()) { selectionRenderChange = GetComponent<SelectionRenderChange>(); }
        AfterStart();



        //sets up the object to have a faster or slower glide to the player camera
        //if (itemProperties.objectType == ItemProperties.ObjectType.Clickable) 
        if (objectType == InteractableObject.ObjectType.Clickable)
        {
            multiplier = 1f;
            particleSystem = GetComponent<ParticleSystem>();
        }
        else { multiplier = .25f; }
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
        Vector3 floatingPosition = mainCamera.transform.position + mainCamera.transform.forward * pickUp.MaxPickupDistance;
        rigidbody.angularVelocity *= 0.5f;
        rigidbody.velocity = ((floatingPosition - rigidbody.transform.position) * (currentPositionVelocity*multiplier));
    }

    public void Select()
    {
        if (selectionRenderChange){
            selectionRenderChange.OnHold();
        }
            material.SetFloat("_onHold", 1f);
        material.SetFloat("_Elpapsed", 0f);
        selected = true;
    }

    public virtual void UnSelect()
    {
        if (selectionRenderChange){
            selectionRenderChange.OnDrop();
        }

        material.SetFloat("_onHold", 0f);
        //material.SetFloat("_Elpapsed", 0f);
        selected = false;

    }

    public virtual bool Flippable { get { return false; } }

    protected virtual bool HasBeenPickedUp{ get { return false; } }

    protected virtual void AfterStart() {} //for objects'/classes' additional initializations

    public abstract void Pickup();

    public abstract void Drop();

    public abstract void DistantIconHover();

    public abstract void CloseIconHover();

    public abstract void InteractingIconHover();

    public abstract void SetType();

    protected virtual void StopShimmerRoutine() {} //used in flippable


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
                iconContainer.SetBothBounce();
            else if (booster.affectsObjects && !booster.affectsPlayer)
                iconContainer.SetObjectBounce();
            else if (booster.affectsPlayer && !booster.affectsObjects)
                iconContainer.SetPlayerBounce();
        }
        else
            iconContainer.SetInteractHover();
        
    }

}