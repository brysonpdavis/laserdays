using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

namespace CodingJar.MultiScene.Editor
{
	public static class AmsPlaymodeHandler
	{
		[InitializeOnLoadMethod]
		private static void SaveCrossSceneReferencesBeforePlayInEditMode()
		{
#if UNITY_2017_2_OR_NEWER
			EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#else
			EditorApplication.playmodeStateChanged += EditorApplication_playModeStateChanged;
#endif
		}

#if UNITY_2017_2_OR_NEWER
		private static void EditorApplication_playModeStateChanged( PlayModeStateChange playmodeState )
		{
			bool isExitingEditMode = (playmodeState == PlayModeStateChange.ExitingEditMode);
#else
		private static void EditorApplication_playModeStateChanged()
		{
			bool isExitingEditMode = !EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode;
#endif

#if UNITY_5_6_OR_NEWER
			if ( EditorUtility.scriptCompilationFailed )
			{
				AmsDebug.Log( null, "Skipping cross-scene references due to compilation errors" );
				return;
			}
#endif

			if ( isExitingEditMode )
			{
				List<Scene> allScenes = new List<Scene>( EditorSceneManager.sceneCount );
				for (int i = 0 ; i < EditorSceneManager.sceneCount ; ++i)
				{
					var scene = EditorSceneManager.GetSceneAt(i);
					if ( scene.IsValid() && scene.isLoaded )
						allScenes.Add( scene );
				}

				AmsDebug.Log( null, "Handling Cross-Scene Referencing for Playmode" );
				AmsSaveProcessor.HandleCrossSceneReferences( allScenes );
			}
		}
	} // class
} // namespace