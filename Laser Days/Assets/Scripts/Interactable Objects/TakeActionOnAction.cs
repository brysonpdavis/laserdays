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

    private enum ResultActionType
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

    private void TakeAction()
    {
        switch (resultAction)
        {
            case ResultActionType.Deactivate:

                victim.GetComponent<ObjectWorldSpaceUI>().TurnOff();
                
                break;
            
            case ResultActionType.ActivateOnScreen:

                break;
            
            case ResultActionType.DeactivateOnScreen:
                
                Toolbox.Instance.ClearNarration();

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
