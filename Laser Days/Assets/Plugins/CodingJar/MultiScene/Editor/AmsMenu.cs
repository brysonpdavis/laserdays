using UnityEngine;
using UnityEditor;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using System.Collections.Generic;

namespace CodingJar.MultiScene.Editor
{
	/// <summary>
	/// Class that holds menu items
	/// </summary>
	static class AmsMenu
	{
#if TODO_DEBUG
		[MenuItem("Tools/Advanced Multi-Scene/Debug")]
		private static void ShowDebugMenu()
		{
			EditorUtility.DisplayPopupMenu( new Rect(0.0f, 0.0f, 100.0f, 100.0f), "CONTEXT/AmsDebug", new MenuCommand(null) );
		}

		[MenuItem("Tools/Advanced Multi-Scene/Debug", true)]
		private static bool AllowDebugMenuItem()
        {
			return AmsPreferences.DebugEnabled;
        }
#endif

		[MenuItem("Tools/Advanced Multi-Scene/Support")]
		private static void GotoForumThread()
		{
			System.Diagnostics.Process.Start( "http://forum.unity3d.com/threads/advanced-multi-scene-now-available.383525/" );
		}

		[MenuItem("Tools/Advanced Multi-Scene/Detect Cross-Scene Refs")]
		public static void DebugShowAllCrossSceneReferences()
		{
			List<Scene> allScenes = new List<Scene>();
			for(int i = 0 ; i < EditorSceneManager.sceneCount ; ++i)
				allScenes.Add( EditorSceneManager.GetSceneAt(i) );

			var allCrossSceneRefs = AmsCrossSceneReferenceProcessor.GetCrossSceneReferencesForScenes( allScenes );
			foreach( var xRef in allCrossSceneRefs )
			{
				Debug.LogFormat( "Cross Scene Reference: {0}", xRef );
			}
		}

	}
}
