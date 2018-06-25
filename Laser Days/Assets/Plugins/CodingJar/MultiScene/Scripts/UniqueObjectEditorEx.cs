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
	/// Editor extensions for UniqueObject to make manipulations quicker (and stronger) when using the Editor.
	/// </summary>
	public partial struct UniqueObject
	{
#if UNITY_EDITOR
		private static int	CurrentSerializedVersion = 1;

		private int	GetEditorId( Object obj )
		{
			int editorId = 0;

			PropertyInfo inspectorModeInfo = typeof(UnityEditor.SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
			SerializedObject sObj = new SerializedObject(obj);
			inspectorModeInfo.SetValue( sObj, UnityEditor.InspectorMode.Debug, null );

			SerializedProperty sProp = sObj.FindProperty( "m_LocalIdentfierInFile" );
			if ( sProp != null )
			{
				editorId = sProp.intValue;
				sProp.Dispose();
			}

			sObj.Dispose();

			return editorId;
		}

		private Object	EditorResolveSlow()
		{
			var scene = this.scene.scene;

			if ( !scene.IsValid() )
				return null;

			if ( !scene.isLoaded )
				return null;

			// Find the object (this is potentially very slow).
			Object[] allObjs = EditorUtility.CollectDeepHierarchy( scene.GetRootGameObjects() );
			foreach( var obj in allObjs )
			{
				// Apparently this happens... bummer
				if ( !obj )
					continue;

				// If it's a prefab, it's a lot more difficult since Unity doesn't store IDs
				var prefabObj = PrefabUtility.GetPrefabObject(obj);
				if ( prefabObj && editorLocalId == GetEditorId(prefabObj) )
				{
					GameObject gameObject = GameObjectEx.EditorGetGameObjectFromComponent( obj );
					if ( !gameObject )
					{
						Debug.LogWarningFormat( obj, "Could not SLOW resolve {0} (cross-scene reference is broken). Pointing it to {1}.", this, obj );
						return obj;
					}
					
					// Make sure we do this from the root
					var prefabRoot = PrefabUtility.FindPrefabRoot(gameObject);
					if ( prefabRoot )
						gameObject = prefabRoot;

					// If we have a relative path, grab that GameObject
					if ( !string.IsNullOrEmpty(editorPrefabRelativePath) )
					{
						Transform transform = gameObject.transform.Find( editorPrefabRelativePath );
						if ( transform )
							gameObject = transform.gameObject;
						else
							Debug.LogWarningFormat( gameObject, "Tried to perform a slow resolve for {0} and found prefab {1}, but could not resolve the expected sub-path {2} which indicates the Prefab instance path was renamed.", this, gameObject, editorPrefabRelativePath );
					}

					Debug.LogWarningFormat( gameObject, "Performed a slow resolve on {0} due to a rename.  We found PREFAB with same ID named {1} (but we're not sure). Attempting a resolve with it.", this, gameObject );
					fullPath = GameObjectEx.GetFullName( gameObject );
					return RuntimeResolve();
				}
				else if ( editorLocalId == GetEditorId(obj) )
				{
					GameObject gameObject = GameObjectEx.EditorGetGameObjectFromComponent( obj );
					Debug.LogWarningFormat( obj, "Performed a slow resolve on {0} and found {1} ({2}). Double-check this is correct.", this, gameObject ? gameObject.GetFullName() : "(Non-Existant GameObject)", obj.GetType() );
					return obj;
				}
			}

			return null;
		}

		public UniqueObject( Object obj )
		{
			scene = new AmsSceneReference();
			fullPath = string.Empty;
			componentName = string.Empty;
			version = CurrentSerializedVersion;
			componentIndex = 0;
			editorLocalId = 0;
			editorPrefabRelativePath = string.Empty;

			if ( !obj )
				return;

			GameObject gameObject = GameObjectEx.EditorGetGameObjectFromComponent( obj );
			if ( gameObject )
			{
				scene = new AmsSceneReference( gameObject.scene );
				fullPath = gameObject.GetFullName();

				Component comp = obj as Component;
				if ( comp )
				{
					componentName = obj.GetType().AssemblyQualifiedName;
					gameObject.GetComponents( obj.GetType(), _reusableComponentsList );
					componentIndex = _reusableComponentsList.IndexOf( comp );
				}
			}

			// Get the prefab object
			var prefabObj = PrefabUtility.GetPrefabObject(obj);
			if ( prefabObj )
			{
				GameObject prefabRoot = PrefabUtility.FindPrefabRoot( gameObject );
				editorLocalId = prefabRoot ? GetEditorId(prefabRoot) : GetEditorId( obj );
				editorPrefabRelativePath = TransformEx.GetPathRelativeTo( gameObject.transform, prefabRoot.transform ); 
			}

			if ( editorLocalId == 0 )
				editorLocalId = GetEditorId( obj );
		}
#endif // UNITY_EDITOR

	} // struct
} // namespace 
