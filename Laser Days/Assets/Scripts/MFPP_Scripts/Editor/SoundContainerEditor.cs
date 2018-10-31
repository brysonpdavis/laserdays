using System.Linq;
using UnityEditor;

namespace MFPP.Editor
{
    [CustomEditor(typeof(SoundContainerAsset))]
    public class SoundContainerEditor : BaseEditor
    {
        public override void OnInspectorGUI()
        {
            SoundContainerAsset asset = (SoundContainerAsset)target;

            ResetErrorsAndWarnings();

            EmptyNames(asset);

            NameConflict(asset);

            DrawDefaultInspector();

            ShowErrorsAndWarnings();
        }

        private void EmptyNames(SoundContainerAsset asset)
        {
          
        }
        private void NameConflict(SoundContainerAsset asset)
        {

        }
    }
}