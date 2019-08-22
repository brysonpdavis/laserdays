using UnityEngine;

[CreateAssetMenu(fileName = "New RobotNarrationSettings", menuName = "Narration Settings/Robot", order = 52)]
public class RobotNarrationSettings : NarrationSettings
{
    [SerializeField] private bool flipOnResume = true;
    
    public override void OnActivate()
    {
        base.OnActivate();
        Toolbox.Instance.DisablePlayerMovementAndFlip();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        Toolbox.Instance.EnablePlayerMovementAndFlip(flipOnResume);
    } 
}
