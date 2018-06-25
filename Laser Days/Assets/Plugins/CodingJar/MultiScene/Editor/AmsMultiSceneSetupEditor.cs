using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Linq;

using CodingJar;

namespace CodingJar.MultiScene.Editor
{
	/// <summary>
	/// Custom 
	/// </summary>
	[CustomEditor(typeof(AmsMultiSceneSetup), true)]
	class AmsMultiSceneSetupEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI ()
		{
			AmsMultiSceneSetup target = (AmsMultiSceneSetup)this.target;

			if ( !target.isMainScene )
			{
				DrawPropertiesExcluding( serializedObject, "m_Script", "_sceneSetup" );
				EditorGUILayout.HelpBox( "Since we are not configured as a Main Scene, we will NOT keep track of loaded scenes.\nThis behaviour is still required for cross-scene referencing to work.", MessageType.Info );
			}
			else
			{
				DrawPropertiesExcluding( serializedObject, "m_Script" );
			}

			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();

			if ( GUI.changed )
				EditorApplication.RepaintHierarchyWindow();
		}
	} // class


	/// <summary>
	/// Draw the scene name instead of "Element 0" when listing a Multi-Scene Entry.
	/// </summary>
	[CustomPropertyDrawer(typeof(AmsMultiSceneSetup.SceneEntry))]
	class AmsMultiSceneEntryDrawer : UnityEditor.PropertyDrawer
	{
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
		{
			return EditorGUI.GetPropertyHeight( property, null, true );
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var propScene = property.FindPropertyRelative( "scene" );
			if ( propScene != null )
			{
				var propName = propScene.FindPropertyRelative( "name" );
				if ( propName != null )
				{
					label.text = propName.stringValue;
				}
			}

			EditorGUI.PropertyField( position, property, label, true );
		}
	}
} // namespace

