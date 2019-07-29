using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryWall : FlippableObject {


    public override void Pickup()
    {
        if (MaxFlipCheck(false))
        {

            Debug.Log("Picking up");
            if (selected)
            {
                selected = false;
                OffSelect();

            }
            else
            {
                //selected = true;
                OnSelect();
            }

            player.GetComponent<MFPP.Modules.PickUpModule>().KillPickup();
        }
    }
    public override void Drop()
    {
        OffSelect();
    }

    public override void DistantIconHover()
    {
        _iconContainer.SetSelectHover();
    }

    public override void CloseIconHover()
    {
        _iconContainer.SetOpenHand();
    }

    public override void InteractingIconHover()
    {
        _iconContainer.SetDrag();
    }

    public override bool Flippable { get { return true; } }
}
