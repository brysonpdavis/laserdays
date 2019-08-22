using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffNarrationOnDoorTrigger : MonoBehaviour {

    public DoorTrigger trigger;
    bool activated = false;
    TakeMultipleActionsOnAction actionsOnAction;


	// Use this for initialization
	void Start () 
    {
        actionsOnAction = GetComponent<TakeMultipleActionsOnAction>();
	}
	
	// Update is called once per frame
	void Update () {
		
        if (!activated && trigger.active)
        {
            activated = true;
            NarrationController.CancelNarration();
            Destroy(actionsOnAction);

        }
	}
}
