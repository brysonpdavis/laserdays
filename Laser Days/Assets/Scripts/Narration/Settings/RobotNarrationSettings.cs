using UnityEngine;

[CreateAssetMenu(fileName = "New RobotNarrationSettings", menuName = "Narration Settings/Robot", order = 52)]
public class RobotNarrationSettings : NarrationSettings
{
    public override void OnActivate()
    {
        base.OnActivate();
        Toolbox.Instance.DisablePlayerMovement();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        Toolbox.Instance.EnablePlayerMovement();
    } 
}
