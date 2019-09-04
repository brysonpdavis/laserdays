using UnityEngine;

public class HintObject : MonoBehaviour, IActivatable, IDeactivatable
{
    [SerializeField]
    private TextAsset text;
    [SerializeField]
    private Sprite icon;
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
                NarrationController.TriggerHint(text, icon);
        }
        else
        {
            NarrationController.TriggerHint(text, icon);
        }
        
        _activated = true;
    }

    public void Deactivate()
    {
        NarrationController.ClearHint();
    }
}
