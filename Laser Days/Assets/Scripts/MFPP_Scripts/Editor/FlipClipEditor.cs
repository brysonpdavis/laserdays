using System.Linq;
using UnityEditor;

namespace MFPP.Editor
{
    [CustomEditor(typeof(FlipClipAsset))]
    public class FlipClipAssetEditor : BaseEditor
    {
        public override void OnInspectorGUI()
        {
            FlipClipAsset asset = (FlipClipAsset)target;

            ResetErrorsAndWarnings();

            EmptyNames(asset);

            NameConflict(asset);

            DrawDefaultInspector();

            ShowErrorsAndWarnings();
        }

        private void EmptyNames(FlipClipAsset asset)
        {
          
        }
        private void NameConflict(FlipClipAsset asset)
        {

        }
    }
}