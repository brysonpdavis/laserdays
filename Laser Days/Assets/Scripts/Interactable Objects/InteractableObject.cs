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
    private float multiplier;
    public bool selected = false;

    //from item properties
    Renderer mRenderer;
    Material material;
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

        if (Flippable)
        {
            material.SetInt("_IsFlippable", 1);
        }

        else { material.SetInt("_IsFlippable", 0); }
    }

    void Start()
    {
        //itemProperties = GetComponent<ItemProperties>();
        if (GetComponent<Rigidbody>())
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
        }
        player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        iconContainer = Toolbox.Instance.GetIconContainer();
        raycastManager = Toolbox.Instance.GetPlayer().GetComponent<RaycastManager>();
        renderer = GetComponent<Renderer>();
        mainCamera = Camera.main;
        pickUp = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Modules.PickUpModule>();
        if (GetComponent<AudioSource>()) { audioSource = GetComponent<AudioSource>(); }



        //sets up the object to have a faster or slower glide to the player camera
        //if (itemProperties.objectType == ItemProperties.ObjectType.Clickable) 
        if (objectType == InteractableObject.ObjectType.Clickable) { multiplier = 1f; }
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
        material.shader = selectedShader;

    }

    public virtual void UnSelect()
    {
        if (this.gameObject.layer == 10)
        {
            material.shader = raycastManager.laserWorldShader;
        }

        else { material.shader = raycastManager.realWorldShader; }

    }

    public virtual bool Flippable { get { return false; } }

    protected virtual bool HasBeenPickedUp{ get { return false; } }

    public abstract void Pickup();

    public abstract void Drop();

    public abstract void DistantIconHover();

    public abstract void CloseIconHover();

    public abstract void InteractingIconHover();

    public abstract void SetType();

}