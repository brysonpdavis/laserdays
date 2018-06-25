using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Linq;

using CodingJar;

#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
#endif

namespace CodingJar.MultiScene
{
	[ExecuteInEditMode]
	public class AmsMultiSceneSetup : MonoBehaviour, ISerializationCallbackReceiver
	{
		[System.Serializable]
		public enum LoadMethod
		{
			Baked,
			Additive,
			AdditiveAsync,
			DontLoad
		}

		[System.Serializable]
		public class SceneEntry
		{
			// The scene that is automatically processed both in Editor and Runtime
			[BeginReadonly]
			public AmsSceneReference	scene;

			[Tooltip("Should this be automatically loaded in the Editor?")]
			[UnityEngine.Serialization.FormerlySerializedAs("isLoaded")]
			public bool					loadInEditor;

			[EndReadonly]
			[Tooltip("How should we load this scene at Runtime?")]
			public LoadMethod			loadMethod;

			public AsyncOperation		asyncOp { get; set; }

			public override string ToString()
			{
				return string.Format( "{0} loadInEditor: {1} loadMethod: {2}", scene.name, loadInEditor, loadMethod );
			}

			/// <summary>
			/// Overridden Equals to we can compare entries.  Entries with the same scene references and load settings are considered equal.
			/// </summary>
			public override bool Equals( object obj )
			{
				if ( this == obj )
					return true;

				var other = obj as SceneEntry;
				if ( other == null )
					return false;

				return (scene.Equals(other.scene)) && (loadInEditor == other.loadInEditor) && (loadMethod == other.loadMethod) && (asyncOp == other.asyncOp);
			}

			public override int GetHashCode()
			{
				return scene.GetHashCode() * 4 + loadInEditor.GetHashCode() * 2 + loadMethod.GetHashCode();
			}

#if UNITY_EDITOR
			/// <summary>
			/// Construct from a Unity SceneSetup
			/// </summary>
			public SceneEntry( UnityEditor.SceneManagement.SceneSetup sceneSetup )
			{
				scene = new AmsSceneReference( sceneSetup.path );

				loadInEditor = sceneSetup.isLoaded;
				loadMethod = LoadMethod.Additive;
			}
#endif
		}

		[SerializeField]	bool				_isMainScene = false;
		[SerializeField]	List<SceneEntry>	_sceneSetup = new List<SceneEntry>();
		[Readonly, SerializeField]	string				_thisScenePath;

#if UNITY_EDITOR
		/// <summary> The co-routine that runs in play-in-editor mode that ensures our scenes are baked correctly </summary>
		private Coroutine _waitingToBake = null;
		private List<SceneEntry>	_bakedScenesLoading = new List<SceneEntry>();	// Currently loading or loaded
#endif
		private List<SceneEntry>	_bakedScenesMerged = new List<SceneEntry>();	// Already merged

		public static System.Action<AmsMultiSceneSetup>	OnAwake;
		public static System.Action<AmsMultiSceneSetup>	OnStart;
		public static System.Action<AmsMultiSceneSetup>	OnDestroyed;
		
#if UNITY_EDITOR
		/// <summary>
		/// Easy accessor for Editor
		/// </summary>
		public bool isMainScene
		{
			get { return _isMainScene; }
			set { _isMainScene = value; }
		}

		/// <summary>
		/// Easy accessor for the Editor
		/// </summary>
		public string scenePath
		{
			get { return _thisScenePath; }
		}
#endif

		/// <summary>
		/// Read-only access to the Scene Setup.
		/// </summary>
		/// 
#if UNITY_METRO && !UNITY_UWP
		public IList<SceneEntry>	GetSceneSetup()
		{
			return _sceneSetup;
		}
#else
		public System.Collections.ObjectModel.ReadOnlyCollection<SceneEntry>	GetSceneSetup()
		{
			return _sceneSetup.AsReadOnly();
		}
#endif

