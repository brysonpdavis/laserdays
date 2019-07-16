using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TakeActionOnAction : MonoBehaviour
{
    
    
    [SerializeField]
    private ActivationActionType activationAction;

    [SerializeField]
    private ResultActionType resultAction;

    public GameObject victim;

    private enum ActivationActionType
    {
        PickUp,
        PutDown,
        Select
    }

    private enum ResultActionType
    {
        Deactivate
    }
    public void PickedUp()
    {
        if (activationAction == ActivationActionType.PickUp)
        {
            TakeAction();
        }
    }

    private void TakeAction()
    {
        switch (resultAction)
        {
            case ResultActionType.Deactivate:

                victim.GetComponent<ObjectWorldSpaceUI>().TurnOff();
                
                break;
            
            default:
                
                break;
        }
    }
}
