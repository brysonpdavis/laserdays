using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotInteraction : MonoBehaviour {
    

    public virtual void RobotActivate()
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