		/// <summary>
		/// Awake can be used to tell anyone that a Scene has just been loaded.
		/// Due to a bug in PostProcessScene, this is the first thing to occur in a loaded scene.
		/// </summary>
		void Awake()
		{
			AmsDebug.Log( this, "{0}.Awake() (Scene {1}). IsLoaded: {2}. Frame: {3}", GetType().Name, gameObject.scene.name, gameObject.scene.isLoaded, Time.frameCount );

#if UNITY_EDITOR
			if ( !BuildPipeline.isBuildingPlayer )
				_thisScenePath = gameObject.scene.path;
#endif

			// Notify any listeners we're now awake
			if ( OnAwake != null )
				OnAwake( this );

			if ( _isMainScene )
			{
				if ( !Application.isEditor || gameObject.scene.isLoaded || Time.frameCount > 1 )
					LoadSceneSetup();
			}
		}

		void OnDestroy()
		{
#if UNITY_EDITOR
			// Make sure we update the settings every time we unload/load a Scene

#if UNITY_2018
			EditorApplication.hierarchyChanged -= OnHierarchyChanged;
#else
			EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
#endif

#endif

			if ( OnDestroyed != null )
				OnDestroyed( this );
		}

		/// <summary>
		/// Start is executed just before the first Update (a frame after Awake/OnEnable).
		/// We execute the Scene loading here because Unity has issues loading scenes during the initial Awake/OnEnable calls.
		/// </summary>
		void Start()
		{
			AmsDebug.Log( this, "{0}.Start() Scene: {1}. Frame: {2}", GetType().Name, gameObject.scene.name, Time.frameCount );

			// Notify any listeners (like the cross-scene referencer)
			if ( OnStart != null )
				OnStart( this );

			// Second chance at loading scenes
			if ( _isMainScene )
				LoadSceneSetup();

#if UNITY_EDITOR
			// Make sure we update the settings every time we unload/load a Scene
			// This is strategically placed after the LoadSceneSetup().
			if ( !EditorApplication.isPlaying )
			{
#if UNITY_2018
				EditorApplication.hierarchyChanged -= OnHierarchyChanged;
				EditorApplication.hierarchyChanged += OnHierarchyChanged;
#else
				EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
				EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
#endif
			}
#endif
			}

		void LoadSceneSetup()
		{
			if ( !_isMainScene )
				return;

		#if UNITY_EDITOR

			if ( !Application.isPlaying )
				LoadSceneSetupInEditor();
			else
				LoadSceneSetupAtRuntime();
			
		#else

			LoadSceneSetupAtRuntime();
			
		#endif
		}

		/// <summary>
		/// Load Scene Setup at Runtime.
		/// </summary>
		private void LoadSceneSetupAtRuntime()
		{
			List<SceneEntry> sceneSetup = new List<SceneEntry>( _sceneSetup );
			foreach( var entry in sceneSetup )
			{
				LoadEntryAtRuntime( entry );
			}
		}

		/// <summary>
		/// Load a particular Scene Entry
		/// </summary>
		/// <param name="entry">The Entry to load</param>
		private void LoadEntryAtRuntime( SceneEntry entry )
		{
			// Don't load 
			if ( entry.loadMethod == LoadMethod.DontLoad )
				return;

			// Already loaded, try editor first
			var existingScene = SceneManager.GetSceneByPath(entry.scene.editorPath);

			// Try runtime path
			if ( !existingScene.IsValid() )
				existingScene = SceneManager.GetSceneByPath(entry.scene.runtimePath);

#if UNITY_EDITOR
			// Could be we just created the scene because it's baked
			if ( !existingScene.IsValid() )
				existingScene = SceneManager.GetSceneByName(entry.scene.runtimePath);

			if ( Application.isEditor && entry.loadMethod == LoadMethod.Baked )
			{
				// If we've already processed this, return early
				if ( _bakedScenesLoading.Contains(entry) || _bakedScenesMerged.Contains(entry) )
					return;

				// We're loading this entry, don't allow this to be re-entrant
				_bakedScenesLoading.Add(entry);

				if ( !existingScene.IsValid() )
				{
					// This allows us to load the level even in playmode
					EditorApplication.LoadLevelAdditiveInPlayMode( entry.scene.editorPath );
				}

				// Loading a scene can take multiple frames so we have to wait.
				// Baking scenes can only take place when they're all loaded due to cross-scene referencing
				if ( _waitingToBake != null )
					StopCoroutine( _waitingToBake );

				_waitingToBake = StartCoroutine( CoWaitAndBake() );
				return;
			}
#endif

			// If it's already loaded, return early
			if( existingScene.IsValid() )
				return;

			if ( entry.loadMethod == LoadMethod.AdditiveAsync )
			{
				AmsDebug.Log( this, "Loading {0} Asynchronously from {1}", entry.scene.name, gameObject.scene.name );
				entry.asyncOp = SceneManager.LoadSceneAsync( entry.scene.runtimePath, LoadSceneMode.Additive );
				return;
			}

			if ( entry.loadMethod == LoadMethod.Additive )
			{
				AmsDebug.Log( this, "Loading {0} from {1}", entry.scene.name, gameObject.scene.name );
				SceneManager.LoadScene( entry.scene.runtimePath, LoadSceneMode.Additive );
				return;
			}
		}

