using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CodingJar;

using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
#endif

namespace CodingJar.MultiScene
{
    /// <summary>
	/// Holds cross-scene references for a particular scene.
    /// </summary>
	[ExecuteInEditMode]
    public sealed class AmsCrossSceneReferences : MonoBehaviour
#if UNITY_EDITOR
		, ISerializationCallbackReceiver
#endif
    {
		// We are versioning now
		const int CurrentSerializedVersion = 1;
		[SerializeField, HideInInspector]	int							_version;

        [SerializeField]    private List<RuntimeCrossSceneReference>	_crossSceneReferences = new List<RuntimeCrossSceneReference>();
		[SerializeField, HideInInspector]	private List<GameObject>	_realSceneRootsForPostBuild = new List<GameObject>();

		// When we merge into a scene, we need to redirect the cross-scene references
		[SerializeField, HideInInspector]	private List<AmsSceneReference>	_mergedScenes = new List<AmsSceneReference>();
		private static Dictionary<AmsSceneReference, AmsSceneReference>		_activeMergedScenes = new Dictionary<AmsSceneReference,AmsSceneReference>();
		
		private List<RuntimeCrossSceneReference>	_referencesToResolve = new List<RuntimeCrossSceneReference>();

		/// <summary>
		/// An event that you can register with in order to receive notifications that a cross-scene reference was restored.
		/// </summary>
		public static event System.Action<RuntimeCrossSceneReference>	CrossSceneReferenceRestored;

		/// <summary>
		/// Return the Singleton for a given scene (there is one per Scene).
		/// </summary>
		/// <param name="scene">The Scene to obtain the singleton for</param>
		/// <returns>The per-scene singleton</returns>
		public static AmsCrossSceneReferences	GetSceneSingleton( Scene scene, bool bCreateIfNotFound )
		{
			var multiSceneSetup = GameObjectEx.GetSceneSingleton<AmsMultiSceneSetup>( scene, bCreateIfNotFound );
			if ( !multiSceneSetup )
				return null;

			var existing = multiSceneSetup.gameObject.GetComponent<AmsCrossSceneReferences>();
			if ( existing )
				return existing;
			else if ( bCreateIfNotFound )
				return multiSceneSetup.gameObject.AddComponent<AmsCrossSceneReferences>();

			return null;
		}

		/// <summary>
		/// Adds a cross-scene reference.
		/// </summary>
		/// <param name="reference"></param>
		public void AddReference( RuntimeCrossSceneReference reference )
		{
			int index = _crossSceneReferences.FindIndex( reference.IsSameSource );
			if ( index >= 0 )
			{
				_crossSceneReferences[index] = reference;
			}
			else
			{
				_crossSceneReferences.Add( reference );
			}
		}

		/// <summary>
		/// Remove all of the stored cross-scene references that reference 'toScene'.
		/// </summary>
		public void ResetCrossSceneReferences( Scene toScene )
		{
			_crossSceneReferences.RemoveAll( x => (x.toScene.scene == toScene) );
		}

		void Awake()
		{
			AmsDebug.Log( this, "{0}.Awake() Scene: {1}. IsLoaded: {2}. Path: {3}. Frame: {4}. Root Count: {5}", GetType().Name, gameObject.scene.name, gameObject.scene.isLoaded, gameObject.scene.path, Time.frameCount, gameObject.scene.rootCount );

			// Upgrade us (we upgraded the Cross-Scene Reference data)
			// This may freak out users on their first run since it will look like their cross-scene references are gone
			// In reality, they are just in the scene, ready to be re-created on save
			if ( _version < CurrentSerializedVersion )
			{
				int numCrossSceneRefs = _crossSceneReferences.Count;
				ConditionalResolveReferences( _crossSceneReferences );

				if ( numCrossSceneRefs != _crossSceneReferences.Count )
				{
					AmsDebug.LogWarning( this, "{0} was upgraded. {1} cross-scene references will be re-created on next save.", name, numCrossSceneRefs - _crossSceneReferences.Count );
				}
				else
				{
					AmsDebug.LogWarning( this, "{0} needs upgrading.  Please resave {1}", name, gameObject.scene.name );
				}

#if UNITY_EDITOR
				if ( !Application.isPlaying )
				{
                    GameObject gObject = this.gameObject;
					EditorApplication.delayCall += () => { if ( gObject ) EditorSceneManager.MarkSceneDirty( gObject.scene ); };
				}
#endif
			}

			// Make sure we keep track of all of the merged scenes
			AmsSceneReference thisScene = new AmsSceneReference( gameObject.scene );
			foreach( var prevScene in _mergedScenes )
				_activeMergedScenes.Add( prevScene, thisScene );

			// We need to queue our cross-scene references super early in case we get merged.
			_referencesToResolve.Clear();
			_referencesToResolve.AddRange( _crossSceneReferences );

			// We should be able to merge our cross-scene references to any other loaded scene here.
			ResolvePendingCrossSceneReferences();
		}

		void Start()
		{
			AmsDebug.Log( this, "{0}.Start() Scene: {1}. IsLoaded: {2}. Path: {3}. Frame: {4}. Root Count: {5}", GetType().Name, gameObject.scene.name, gameObject.scene.isLoaded, gameObject.scene.path, Time.frameCount, gameObject.scene.rootCount );

            // A build might have just been performed, in that case clean-up the leftovers.
            PerformPostBuildCleanup();

			// For some reason in Awake(), the scene we belong to isn't considered "loaded"?!  We've tried to work around that, but if it failed, try again here.
			ResolvePendingCrossSceneReferences();

			// Register to these callbacks only once
			AmsMultiSceneSetup.OnStart -= HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnStart += HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnDestroyed -= HandleSceneDestroyed;
			AmsMultiSceneSetup.OnDestroyed += HandleSceneDestroyed;
		}

		void OnDestroy()
		{
			AmsMultiSceneSetup.OnStart -= HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnDestroyed -= HandleSceneDestroyed;

			// Make sure we keep track of all of the merged scenes
			foreach( var prevScene in _mergedScenes )
				_activeMergedScenes.Remove( prevScene );
		}

		/// <summary>
		/// Whenever another scene is loaded, we have another shot at resolving more cross-scene references
		/// </summary>
		/// <param name="sceneSetup">The AmsMultiSceneSetup that was loaded</param>
		private void HandleNewSceneLoaded( AmsMultiSceneSetup sceneSetup )
		{
			var loadedScene = sceneSetup.gameObject.scene;
			if ( !loadedScene.isLoaded )
				Debug.LogErrorFormat( this, "{0} Received HandleNewSceneLoaded from scene {1} which isn't considered loaded.  The scene MUST be considered loaded by this point", GetType().Name, loadedScene.name );

			// Restore any references to this newly loaded scene
			for (int i = 0 ; i < _crossSceneReferences.Count ; ++i)
			{
				var xRef = _crossSceneReferences[i];

				if ( !xRef.fromObject )
				{
					AmsDebug.LogWarning( this, "xRef Index {0} had Null source (probably stale), Consider removing (via right-click on entry)", i ); 
					continue;
				}

				if ( !_referencesToResolve.Contains(xRef) && xRef.toScene.scene == loadedScene )
					_referencesToResolve.Add( xRef );
			}

			if ( _referencesToResolve.Count > 0 )
			{
				AmsDebug.Log( this, "Scene {0} Loaded. {1} Cross-Scene References (in total) from Cross-Scene Manager in {2} are queued for resolve.", loadedScene.name, _referencesToResolve.Count, gameObject.scene.name );
				ConditionalResolveReferences( _referencesToResolve );
			}
		}

		/// <summary>
		/// Whenever a scene is destroyed, we will receive this callback.  In the editor, we can remember that we may be about to lose a cross-scene reference.
		/// </summary>
		/// <param name="sceneSetup"></param>
		private void HandleSceneDestroyed( AmsMultiSceneSetup sceneSetup )
		{
			var destroyedScene = sceneSetup.gameObject.scene;
			if ( !destroyedScene.IsValid() )
				return;

			// If our own scene is being destroyed, we don't need to do anymore work
			if ( destroyedScene == gameObject.scene )
				return;

			// Remove all of the pending refs for that scene.
			_referencesToResolve.RemoveAll( x => x.toScene.scene == destroyedScene );

			// Now we re-add all of the relevant refs to pending.  They'll be re-resolved when the scene is loaded again.
			var allRelevantRefs = _crossSceneReferences.Where( x => x.toScene.scene == destroyedScene );
			_referencesToResolve.AddRange( allRelevantRefs );
		}

		/// <summary>
		/// This is a co-routine for waiting until a given scene is loaded, then performing a cross-scene reference resolve
		/// </summary>
		/// <param name="scene">The scene to guarantee loaded</param>
		System.Collections.IEnumerator	CoWaitForSceneLoadThenResolveReferences( Scene scene )
		{
			if ( !Application.isPlaying )
			{
				Debug.LogErrorFormat( this, "CoWaitForSceneLoadThenResolveReferences called, but we're not playing. Co-routines do not work reliably in the Editor!" );
				yield break;
			}

			if ( !scene.IsValid() )
				yield break;

			while ( !scene.isLoaded )
				yield return null;

			ResolvePendingCrossSceneReferences();
		}

		[ContextMenu("Retry Pending Resolves")]
		public void ResolvePendingCrossSceneReferences()
		{
			ConditionalResolveReferences( _referencesToResolve );
		}

		[ContextMenu("Retry ALL Resolves")]
		private void RetryAllResolves()
		{
			_referencesToResolve.Clear();
			_referencesToResolve.AddRange( _crossSceneReferences );
			
			ResolvePendingCrossSceneReferences();
		}

		private void ConditionalResolveReferences( List<RuntimeCrossSceneReference> references )
		{
			var fromScene = gameObject.scene;
			for(int i = references.Count - 1 ; i >= 0 ; --i)
			{
				var xRef = references[i];

				if ( !xRef.fromObject )
				{
					AmsDebug.LogWarning( this, "Missing Source Object for xRef at (possible index) {0}: {1}", i, xRef );
					continue;
				}

				try
				{
					// See if it's a reference to a merged scene... if so we need to redirect
					var toScene = xRef.toScene;
					if ( !toScene.IsValid() )
					{
						AmsSceneReference mergedSceneRedirect;
						if ( _activeMergedScenes.TryGetValue( toScene, out mergedSceneRedirect ) )
						{
							AmsDebug.Log( this, "Redirecting cross scene reference {0} from original target scene {1} to scene {2}", xRef, toScene.name, mergedSceneRedirect.name );
							toScene = mergedSceneRedirect;
							xRef.toScene = mergedSceneRedirect;
						}
					}

					// Debug information
					AmsDebug.Log( this, "{0}.ConditionalResolveReferences() Scene: {1}. xRef: {2}. fromSceneLoaded: {3}. toSceneLoaded: {4}.", GetType().Name, fromScene.name, xRef, fromScene.isLoaded, toScene.isLoaded );

					if ( toScene.isLoaded )
					{
						// Remove it from our list (assuming it goes through)
						references.RemoveAt( i );

						AmsDebug.Log( this, "Restoring Cross-Scene Reference {0}", xRef );
						AmsCrossSceneReferenceResolver.Resolve( xRef );

						// Notify any listeners
						if ( CrossSceneReferenceRestored != null )
							CrossSceneReferenceRestored( xRef );
					}
				}
				catch ( ResolveException ex )
				{
					string message = ex.Message;

					// Make it easier for us to find where the cross-scene reference is
					Object context = xRef.fromObject;
					if ( !context )
						context = this;

					Debug.LogErrorFormat( context, "{0} in {1}: {2}", GetType().Name, gameObject.scene.name, message );
				}
				catch ( System.Exception ex )
				{
					Debug.LogException( ex, this );
				}
			}
		}

		void PerformPostBuildCleanup()
		{
			if ( Application.isEditor && !Application.isPlaying && _realSceneRootsForPostBuild.Count > 0 )
			{
				GameObject[] newSceneRoots = gameObject.scene.GetRootGameObjects();
				foreach( GameObject root in newSceneRoots )
				{
					if ( !_realSceneRootsForPostBuild.Contains(root) )
					{
						AmsDebug.LogWarning( this, "Destroying '{0}/{1}' since we've determined it's a temporary for a cross-scene reference", gameObject.scene.name, root.name );
						DestroyImmediate( root );
					}
				}

				_realSceneRootsForPostBuild.Clear();
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Callback that this object has been deserialized which can happen on a level-load, or on a hot recompile
		/// </summary>
		public void OnAfterDeserialize()
		{
			// We should register for this callback (but only once).
			AmsMultiSceneSetup.OnStart -= HandleNewSceneLoaded;
			AmsMultiSceneSetup.OnStart += HandleNewSceneLoaded;

			AmsMultiSceneSetup.OnDestroyed -= HandleSceneDestroyed;
			AmsMultiSceneSetup.OnDestroyed += HandleSceneDestroyed;
		}

		/// <summary>
		/// Callback that this object is about to be serialized which can happen when saving, or when simply inspecting the object.
		/// </summary>
		public void OnBeforeSerialize()
		{
			_version = CurrentSerializedVersion;

			if ( !BuildPipeline.isBuildingPlayer )
				return;

			if ( EditorSceneManager.sceneCount < 2 )
				return;

			// Give us one last shot...
			ResolvePendingCrossSceneReferences();

			// This deserves an explanation.  This gets serialized right before we do a build IF this scene is active
			// at the time of building.  The problem is, when we come back from the build, we get duplicate prefabs in
			// the scene since the cross-scene reference was still active.  So what we wanna do, is delete those when
			// we come back from the build.
			gameObject.scene.GetRootGameObjects( _realSceneRootsForPostBuild  );
		}

		/// <summary>
		/// This is called during the build pipeline to ensure a proper merge from one scene into another, taking into account cross-scene references
		/// </summary>
		/// <param name="sourceSceneSetup">The scene we're merging from</param>
		/// <param name="destSceneSetup">The scene we're merging to</param>
		public static void EditorBuildPipelineMergeScene( AmsMultiSceneSetup sourceSceneSetup, AmsMultiSceneSetup destSceneSetup )
		{
			// This is happening during the build system, so we're going to end up with a scene name of 0.backup
			// So we need to get the actual path from the AmsMultiSceneSetup object and clobber it.
			var amsFromSceneRef = new AmsSceneReference(sourceSceneSetup.gameObject.scene);
			amsFromSceneRef.editorPath = sourceSceneSetup.scenePath;

			var amsIntoSceneRef= new AmsSceneReference(destSceneSetup.gameObject.scene);
			amsIntoSceneRef.editorPath = destSceneSetup.scenePath;

			// Now get the cross-scene references from both scenes to merge them
			var srcCrossSceneRefs = GetSceneSingleton( sourceSceneSetup.gameObject.scene, false );
			if ( !srcCrossSceneRefs )
				return;

			var destCrossSceneRefs = GetSceneSingleton( destSceneSetup.gameObject.scene, true );

			for(int i = 0 ; i < srcCrossSceneRefs._crossSceneReferences.Count ; ++i)
			{
				var xRef = srcCrossSceneRefs._crossSceneReferences[i];
				if ( !srcCrossSceneRefs._referencesToResolve.Contains(xRef) )
				{
					AmsDebug.Log( srcCrossSceneRefs, "Already resolved xRef {0}. No need to merge it.", xRef );
					continue;
				}

				AmsDebug.Log( destSceneSetup, "Merging {0} into Scene {1}", xRef, amsIntoSceneRef.editorPath );
				xRef.DEPRECATED_fromScene = amsIntoSceneRef;
				destCrossSceneRefs.AddReference( xRef );
			}

			// Mark this as a merged scene in the destination, so when we look-up cross-scene references we're aware.
			destCrossSceneRefs._mergedScenes.Add( amsFromSceneRef );

			// Destroy this object after the merge is complete (we don't want it merged into the scene)
			GameObject.DestroyImmediate( srcCrossSceneRefs.gameObject, false );
		}

		/// <summary>
		/// Warn on any cross-scene reference that has yet to be resolved
		/// </summary>
		public bool EditorWarnOnUnresolvedCrossSceneReferences()
		{
			// Warn about any references that were not yet resolved...
			foreach( var xRef in _referencesToResolve )
			{
				Debug.LogWarningFormat( "Did not resolve Cross-Scene Reference during build: {0}", xRef );
			}

			return _referencesToResolve.Count > 0;
		}
#endif

	} // class
} // namespace
