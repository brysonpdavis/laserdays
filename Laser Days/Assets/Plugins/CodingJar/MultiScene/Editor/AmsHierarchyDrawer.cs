using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;

using System.Collections.Generic;
using System.Linq;

namespace CodingJar.MultiScene.Editor
{
	static class AmsHierarchyDrawer
	{
		private static GUIStyle	_justifyRightLabel = null;
		private static GUIStyle _justifyRightPopup = null;

		[InitializeOnLoadMethod]
		static void HookUpDrawer()
		{
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
		}

		private static void OnHierarchyWindowItemOnGUI( int instanceID, Rect selectionRect )
		{
			if ( Application.isPlaying )
				return;

			// We can't early out because now we have widgets that respond to events.
			// We could potentially early-out on EventType.Used (which is used during scrolling).
			//if ( Event.current.type != EventType.Repaint )
			//	return;

			if ( _justifyRightLabel == null )
			{
				_justifyRightLabel = new GUIStyle( GUI.skin.label );
				_justifyRightLabel.alignment = TextAnchor.UpperRight;
				_justifyRightLabel.richText = true;
			}

			if ( _justifyRightPopup == null )
			{
				_justifyRightPopup = new GUIStyle( GUI.skin.FindStyle("Popup") );
				_justifyRightPopup.stretchWidth = false;
				_justifyRightPopup.alignment = TextAnchor.UpperRight;
				_justifyRightPopup.richText = true;
			}

			// Which object are we looking at?
			Object obj = EditorUtility.InstanceIDToObject(instanceID);
			bool bIsSceneHeader = (instanceID != 0 && !obj);
			if ( !bIsSceneHeader )
				return;

			// Make sure we have a scene
			var scene = GetSceneFromHandleID( instanceID );
			if ( !scene.IsValid() )
				return;

			selectionRect.xMax -= 32.0f;

			// Now figure out what these scene settings are
			var activeScene = EditorSceneManager.GetActiveScene();
			var sceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( EditorSceneManager.GetActiveScene(), false );
			if ( !sceneSetup )
			{
				GUI.Label( selectionRect, ColorText("AMS Not Found in " + activeScene.name, Color.red), _justifyRightLabel );
				return;
			}

			// If we're the active scene...
			if ( activeScene == scene )
			{
				GUI.Label( selectionRect, ColorText("<b>Active</b>", Color.green), _justifyRightLabel );
				return;
			}

			var entries = sceneSetup.GetSceneSetup();
			var entry = entries.FirstOrDefault( x => x.scene.editorPath == scene.path );
			if ( entry == null )
			{
				GUI.Label( selectionRect, ColorText("Save to Generate AMS Entry", Color.red), _justifyRightLabel );
				return;
			}

			if ( entry.loadMethod == AmsMultiSceneSetup.LoadMethod.Additive || entry.loadMethod == AmsMultiSceneSetup.LoadMethod.AdditiveAsync )
			{
				var buildEntry = EditorBuildSettings.scenes.FirstOrDefault(x => x.path == scene.path);
				if ( buildEntry == null || !buildEntry.enabled )
				{
					// Draw this next to the drop-down.
					Rect textRect = new Rect(selectionRect);
					textRect.xMax -= 100.0f;
					GUI.Label( textRect, ColorText( "Not in Build", Color.red ), _justifyRightLabel );
				}
			}

			EditorGUI.BeginChangeCheck();
			selectionRect.xMin = selectionRect.xMax - 100.0f;
			entry.loadMethod = (AmsMultiSceneSetup.LoadMethod)EditorGUI.EnumPopup( selectionRect, entry.loadMethod );
			if ( EditorGUI.EndChangeCheck() )
				EditorSceneManager.MarkSceneDirty( sceneSetup.gameObject.scene );
		}

		/// <summary>
		/// Return the Scene that belongs to a particular Handle (which manifests as a HashCode)
		/// </summary>
		/// <param name="handleID">The handle to search for</param>
		/// <returns>The Scene from the SceneManager</returns>
		private static Scene GetSceneFromHandleID( int handleID )
		{
			int numScenes = EditorSceneManager.sceneCount;
			for (int i = 0 ; i < numScenes ; ++i)
			{
				var scene = EditorSceneManager.GetSceneAt(i);
				if ( scene.GetHashCode() == handleID )
					return scene;
			}

			return new Scene();
		}

		/** Helper function to color text using RTF format */
		static string ColorText( string text, Color32 color )
		{
			return System.String.Format( "<color=#{1:X2}{2:X2}{3:X2}{4:X2}>{0}</color>", text, color.r, color.g, color.b, color.a );
		}
	}
}
