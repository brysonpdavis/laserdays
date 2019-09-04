using System.Collections.Generic;
using UnityEngine;

public class TakeMultipleActionsOnAction : TakeActionOnAction
{
    [SerializeField] private List<ResultActionType> resultActions;
    [SerializeField] private List<GameObject> victims;
    protected override void TakeAction()
    {
        for (int i = 0; i < resultActions.Count; i++)
        {
            switch (resultActions[i])
            {
                case ResultActionType.Deactivate:

                    victims[i].GetComponent<IDeactivatable>().Deactivate();
            
                    break;
        
                case ResultActionType.ActivateOnScreen:
            
                    victims[i].GetComponent<IActivatable>().Activate();

                    break;
        
                case ResultActionType.DeactivateOnScreen:
            
                    NarrationController.CancelNarration();

                    break;
        
                default:
            
                    break;
            }

        }
    }
}