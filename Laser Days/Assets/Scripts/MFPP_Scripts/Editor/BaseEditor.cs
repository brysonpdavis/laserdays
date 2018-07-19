using UnityEditor;

namespace MFPP.Editor
{
    public abstract class BaseEditor : UnityEditor.Editor
    {
        private string errors = string.Empty;
        private string warnings = string.Empty;

        protected void AddError(string message)
        {
            errors += message + "\n\n";
        }

        protected void AddWarning(string message)
        {
            warnings += message + "\n\n";
        }

        protected void ShowErrorsAndWarnings()
        {
            if (errors.IsNullOrWhitespace() && warnings.IsNullOrWhitespace())
                return;

            EditorGUILayout.Space();

            if (!errors.IsNullOrWhitespace())
                EditorGUILayout.HelpBox(errors.TrimEnd('\n'), MessageType.Error);

            if (!warnings.IsNullOrWhitespace())
                EditorGUILayout.HelpBox(warnings.TrimEnd('\n'), MessageType.Warning);
        }

        protected void ResetErrorsAndWarnings()
        {
            errors = warnings = string.Empty;
        }
    }
}