using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Needed for UWP
using System.Reflection;
using System.Linq;

namespace CodingJar
{
	/// <summary>
	/// GameObject Extensions.  Useful functionality for Unity's GameObject class.
	/// </summary>
	public static class GameObjectEx
	{
		/// <summary>
		/// Reusable list of GameObjects for non-allocating calls.
		/// </summary>
		private static List<GameObject> s_GameObjects = new List<GameObject>();

#if !UNITY_2017_1_OR_NEWER
		/// <summary>
		/// Store this method as a way of getting the root game objects in a scene before the scene is considered loaded
		/// </summary>
		private static System.Reflection.MethodInfo	s_GetRootGameObjectsInternal;

		static GameObjectEx()
		{
#if NETFX_CORE
			s_GetRootGameObjectsInternal = typeof(Scene).GetTypeInfo().DeclaredMethods.FirstOrDefault( x => x.Name == "GetRootGameObjectsInternal");
#else
			s_GetRootGameObjectsInternal = typeof(Scene).GetMethod( "GetRootGameObjectsInternal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic );
#endif
		}
#endif

		/**
		 * Get the full path from the root to this GameObject
		 */
		public static string GetFullName(this GameObject gameObj)
		{
            return gameObj.transform.FullPath();
		}

		/// <summary>
		/// Create a GameObject in a specific Scene
		/// </summary>
		/// <param name="name">The object name</param>
		/// <param name="scene">The target scene to create the object in</param>
		/// <returns>The GameObject instance</returns>
		public static GameObject	CreateGameObjectInScene( string name, Scene scene )
		{
			Scene oldActiveScene = SceneManager.GetActiveScene();
			SceneManager.SetActiveScene( scene );
			GameObject gameObject = new GameObject( name );
			SceneManager.SetActiveScene( oldActiveScene );

			return gameObject;
		}

		public static T		GetSceneSingleton<T>( this Scene scene, bool bCreate ) where T : MonoBehaviour
		{
			// Find the existing instance
			foreach( var gameObj in scene.GetRootGameObjects() )
			{
				var instance = gameObj.GetComponent<T>();
				if ( instance )
					return instance;
			}

			// Create a new one
			if ( bCreate )
			{
				GameObject gameObj = CreateGameObjectInScene( "! " + typeof(T).Name, scene );
				return gameObj.AddComponent<T>();
			}

			// None found
			return null;
		}

		/// <summary>
		/// Unity by default will not return root objects during the initialzation phase of a Scene (i.e. before Awake has completed).
		/// However, we definitely want to know during the Awake() period, so we go into the internal code to do so.
		/// </summary>
		/// <param name="scene">The scene to get root objects for</param>
		/// <returns>The root objects</returns>
		public static List<GameObject>	GetRootObjectsEvenIfNotLoaded( this Scene scene )
		{
			s_GameObjects.Clear();

			if ( scene.IsValid() )
			{
				s_GameObjects.Capacity = Mathf.Max( s_GameObjects.Capacity, scene.rootCount );
#if UNITY_2017_1_OR_NEWER
				scene.GetRootGameObjects( s_GameObjects );
#else
				s_GetRootGameObjectsInternal.Invoke( null, new object[] { scene.GetHashCode(), s_GameObjects } );
#endif
			}
				return s_GameObjects;
		}

		/// <summary>
		/// Gets the component on a given GameObject or creates it if it doesn't already exist.
		/// </summary>
		/// <typeparam name="T">The type of MonoBehaviour to look for (or add)</typeparam>
		/// <param name="gameObject">The GameObject to check/add</param>
		/// <returns>The instance of T (which either existed or was created)</returns>
		public static T		GetRequiredComponent<T>( this GameObject gameObject ) where T : Component
		{
			var instance = gameObject.GetComponent<T>();
			if ( instance )
				return instance;

			return gameObject.AddComponent<T>();
		}

		/// <summary>
		/// Find a GameObject by an absolute path regardless of if that GameObject is disabled or not.
		/// </summary>
		/// <param name="absPath">The absolute path (e.g. "/Path/To/Object" or "Scene/Path/To/Object")</param>
		/// <returns>The GameObject if found, null otherwise</returns>
		public static GameObject FindByAbsolutePath( string absolutePath )
		{
			for( int i = 0 ; i < SceneManager.sceneCount ; ++i )
			{
				GameObject gameObj = FindBySceneAndPath( SceneManager.GetSceneAt(i), absolutePath );
				if ( gameObj )
					return gameObj;
			}

			return null;
		}

		/// <summary>
		/// Find a GameObject in a specific scene with an absolute path.
		/// </summary>
		/// <param name="scene">The scene to search</param>
		/// <param name="absolutePath">The absolute path of the GameObject in the scene</param>
		/// <returns>The GameObject if found, null otherwise</returns>
		public static GameObject FindBySceneAndPath( Scene scene, string absolutePath )
		{
			string[] paths = absolutePath.Split('/');
			int numPaths = paths.Length;
			
			// Fail-safe
			if ( numPaths < 1 )
				return null;
			else if ( numPaths < 2 )
				paths = new string[2] { "/", paths[0] };

			foreach( var gameObject in scene.GetRootObjectsEvenIfNotLoaded() )
			{
				if ( gameObject.name != paths[1] )
					continue;

				Transform transform = gameObject.transform;
				for( int deep = 2 ; deep < numPaths && transform ; ++deep )
				{
					transform = transform.Find( paths[deep] );
				}

				if ( transform ) 
					return transform.gameObject;
			}

			return null;
		}
		
#if UNITY_EDITOR
		/**
		 * @Return true if this GameObject lives in the Scene.  False if it's part of an asset.
		 */
		public static bool EditorIsSceneObject( this GameObject gameObj )
		{
			string assetPath = AssetDatabase.GetAssetOrScenePath(gameObj);
			return !EditorUtility.IsPersistent(gameObj) || assetPath.EndsWith(".unity");
		}
		
		/**
		 * Get a nice, human-readable path for this GameObject. Takes into account if the GameObject lives in the Scene or an Asset.
		 */
		public static string EditorGetFriendlyPath( this GameObject gameObj )
		{
			if ( EditorIsSceneObject(gameObj) )
			{
				return GetFullName(gameObj) + " (Scene Object)";
			}
			else
			{
				string assetPath = AssetDatabase.GetAssetOrScenePath(gameObj);
				return assetPath + GetFullName(gameObj) + " (Asset Object)";
			}
		}

        /// <summary>
        /// Hack that returns all ScriptableObjects of a type attached to a GameObject (usually not allowed by the GUI)
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject to query for</typeparam>
        /// <param name="gameObj">The GameObject on which they are attached</param>
        /// <returns>A list of ScriptableObjects of type T attached on a specific GameObject</returns>
        public static IEnumerable<T>  EditorGetScriptableObjects<T>( this GameObject gameObj ) where T : ScriptableObject
        {
            List<T> returnList = new List<T>();

            SerializedObject so = new SerializedObject(gameObj);
            var spComponents = so.FindProperty( "m_Component" );

            for (int i = 0 ; i < spComponents.arraySize ; ++i)
            {
                // For some reason the Components has a first/second pair...
                var spElement = spComponents.GetArrayElementAtIndex(i);
                var spObjRef = spElement.FindPropertyRelative("second");

                var scriptObj = spObjRef.objectReferenceValue as T;
                if ( scriptObj )
                {
                    returnList.Add( scriptObj );
                }
            }

            return returnList;
        }

		/// <summary>
        /// This will give you a GameObject from a Component instance (or null if it's not attached).
        /// </summary>
        /// <param name="obj"> The object is a 'component' but can be our hack of being a ScriptableObject </param>
        /// <returns> The GameObject the component is attached to </returns>
        public static GameObject EditorGetGameObjectFromComponent( this Object obj )
        {
            if ( !obj )
                return null;

            int gameObjID = UnityEditorInternal.InternalEditorUtility.GetGameObjectInstanceIDFromComponent( obj.GetInstanceID() );
            return gameObjID != 0 ? (GameObject)EditorUtility.InstanceIDToObject(gameObjID) : (obj as GameObject);
        }

#endif
	}
}