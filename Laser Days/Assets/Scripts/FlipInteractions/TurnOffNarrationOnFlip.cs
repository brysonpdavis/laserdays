using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffNarrationOnFlip : FlipInteraction {

    FlippableObject flippable;
    bool activated = false;
    TakeMultipleActionsOnAction actionsOnAction;

	// Use this for initialization
	void Start () {
        flippable = GetComponent<FlippableObject>();
        actionsOnAction = GetComponent<TakeMultipleActionsOnAction>();
	}
    
    public override void Interact()
    {
        if (!activated && flippable.timesFlipped > 0)
        {
            activated = true;
            NarrationController.CancelNarration();
            //textNarration.SetActive(false);
            Destroy(actionsOnAction);
        }

    }
}