		/// <summary>
		/// This executes in the Editor when a behaviour is initially added to a GameObject.
		/// </summary>
		void Reset()
		{
			_isMainScene = (SceneManager.GetActiveScene() == gameObject.scene);
		}

		// Intentionally left blank
		public void OnAfterDeserialize() {}

		/// <summary>
		/// OnBeforeSerialize is called whenever we're about to save or inspect this Component.
		/// We want to match exactly what the Editor has in terms of Scene setup, so we do it here.
		/// </summary>
		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			if ( !this || BuildPipeline.isBuildingPlayer || Application.isPlaying )
				return;

			// Save off the scene path
			if ( gameObject && gameObject.scene.IsValid() )
				_thisScenePath = gameObject.scene.path;
#endif
		}

#if UNITY_EDITOR
		public void OnHierarchyChanged()
		{
			OnBeforeSerialize();

			// We don't care about the scene setup
			if ( !_isMainScene )
				return;

			var newSceneSetup = new List<SceneEntry>();
			var activeScene = EditorSceneManager.GetActiveScene();

			// Update our scene setup
			bool bForceDirty = false;
			SceneSetup[] editorSceneSetup = EditorSceneManager.GetSceneManagerSetup();
			for(int i = 0 ; i < editorSceneSetup.Length ; ++i)
			{
				// If we're the active scene, don't save it.
				var editorEntry = editorSceneSetup[i];
				if ( editorEntry.path == activeScene.path )
					continue;

				var newEntry = new SceneEntry(editorEntry);
				newSceneSetup.Add( newEntry );
				
				// Save the baked settings
				var oldEntry = _sceneSetup.Find( x => newEntry.scene.Equals(x.scene) );
				if ( oldEntry != null )
				{
					newEntry.loadMethod = oldEntry.loadMethod;
					
					// We need to update the path if the runtime paths aren't the same (implies a rename)
					bForceDirty = bForceDirty || (newEntry.scene.runtimePath != oldEntry.scene.runtimePath);
				}
			}

			// If we had a new scene setup...
			if ( bForceDirty || !newSceneSetup.SequenceEqual(_sceneSetup) )
			{
				_sceneSetup = newSceneSetup;
				EditorUtility.SetDirty( this );

				if ( gameObject )
					EditorSceneManager.MarkSceneDirty( gameObject.scene );
			}
		}

		/// <summary>
		/// Loads the scene setup in the Editor
		/// </summary>
		private void LoadSceneSetupInEditor()
		{
			// Don't load this if we just dragged in a scene.
			if ( EditorSceneManager.GetActiveScene() != gameObject.scene )
				return;

			foreach( var entry in _sceneSetup )
			{
				LoadEntryInEditor(entry);
			}
		}

