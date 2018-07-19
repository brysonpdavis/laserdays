using UnityEditor;

namespace MFPP.Editor
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : BaseEditor
    {
        public override void OnInspectorGUI()
        {
            Player player = (Player)target;

            ResetErrorsAndWarnings();

            if (player.Main.Camera == null)
                AddError("No camera attached to the player!");

            if (player.Controls.MouseXAxis.IsNullOrWhitespace())
                AddError("Mouse X axis not assigned!");

            if (player.Controls.MouseYAxis.IsNullOrWhitespace())
                AddError("Mouse Y axis not assigned!");

            if (player.Controls.HorizontalAxis.IsNullOrWhitespace())
                AddError("Horizontal axis not assigned!");

            if (player.Controls.VerticalAxis.IsNullOrWhitespace())
                AddError("Vertical axis not assigned!");

            if (player.Controls.JumpButton.IsNullOrWhitespace() && player.Movement.Jump.AllowJump)
                AddWarning("Jump is allowed but no jump button has been assigned.");

            if (player.Controls.SprintButton.IsNullOrWhitespace() && player.Movement.Sprint.AllowSprint)
                AddWarning("Sprint is allowed but no sprint button has been assigned.");

            if (player.Controls.CrouchButton.IsNullOrWhitespace() && player.Movement.Crouch.AllowCrouch)
                AddWarning("Crouch is allowed but no crouch button has been assigned.");

            DrawDefaultInspector();

            ShowErrorsAndWarnings();
        }
    }
}
