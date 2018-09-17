using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryWall : InteractableObject {

    public override void Pickup()
    {
        if (itemProperties.selected)
        {
            raycastManager.RemoveFromList(this.gameObject, false);
            raycastManager.selectedObjs.Remove(this.gameObject);
            itemProperties.selected = false;
            itemProperties.UnSelect();

        }
        else
        {
            raycastManager.AddToList(this.gameObject);
            raycastManager.selectedObjs.Add(this.gameObject);
            itemProperties.selected = true;
            itemProperties.Select();
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
}
