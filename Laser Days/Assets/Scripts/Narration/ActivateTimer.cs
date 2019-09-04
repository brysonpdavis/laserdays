using System.Collections;
using UnityEngine;

public class ActivateTimer : MonoBehaviour, IActivatable
{

    [SerializeField] private Action actionType;
    [SerializeField] private float time;

    private enum Action
    {
        ClearHint
    }
    public void Activate()
    {
        StartCoroutine(WaitThenTakeAction());
    }

    private IEnumerator WaitThenTakeAction()
    {
        yield return new WaitForSeconds(time);

        switch (actionType)
        {
            case Action.ClearHint:
                NarrationController.ClearHint();
                break;
            
        }
    }
}
