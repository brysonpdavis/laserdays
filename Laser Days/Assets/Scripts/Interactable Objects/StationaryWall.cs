using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryWall : FlippableObject {


    public override void Pickup()
    {
        Debug.Log("Picking up");
        if (selected)
        {
            raycastManager.RemoveFromList(this.gameObject, false, false);
            raycastManager.selectedObjs.Remove(this.gameObject);
            selected = false;
            UnSelect();

        }
        else
        {
            raycastManager.AddToList(this.gameObject);
            raycastManager.selectedObjs.Add(this.gameObject);
            //selected = true;
            Select();
        }

        player.GetComponent<MFPP.Modules.PickUpModule>().KillPickup();

    }
    public override void Drop()
    {
        UnSelect();
    }

    public override void SetType()
    {
        objectType = ObjectType.Wall;
    }

    public override void DistantIconHover()
    {
        iconContainer.SetSelectHover();
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
