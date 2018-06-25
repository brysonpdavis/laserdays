using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace CodingJar.MultiScene
{
	/// <summary>
	/// A UniqueObject that is represented by a Scene and an ID in the Scene.
	/// </summary>
	[System.Serializable]
	public partial struct UniqueObject
	{
		public AmsSceneReference	scene;
		public string				fullPath;
		public string				componentName;
		public int					componentIndex;
		public int					editorLocalId;
		public string				editorPrefabRelativePath;

		// So we can version and auto-upgrade
		[SerializeField, HideInInspector]	private int		version;

		private static List<Component>	_reusableComponentsList = new List<Component>();
		private static Scene?			_dontDestroyOnLoadScene = new Scene?();

		/// <summary>
		/// Helper method to get the DontDestroyOnLoad Scene.  It's inaccessible by standard Unity methods.
		/// </summary>
		/// <returns>The DontDestroyOnLoad Scene.</returns>
		private static Scene GetDontDestroyOnLoadScene()
		{
			if ( !_dontDestroyOnLoadScene.HasValue )
			{
				GameObject temp = new GameObject( "AMS-DontDestroyOnLoad-Finder" );
				Object.DontDestroyOnLoad( temp );

				_dontDestroyOnLoadScene = temp.scene;
				Object.DestroyImmediate( temp );
			}

			return _dontDestroyOnLoadScene.Value;
		}

		/// <summary>
		/// Resolve a cross-scene reference if possible.
		/// </summary>
		/// <returns>The cross-scene referenced object if it's possible</returns>
		private Object	RuntimeResolve()
		{
			var scene = this.scene.scene;

			if ( !scene.IsValid() )
				return null;

			// Try to find the Object with our custom method that checks only the subscene
			GameObject gameObject = GameObjectEx.FindBySceneAndPath( scene, fullPath );

			// If that fails, we can try using Unity's GameObject.Find in case the user has switched it on us, or 
			// in the case that's it's in a DontDestroyOnLoad scene
			if ( !gameObject )
			{
				gameObject = GameObject.Find( fullPath );

				// It's truly failed
				if ( !gameObject )
					return null;
				else
					AmsDebug.LogWarning( gameObject, "UniqueObject '{0}' resolved unexpected to '{1}'{2}.  Did you move it manually?", this, gameObject.scene.name, gameObject.GetFullName() );
			}

			if ( string.IsNullOrEmpty(componentName) )
				return gameObject;

			// This is the old method where we didn't store the component index (deprecated)
			if ( version < 1 )
			{
				Component oldStyleComponent = gameObject.GetComponent( componentName );
				if ( componentIndex < 0 || oldStyleComponent )
					return oldStyleComponent;
			}

			// Get the component and index
			System.Type type = System.Type.GetType( componentName, false );
			if ( type != null )
			{
				gameObject.GetComponents( type, _reusableComponentsList );
				if ( componentIndex < _reusableComponentsList.Count )
					return _reusableComponentsList[componentIndex];
			}

			return null;
		}

		/// <summary>
		/// Attempt to resolve the UniqueObject to an actual Object.  Can throw exceptions.
		/// This can do deep (but slow) resolves if used in the Editor.
		/// </summary>
		internal Object		Resolve()
		{
			Object obj = RuntimeResolve();

#if UNITY_EDITOR
			if ( !obj && !Application.isPlaying )
				obj = EditorResolveSlow();
#endif

			return obj;
		}

		/// <summary>
		/// See if two UniqueObjects point to the same Object
		/// </summary>
		public override bool Equals( object obj )
		{
			if ( !(obj is UniqueObject) )
				return base.Equals(obj);

			UniqueObject other = (UniqueObject)obj;
			if ( !scene.Equals(other.scene) )
				return false;

			if( this.editorLocalId == other.editorLocalId )
				return true;

			return (this.componentIndex == other.componentIndex) && (this.componentName == other.componentName) && (this.fullPath == other.fullPath);
		}

		/// <summary>
		/// You will get a warning if you override Equals without GetHashCode
		/// </summary>
		public override int GetHashCode()
		{
			if ( editorLocalId != 0 )
				return editorLocalId;

			return (fullPath + componentName + componentIndex).GetHashCode();
		}

		/// <summary>
		/// Give us an easy way to debug what a UniqueObject is pointing to
		/// </summary>
		public override string ToString()
		{
			System.Type type = string.IsNullOrEmpty(componentName) ? null : System.Type.GetType( componentName, false );
			return string.Format( "{0}'{1}' ({2} #{3})", scene.name, fullPath, type != null ? type.FullName : "GameObject", componentIndex );
		}

	} // struct
} // namespace 
