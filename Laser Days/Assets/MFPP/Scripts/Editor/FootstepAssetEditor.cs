using System.Linq;
using UnityEditor;

namespace MFPP.Editor
{
    [CustomEditor(typeof(FootstepAsset))]
    public class FootstepAssetEditor : BaseEditor
    {
        public override void OnInspectorGUI()
        {
            FootstepAsset asset = (FootstepAsset)target;

            ResetErrorsAndWarnings();

            EmptyNames(asset);

            if (asset.DefaultFootstepsName.IsNullOrWhitespace())
                AddError("No default footsteps name specified! This will lead in a runtime error if we fallback to the default footsteps.");
            else if (asset.Footsteps != null && asset.Footsteps.Count > 0 && asset.Footsteps.All(data => data.Name != asset.DefaultFootstepsName))
                AddError("Specified default footsteps name: \"" + asset.DefaultFootstepsName + "\", doesn't exist anywhere in the footstep data list.");

            if (asset.Footsteps == null || asset.Footsteps.Count <= 0)
                AddWarning("No footstep data! Create one in the \"Footsteps\" list.");

            NameConflict(asset);

            DrawDefaultInspector();

            ShowErrorsAndWarnings();
        }

        private void EmptyNames(FootstepAsset asset)
        {
            if (asset.Footsteps == null)
                return;

            int count = 0;

            foreach (FootstepAsset.Data data in asset.Footsteps)
            {
                if (data.Name.IsNullOrWhitespace())
                    count++;
            }

            if (count > 0)
                AddError("There are " + count + " footstep data item(s) with empty names!");
        }
        private void NameConflict(FootstepAsset asset)
        {
            if (asset.Footsteps == null)
                return;

            if (asset.Footsteps.Select(data => data.Name).Distinct().Count() < asset.Footsteps.Count)
                AddError("There are name conflicts in the footstep data list!");
        }
    }
}