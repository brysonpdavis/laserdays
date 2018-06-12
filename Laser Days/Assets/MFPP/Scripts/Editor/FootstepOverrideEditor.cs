using UnityEditor;

namespace MFPP.Editor
{
    [CustomEditor(typeof(FootstepOverride))]
    public class FootstepOverrideEditor : BaseEditor
    {
        public override void OnInspectorGUI()
        {
            FootstepOverride fo = (FootstepOverride)target;

            ResetErrorsAndWarnings();

            if (fo.FootstepOverrideName.IsNullOrWhitespace())
                AddWarning("Override name is null or empty.");

            DrawDefaultInspector();

            ShowErrorsAndWarnings();
        }
    }
}