		/// <summary>
		/// Loads a particular Scene Entry in the Editor
		/// </summary>
		/// <param name="entry">The entry to load</param>
		private void LoadEntryInEditor( SceneEntry entry )
		{
			// Bad time to do this.
			if ( EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying )
				return;

			// We can't do this
			if ( string.IsNullOrEmpty(entry.scene.editorPath) || entry.scene.editorPath == gameObject.scene.path )
				return;

			bool bShouldLoad = entry.loadInEditor && AmsPreferences.AllowAutoload;
			var scene = entry.scene.scene;

			try
			{
				if ( !scene.IsValid() )
				{
					if ( bShouldLoad )
					{
						AmsDebug.Log( this, "Scene {0} is loading Scene {1} in Editor", gameObject.scene.name, entry.scene.name );
						EditorSceneManager.OpenScene( entry.scene.editorPath, OpenSceneMode.Additive );
					}
					else
					{
						AmsDebug.Log( this, "Scene {0} is opening Scene {1} (without loading) in Editor", gameObject.scene.name, entry.scene.name );
						EditorSceneManager.OpenScene( entry.scene.editorPath, OpenSceneMode.AdditiveWithoutLoading );
					}
				}
				else if ( bShouldLoad != scene.isLoaded )
				{
					if ( bShouldLoad && !scene.isLoaded )
					{
						AmsDebug.Log( this, "Scene {0} is loading existing Scene {1} in Editor", gameObject.scene.name, entry.scene.name );
						EditorSceneManager.OpenScene( entry.scene.editorPath, OpenSceneMode.Additive );
					}
					else
					{
						AmsDebug.Log( this, "Scene {0} is closing Scene {1} in Editor", gameObject.scene.name, entry.scene.name );
						EditorSceneManager.CloseScene( scene, false );
					}
				}
			} catch ( System.Exception ex ) {	Debug.LogException( ex, this );	}
		}
#endif

		/// <summary>
		/// Scene loads take a frame to complete, so we wait until all of the baked scenes are loaded, then we merge them.
		/// </summary>
		private System.Collections.IEnumerator	CoWaitAndBake()
		{
			// Ensure ALL of the Baked Scenes are loaded
			bool bAllLoaded = false;
			while ( !bAllLoaded )
			{
				bAllLoaded = true;
				foreach( var entry in _sceneSetup )
				{
					// For now, we wait until EVERYTHING is loaded so the cross-scene references are correct.
					bAllLoaded = bAllLoaded && (entry.loadMethod == LoadMethod.DontLoad || entry.scene.isLoaded || _bakedScenesMerged.Contains(entry));
				}

				// We're not all loaded, wait another frame.
				if ( !bAllLoaded )
					yield return null;
			}

			// Give us the ability to fix-up the cross-scene references.
			foreach( var entry in _sceneSetup )
			{
				// If it's Invalid, it's already baked.
				if ( CanMerge(entry) )
					PreMerge(entry);
			}

			// Now merge the scenes
			foreach( var entry in _sceneSetup )
			{
				if ( CanMerge(entry) )
				{
					MergeScene(entry);
					_bakedScenesMerged.Add( entry );
				}
			}
		}

		/// <summary>
		/// Are we ready to merge in this scene entry?
		/// </summary>
		/// <param name="entry">The entry to merge</param>
		/// <returns>True iff it can be merged</returns>
		private bool CanMerge( SceneEntry entry )
		{
			if ( entry.loadMethod != LoadMethod.Baked )
				return false;

			var scene = entry.scene.scene;
			if ( !scene.IsValid() )
				return false;

			var activeScene = SceneManager.GetActiveScene();
			if ( scene == activeScene || scene == gameObject.scene )
				return false;

			if ( !gameObject.scene.isLoaded )
				return false;

			return true;
		}

		/// <summary>
		/// 'Bakes' a scene by merging it into our scene.
		/// </summary>
		/// <param name="entry">The entry to bake</param>
		private void PreMerge( SceneEntry entry )
		{
			var scene = entry.scene.scene;

			// This is a last chance before that scene gets destroyed.
			var crossSceneRefs = AmsCrossSceneReferences.GetSceneSingleton( scene, false );
			if ( crossSceneRefs )
				crossSceneRefs.ResolvePendingCrossSceneReferences();
		}

		private void MergeScene( SceneEntry entry )
		{
			var scene = entry.scene.scene;

			// Make sure there is only ever one AmsMultiSceneSetup in a given scene
			var sourceSetup = GameObjectEx.GetSceneSingleton< AmsMultiSceneSetup >( scene, false );				
			if ( sourceSetup )
				GameObject.Destroy( sourceSetup.gameObject );

			AmsDebug.Log( this, "Merging {0} into {1}", scene.path, gameObject.scene.path );
			SceneManager.MergeScenes( scene, gameObject.scene );
		}
	}
}
