using UnityEngine;
using UnityEditor;

namespace CodingJar.MultiScene.Editor
{
    [CustomPropertyDrawer(typeof(AmsSceneReference))]
    public class AmsSceneReferenceDrawer : PropertyDrawer
    {
        // Summary:
        //     Override this method to specify how tall the GUI for this field is in pixels.
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUIUtility.singleLineHeight;
        }

        // Summary:
        //     Override this method to make your own GUI for the property.
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Indent level reset
            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Find all of the data and grab it so we can start displaying (and calculating using it)
            var propAssetGUID = property.FindPropertyRelative ("editorAssetGUID");
            string assetGUID = propAssetGUID.stringValue;

            var propName = property.FindPropertyRelative ("name");
            string name = propName.stringValue;

            var propPath = property.FindPropertyRelative ("_path");
            string path= propPath.stringValue;

			string realPath = AssetDatabase.GUIDToAssetPath( assetGUID );
			Object sceneAsset = AssetDatabase.LoadMainAssetAtPath( realPath );

			// Draw the ObjectField and apply changes.
			EditorGUI.BeginChangeCheck();
			sceneAsset = EditorGUI.ObjectField( position, sceneAsset, typeof(SceneAsset), false );
			if ( EditorGUI.EndChangeCheck() )
			{
				path = AssetDatabase.GetAssetOrScenePath( sceneAsset );
				name = System.IO.Path.GetFileNameWithoutExtension( path );
				assetGUID = AssetDatabase.AssetPathToGUID( path );

				// Reassign the properties
				propAssetGUID.stringValue = assetGUID;
				propPath.stringValue = path;
				propName.stringValue = name;

				property.serializedObject.ApplyModifiedProperties();
			}

			propPath.Dispose();
			propName.Dispose();
			propAssetGUID.Dispose();

            EditorGUI.indentLevel = oldIndentLevel;
            EditorGUI.EndProperty();
        }
    }
}
