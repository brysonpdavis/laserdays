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
        Select,
        Hover
    }

    protected enum ResultActionType
    {
        Deactivate,
        ActivateOnScreen,
        DeactivateOnScreen
    }
    
    
    public void PickedUp()
    {
        if (activationAction == ActivationActionType.PickUp)
        {
            TakeAction();
        }
    }

    public void Selected()
    {
        if (activationAction == ActivationActionType.Select)
        {
            TakeAction();
        }
    }

    protected virtual void TakeAction()
    {
        switch (resultAction)
        {
            case ResultActionType.Deactivate:

                victim.GetComponent<ObjectWorldSpaceUI>().TurnOff();
                
                break;
            
            case ResultActionType.ActivateOnScreen:
                
                victim.GetComponent<TextNarration>().Activate();

                break;
            
            case ResultActionType.DeactivateOnScreen:
                
                NarrationController.CancelNarration();

                break;
            
            default:
                
                break;
        }
    }

    public void Hovered()
    {
        if (activationAction == ActivationActionType.Hover)
        {
            TakeAction();
        }
    }
}
