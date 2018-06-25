using UnityEngine;
using UnityEditor;

namespace CodingJar.MultiScene
{
#if false
    [CustomPropertyDrawer(typeof(SceneData.CrossSceneReference))]
    public class CrossReferencePropertyDrawer : PropertyDrawer
    {
        // Summary:
        //     Override this method to specify how tall the GUI for this field is in pixels.
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUIUtility.singleLineHeight * 5.0f;
        }

        //
        // Summary:
        //     Override this method to make your own GUI for the property.
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Less area needed to display the prefix, more for the variables
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = EditorGUIUtility.labelWidth * 0.65f;

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Indent level reset
            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Find all of the data and grab it so we can start displaying (and calculating using it)
            var propFromSubScene = property.FindPropertyRelative ("fromSubScene");
            SubScene fromSubScene = propFromSubScene.objectReferenceValue as SubScene;

            var propFromObjectHash = property.FindPropertyRelative ("fromObjectHash");
            string fromObjectHash = propFromObjectHash.stringValue;

            var propFromPropertyPath = property.FindPropertyRelative ("fromPropertyPath");
            string fromPropertyPath = propFromPropertyPath.stringValue;

            var propToSubScene = property.FindPropertyRelative ("toSubScene");
            SubScene toSubScene = propToSubScene.objectReferenceValue as SubScene;

            var propToObjectHash = property.FindPropertyRelative ("toObjectHash");
            string toObjectHash = propToObjectHash.stringValue;

            // Resolve the actual objects
            Object fromObject = CrossSceneReferenceProcessor.ResolveObject( fromSubScene, fromObjectHash );
            Object toObject = CrossSceneReferenceProcessor.ResolveObject( toSubScene, toObjectHash );

            // If we're missing, maybe it's a deep reference?
            bool bWasDeepObjRef = false;
            if ( !toObject )
            {
                toObject = CrossSceneReferenceProcessor.ResolveGlobalObject( toObjectHash );
                bWasDeepObjRef = toObject;
            }

            bool bIsValidPath = fromObject && new SerializedObject(fromObject).FindProperty( fromPropertyPath ) != null;

            // Calculate rects
            float lineHeight = EditorGUIUtility.singleLineHeight;
            Rect fromSubSceneRect   = new Rect(position.x, position.y + lineHeight * 0, position.width, lineHeight);
            Rect fromObjectRect     = new Rect(position.x, position.y + lineHeight * 1, position.width, lineHeight);
            Rect fromPropertyRect   = new Rect(position.x, position.y + lineHeight * 2, position.width, lineHeight);
            Rect toSubSceneRect     = new Rect(position.x, position.y + lineHeight * 3, position.width, lineHeight);
            Rect toObjectRect       = new Rect(position.x, position.y + lineHeight * 4, position.width, lineHeight);

            Color oldColor = GUI.color;

            // Draw the From Scene
            EditorGUI.PropertyField( fromSubSceneRect,   propFromSubScene );

            // Draw from Object
            if ( fromObject )
            {
                GUI.color = Color.green;
                EditorGUI.ObjectField( fromObjectRect, "From Object Ref", fromObject, typeof(Object), true );
            }
            else
            {
                GUI.color = Color.red;
                EditorGUI.PropertyField( fromObjectRect, propFromObjectHash );
            }

            // Draw From Path
            GUI.color = bIsValidPath ? Color.green : Color.red;
            EditorGUI.PropertyField( fromPropertyRect, propFromPropertyPath );

            // Draw the To Data
            GUIContent content = new GUIContent( propToSubScene.displayName );
            if ( !toSubScene && toObject )
            {
                toSubScene = SubSceneEx.GetSubScene( toObject );
                if ( toSubScene )
                {
                    content = new GUIContent( propToSubScene.displayName, "This value was computed from a deep-search for the Object. It is probably a deep reference" );
                    GUI.color = Color.yellow;
                }
            }

            // To SubScene
            EditorGUI.ObjectField( toSubSceneRect, content, toSubScene, typeof(SubScene), true );

            if ( toObject )
            {
                GUI.color = Color.green;
                content = new GUIContent( "To Object Ref" );
                if ( bWasDeepObjRef )
                {
                    GUI.color = Color.yellow;
                    content.tooltip = "This value was computed from a deep-search for the Object. It is probably a deep reference";
                }

                EditorGUI.ObjectField( toObjectRect, "To Object Ref", toObject, typeof(Object), true );
            }
            else
            {
                GUI.color = Color.red;
                EditorGUI.PropertyField( toObjectRect, propToObjectHash );

                GUI.color = oldColor;
            }

            GUI.color = oldColor;
            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.indentLevel = oldIndentLevel;
            EditorGUI.EndProperty();
        }
    }
#endif
}
