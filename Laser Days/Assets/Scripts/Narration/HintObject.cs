using UnityEngine;

public class HintObject : MonoBehaviour, IActivatable, IDeactivatable
{
    [SerializeField]
    private TextAsset text;
    [SerializeField]
    private Sprite keyboardIcon;
    [SerializeField]
    private Sprite controllerIcon;
    [SerializeField] 
    private bool activateOnce = true;

    private bool _activated;

    public void Start()
    {
        _activated = false;
    }

    public void Activate()
    {
        if (activateOnce)
        {
            if (!_activated)
            {

                NarrationController.TriggerHint(text, 
                    (ControlManager.GetControllerState() == ControlManager.ControllerState.JoystickPS4) ? controllerIcon : keyboardIcon);
            }
        }
        else
        {
            NarrationController.TriggerHint(text, keyboardIcon);
        }
        
        _activated = true;
    }

    public void Deactivate()
    {
        NarrationController.ClearHint();
    }
}
