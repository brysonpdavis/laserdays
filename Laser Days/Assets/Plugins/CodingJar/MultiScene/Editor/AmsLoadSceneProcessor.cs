using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;

using CodingJar.MultiScene;

namespace CodingJar.MultiScene.Editor
{
	/// <summary>
	/// Detect if a scene has been opened.  If it has, we need to add our AmsMultiSceneSetup to it.
	/// </summary>
	public static class AmsLoadSceneProcessor
	{
		[InitializeOnLoadMethod]
		static void AssemblyReloaded()
		{
			EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
		}

		/// <summary>
		/// When a scene is opened, determine if we must add an AmsMultiSceneSetup to it
		/// </summary>
		/// <param name="scene">The scene that was opened</param>
		/// <param name="mode">How it was opened</param>
		private static void EditorSceneManager_sceneOpened( Scene scene, OpenSceneMode mode )
		{
			if ( !scene.isLoaded )
				return;

			var sceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( scene, false );
			if ( !sceneSetup )
			{
				sceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( scene, true );
				sceneSetup.transform.SetSiblingIndex(0);
				sceneSetup.OnHierarchyChanged();

				// Let's not mark it dirty as it doesn't need to be dirtied (really) as nothing's changed yet.
				//EditorSceneManager.MarkSceneDirty( scene );
			}
		}
	} // class
} // namespace