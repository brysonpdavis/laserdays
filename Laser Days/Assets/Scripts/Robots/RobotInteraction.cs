using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotInteraction : SelectableObject {

    public override void OnSelect()
    {
        TakeActionOnAction actionScript = GetComponent<TakeActionOnAction>();

        if (actionScript)
        {
            actionScript.Selected();
        }
    }

    public virtual void RobotDeactivate()
    {
        //GetComponentInChildren<RobotInteraction>().RobotActivate();
    }
}
