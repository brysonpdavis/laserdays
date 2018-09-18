using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryWall : InteractableObject {

    public override void Pickup()
    {
        if (selected)
        {
            raycastManager.RemoveFromList(this.gameObject, false);
            raycastManager.selectedObjs.Remove(this.gameObject);
            selected = false;
            UnSelect();

        }
        else
        {
            raycastManager.AddToList(this.gameObject);
            raycastManager.selectedObjs.Add(this.gameObject);
            selected = true;
            Select();
        }

        player.GetComponent<MFPP.Modules.PickUpModule>().KillPickup();

    }
    public override void Drop()
    {

    }

    public override void DistantIconHover()
    {
        iconContainer.SetInteractHover();
    }

    public override void CloseIconHover()
    {
        iconContainer.SetOpenHand();
    }

    public override void InteractingIconHover()
    {
        iconContainer.SetDrag();
    }

    public override bool Flippable { get { return true; } }

}
