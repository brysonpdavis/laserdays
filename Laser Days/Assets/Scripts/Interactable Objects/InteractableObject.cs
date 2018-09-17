using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class InteractableObject : MonoBehaviour
{
    protected ItemProperties itemProperties;
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

    // Use this for initialization
    void Start()
    {
        itemProperties = GetComponent<ItemProperties>();
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
        if (itemProperties.objectType == ItemProperties.ObjectType.Clickable) { multiplier = 1f; }
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

    public abstract void Pickup();

    public abstract void Drop();

    public abstract void DistantIconHover();

    public abstract void CloseIconHover();

    public abstract void InteractingIconHover();

}