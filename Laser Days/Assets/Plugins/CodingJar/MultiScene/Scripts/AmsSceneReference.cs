using UnityEngine;
using UnityEngine.SceneManagement;
using CodingJar.MultiScene;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[System.Serializable]
public struct AmsSceneReference
{
	public string	editorAssetGUID;
	public string	name;

	[UnityEngine.Serialization.FormerlySerializedAs("path")]
	[SerializeField]	string	_path;

	public AmsSceneReference( Scene scene ) : this( scene.path ) {}

	public AmsSceneReference( string scenePath )
	{
#if UNITY_EDITOR
		var scene = EditorSceneManager.GetSceneByPath( scenePath );
		editorAssetGUID = AssetDatabase.AssetPathToGUID( scenePath );
#else
		var scene = SceneManager.GetSceneByPath( scenePath );
		editorAssetGUID = "";
#endif

		name = scene.name;
		_path = scene.path;
	}

	/// <summary>
	/// Need to override Equals for renames
	/// </summary>
	public override bool Equals( object obj )
	{
		if ( !(obj is AmsSceneReference) )
			return false;

		var otherScene = (AmsSceneReference)obj;
		return (editorAssetGUID == otherScene.editorAssetGUID);
	}

	/// <summary>
	/// C# wants GetHashCode() overwritten if we overwrite Equals.  It's the HashCode of the Asset.
	/// </summary>
	/// <returns>The hash code of the asset</returns>
	public override int GetHashCode()
	{
		return editorAssetGUID.GetHashCode();
	}
	
	public Scene scene
	{
		get
		{
#if UNITY_EDITOR
			if ( !Application.isPlaying )
			{
				// The easy case
				var scene = EditorSceneManager.GetSceneByPath( editorPath );
				if ( scene.IsValid() )
				{
					// Just update our name and path while we're here... this helps with renames.
					name = scene.name;
					_path = scene.path;

					return scene;
				}

				// Welcome to my hell!
				// If we're building a scene that was open, we have temporary scenes named 0.backup, 1.backup etc.
				if ( BuildPipeline.isBuildingPlayer )
				{
					// Find the scene by using the stored path
					var allMultiSceneSetups = Resources.FindObjectsOfTypeAll<AmsMultiSceneSetup>();
					foreach( var sceneSetup in allMultiSceneSetups )
					{
						// Did we just find it?  Should I double-check that we're in a Temp path?
						if ( sceneSetup.scenePath == editorPath )
							return sceneSetup.gameObject.scene;
					}
				}

				return scene;
			}
#endif	// UNITY_EDITOR

			// Try the editor path first, it works at least in the Editor
			var editorScene = SceneManager.GetSceneByPath( editorPath );
			if ( editorScene.IsValid() )
				return editorScene;

			return SceneManager.GetSceneByPath( runtimePath );
		}
	}

	public bool	IsValid()
	{
		return scene.IsValid();
	}

	public bool isLoaded
	{
		get	{	return scene.isLoaded;	}
	}

	public string editorPath
	{
		get
		{
#if UNITY_EDITOR
			// In the Editor, we can follow renames by using the GUID.
			if ( !string.IsNullOrEmpty( editorAssetGUID ) )
				return AssetDatabase.GUIDToAssetPath( editorAssetGUID );
#endif

			return _path;
		}

		set { _path = value; }
	}

	public string runtimePath
	{
		get
		{
			int startIndex = 0;
			int endIndex = _path.Length;

			if ( _path.StartsWith( "Assets/" ) )
			{
				startIndex = "Assets/".Length;
				endIndex -= startIndex;
			}

			if ( _path.EndsWith(".unity") )
			{
				endIndex -= ".unity".Length;
			}

			return _path.Substring( startIndex, endIndex );
		}
	}
}
