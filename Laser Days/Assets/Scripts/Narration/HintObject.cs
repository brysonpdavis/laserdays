using UnityEngine;

public class HintObject : MonoBehaviour, IActivatable, IDeactivatable
{
    [SerializeField]
    private TextAsset text;
    [SerializeField]
    private Sprite icon;

    public void Activate()
    {
        NarrationController.TriggerHint(text, icon);
    }

    public void Deactivate()
    {
        NarrationController.ClearHint();
    }
}
