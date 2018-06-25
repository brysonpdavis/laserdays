using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Linq;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace CodingJar.MultiScene.Editor
{
	/// <summary>
	/// This class listens for saves/loads and ensures the subScene data end up where they belong
	/// </summary>
	internal static class AmsScenePostProcessor
	{
		private static void GetCommonParameters( ref Scene activeScene, ref AmsMultiSceneSetup activeSceneSetup, List<AmsMultiSceneSetup.SceneEntry> bakedScenes )
		{
			// We can only execute this when building the player.  Otherwise we expect entries to already be in the scene.
            if ( !BuildPipeline.isBuildingPlayer )
                return;

			// Get the SceneSetup for the Active Scene.
			activeScene = EditorSceneManager.GetActiveScene();
			activeSceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( activeScene, false );
			if ( !activeSceneSetup )
				return;

			var scenesInSetup = activeSceneSetup.GetSceneSetup();
			foreach( var entry in scenesInSetup )
			{
                bool bShouldBake = entry.loadMethod == AmsMultiSceneSetup.LoadMethod.Baked;
				if ( bShouldBake )
					bakedScenes.Add( entry );
			}
		}

		/// <summary>
		/// We want to bake all of the SubScenes that require baking
		/// </summary>
        [UnityEditor.Callbacks.PostProcessScene(-1000)]
        internal static void LoadScenesForMerging()
		{
			Scene activeScene = new Scene();
			AmsMultiSceneSetup activeSetup = null;
			List<AmsMultiSceneSetup.SceneEntry> bakedScenes = new List<AmsMultiSceneSetup.SceneEntry>();

			GetCommonParameters( ref activeScene, ref activeSetup, bakedScenes );
			if ( bakedScenes.Count < 1 )
				return;

			AmsDebug.Log( null, "Running LoadScenesForBaking on Scene {0}", activeScene.name );

			// Now load all of the scenes
			foreach( var entry in bakedScenes )
			{
				var realScene = entry.scene.scene;
				if ( !realScene.IsValid() )
				{
					// This is good.  This means it's not loaded yet.
					realScene = EditorSceneManager.OpenScene( entry.scene.editorPath, OpenSceneMode.Additive );

					if ( !realScene.IsValid() )
					{
						AmsDebug.LogError( activeSetup, "BakeScene: Scene {0} ({1}) referenced from Multi-Scene Setup in {2} is invalid.", entry.scene.editorPath, entry.scene.name, activeScene.name );
						continue;
					}
				}

				// Let's catch this...
				if ( !realScene.isLoaded )
				{
					realScene = EditorSceneManager.OpenScene( realScene.path, OpenSceneMode.Additive );
					
					// if we're still not loaded, we're probably in trouble.
					if ( !realScene.isLoaded )
					{
						AmsDebug.LogError( activeSetup, "BakeScene: Scene {0} ({1}) referenced from Multi-Scene Setup in {2} could not load.", entry.scene.editorPath, entry.scene.name, activeScene.name );
						continue;
					}
				}
			}
		}

		/// <summary>
		/// After the scenes are loaded, we need to restore the cross-scene references while the scenes are still intact.
		/// </summary>
		[UnityEditor.Callbacks.PostProcessScene(-750)]
		static void RestoreCrossSceneReferences()
		{
			Scene activeScene = new Scene();
			AmsMultiSceneSetup activeSetup = null;
			List<AmsMultiSceneSetup.SceneEntry> bakedScenes = new List<AmsMultiSceneSetup.SceneEntry>();

			GetCommonParameters( ref activeScene, ref activeSetup, bakedScenes );
			if ( bakedScenes.Count < 1 )
				return;

			AmsDebug.Log( null, "Running RestoreCrossSceneReferences on Scene {0}", activeScene.name );

			// Do the merge (bake)
			var targetCrossRefs = AmsCrossSceneReferences.GetSceneSingleton( activeScene, false );
			if ( targetCrossRefs )
				targetCrossRefs.ResolvePendingCrossSceneReferences();

			foreach( var entry in bakedScenes )
			{
				if ( !entry.scene.isLoaded )
				{
					AmsDebug.LogError( activeSetup, "Could not restore cross-scene references for non-loaded scene: {0}", entry.scene.name );
					continue;
				}

				var sourceCrossRefs = AmsCrossSceneReferences.GetSceneSingleton( entry.scene.scene, false );
				if ( sourceCrossRefs )
					sourceCrossRefs.ResolvePendingCrossSceneReferences();
			}
		}

		/// <summary>
		/// Finally, we want to merge all of the scenes
		/// </summary>
		[UnityEditor.Callbacks.PostProcessScene(-500)]
		static void MergeScenes()
		{
			Scene activeScene = new Scene();
			AmsMultiSceneSetup activeSetup = null;
			List<AmsMultiSceneSetup.SceneEntry> bakedScenes = new List<AmsMultiSceneSetup.SceneEntry>();

			GetCommonParameters( ref activeScene, ref activeSetup, bakedScenes );
			if ( bakedScenes.Count < 1 )
				return;

			AmsDebug.Log( null, "Running AMS MergeScenes on Scene {0} ({1})", activeScene.name, activeSetup.scenePath );

			foreach( var entry in bakedScenes )
			{
				if ( !entry.scene.isLoaded )
				{
					AmsDebug.LogError( activeSetup, "Could not merge non-loaded scene: {0}", entry.scene.name );
					continue;
				}

				// Merge the cross-scene references (and keep track of the merges)
				var bakedSceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( entry.scene.scene, false );
				if ( bakedSceneSetup )
					AmsCrossSceneReferences.EditorBuildPipelineMergeScene( bakedSceneSetup, activeSetup );

				AmsDebug.Log( null, "Running Unity MergeScenes for {0} into {1}", entry.scene.name, activeScene.name );
				EditorSceneManager.MergeScenes( entry.scene.scene, activeScene );
			}
		} // MergeScenes

		/// <summary>
		/// We should warn if any cross-scene references were not resolved during the building of the scene
		/// </summary>
		[UnityEditor.Callbacks.PostProcessScene(1)]
		static void WarnOnAllMissingCrossSceneRefs()
		{
			Scene activeScene = new Scene();
			AmsMultiSceneSetup activeSetup = null;
			List<AmsMultiSceneSetup.SceneEntry> bakedScenes = new List<AmsMultiSceneSetup.SceneEntry>();

			GetCommonParameters( ref activeScene, ref activeSetup, bakedScenes );
			if ( !activeSetup )
				return;

			var crossSceneReferences = activeScene.GetSceneSingleton<AmsCrossSceneReferences>( false );
			if ( crossSceneReferences && crossSceneReferences.EditorWarnOnUnresolvedCrossSceneReferences() )
			{
				Debug.LogWarningFormat( "Previous Cross-Scene Reference Errors were in {0}", activeSetup.scenePath );
			}
		}

	} // class AmsScenePostProcessor
} // namespace CodingJar.MultiScene.Editor