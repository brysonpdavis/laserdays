using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;

namespace CodingJar.MultiScene.Editor
{
	/// <summary>
	/// Functionality for mapping object references that occur across multiple scenes.
	///	We can't do much with them for now, so we just warn about them.
	/// </summary>
	internal static class AmsCrossSceneReferenceProcessor
	{
		// Map that puts a SerializedProperty to what it's referencing
		// e.g. 	GameObject'/SubScenes/Art/Cube.SomeBehaviour.ToggleRender' ->
		//			MeshRenderer'/SubScenes/GamePlay/SomeObject.MeshRenderer'
		private static List<KeyValuePair<SerializedProperty,Object>> _referenceMap;

		// Map that stores types to their cross-scene reference data processors.  E.g. PlayableDirector -> AmsPlayableDirectorCrossSceneData.
		private static Dictionary<System.Type, CustomDataProcessorDelegate>		_customCrossSceneReferenceDataProcessors = new Dictionary<System.Type, CustomDataProcessorDelegate>();

		/// <summary>
		/// A method of adding custom data to a reference.
		/// </summary>
		/// <param name="crossRef">The Editor-side cross scene reference data</param>
		/// <returns>A list of GenericData which we can use in a custom resolver on the runtime side (or null)</returns>
		public delegate List<GenericData>	CustomDataProcessorDelegate( EditorCrossSceneReference crossRef );

		/// <summary>
		/// Add a specialized custom data resolver to the cross-scene reference system.  This allows specialized classes like PlayableDirector (which uses IExposedPropertyTable) to store
		/// restorable cross-scene reference data.
		/// </summary>
		/// <typeparam name="ObjectType">The type that needs custom data to properly store a cross-scene reference (e.g. PlayableDirector)</typeparam>
		/// <param name="customDataProcessor">The delegate to compute the data we need to store in order to restore the reference properly</param>
		public static void AddCustomCrossSceneDataProcessor<ObjectType>( CustomDataProcessorDelegate customDataProcessor ) where ObjectType : Object
		{
			_customCrossSceneReferenceDataProcessors[ typeof(ObjectType) ] = customDataProcessor;
		}

		/// <summary>
		/// Compute the references and return them for processing.  The computation is cached so it's safe to call this
		///	multiple times per frame without a horrendous slowdown.
		/// </summary>
		/// <returns>List of all of the cross-scene references</returns>
		private static List<EditorCrossSceneReference> ComputeAllCrossSceneReferences()
		{
			UpdateReferencesMap();
			return ComputeCrossSceneReferences( _referenceMap );
		}

		public static List<EditorCrossSceneReference> GetCrossSceneReferencesForScenes( IEnumerable<Scene> scenes )
		{
			// Start by logging out the cross-scene references out
			var crossSubSceneRefs = ComputeAllCrossSceneReferences();

			// Remove all of the cross-scene references that do not pertain to our input SubScenes list
			crossSubSceneRefs.RemoveAll( x => !scenes.Contains( x.fromScene ) );

			return crossSubSceneRefs;
		}

		/// <summary>
		/// This will update the map of SerializedProperties to Object ID
		/// </summary>
		private static void UpdateReferencesMap()
		{
			if ( _referenceMap != null )
				return;

			double startTime = EditorApplication.timeSinceStartup;

			// Get all of the valid MonoBehaviours or ScriptableObjects
			var allMonoBehaviourObjs = Resources.FindObjectsOfTypeAll<MonoBehaviour>().Where( x => !EditorUtility.IsPersistent(x) ).Cast<Object>();
			var allScriptableObjs = Resources.FindObjectsOfTypeAll<ScriptableObject>().Where( x => 
				!EditorUtility.IsPersistent(x) && !typeof(EditorWindow).IsAssignableFrom(x.GetType())
			).Cast<Object>();

			// We assume we're up to date for a single frame...
			EditorApplication.delayCall += () => { _referenceMap = null; };
			_referenceMap = new List<KeyValuePair<SerializedProperty, Object>>( allMonoBehaviourObjs.Count() + allScriptableObjs.Count() );

			// Figure out what they're referencing
			PopulateReferenceMap( _referenceMap, allMonoBehaviourObjs );
			PopulateReferenceMap( _referenceMap, allScriptableObjs );

			// Now we have all of the custom data processors.  Note that we don't want to run over-top of what we've already processed, so let's exclude all of those...
			foreach ( var customType in _customCrossSceneReferenceDataProcessors.Keys )
			{
				// Handle the cases where we've already added these objects to the list...
				if ( typeof(MonoBehaviour).IsAssignableFrom(customType) )
					continue;

				if ( typeof(ScriptableObject).IsAssignableFrom(customType) )
					continue;

				// Now grab all of the objects and add them to our reference map
				var sceneCustomObjects = Resources.FindObjectsOfTypeAll( customType ).Where( x => !EditorUtility.IsPersistent(x) );
				PopulateReferenceMap( _referenceMap, sceneCustomObjects );
			}

			AmsDebug.LogPerf( null, "Cross-Scene Reference Map Update: {0}", (EditorApplication.timeSinceStartup - startTime) );
		}

		/// <summary>
		/// Populate a reference map that goes SerializedProperty -> Object
		/// </summary>
		/// <param name="map">The map to populate entries into</param>
		/// <param name="allObjects">The objects to read in order to determine the references</param>
		private static void PopulateReferenceMap( List<KeyValuePair<SerializedProperty, Object>> map, IEnumerable<Object> allObjects )
		{
			foreach( var obj in allObjects )
			{
				// Flags that indicate we aren't rooted in the scene
				if ( obj.hideFlags == HideFlags.HideAndDontSave )
					continue;

				SerializedObject so = new SerializedObject(obj);
				SerializedProperty sp = so.GetIterator();

                bool bCanDispose = true;
				while ( sp.Next(true) )
				{
					// Only care about object references
					if ( sp.propertyType != SerializedPropertyType.ObjectReference )
						continue;

					// Skip the nulls
					if ( sp.objectReferenceInstanceIDValue == 0 )
						continue;

					map.Add( new KeyValuePair<SerializedProperty,Object>(sp.Copy(), sp.objectReferenceValue) );
                    bCanDispose = false;
				}

                // This will help relieve memory pressure (thanks llde_chris)
                if ( bCanDispose )
                {
                    sp.Dispose();
                    so.Dispose();
                }
			}
		}

		/// <summary>
		/// Search the given map for cross-scene references.
		/// </summary>
		/// <param name="map">The map to search</param>
		/// <returns>The list of cross-scene references found in the map</returns>
		private static List<EditorCrossSceneReference> ComputeCrossSceneReferences( List<KeyValuePair<SerializedProperty, Object>> map )
		{
			var refSubSceneMap = new List<EditorCrossSceneReference>();
			var cache = new Dictionary<Object,Scene>();

			// We figure out what the subScene mapping is (from which subScene to which subScene).
			foreach( var pair in map )
			{
				var fromObject = pair.Key.serializedObject.targetObject;
				var toObject = pair.Value;

				Scene fromScene = FindSceneCached( fromObject, cache );
				Scene toScene = FindSceneCached( toObject, cache );

				// Always safe to reference nothing, or a persistent object
				if ( !fromScene.IsValid() || !toScene.IsValid() )
					continue;

				bool bIsWithinSameScene = fromScene.Equals(toScene);
				if ( bIsWithinSameScene )
					continue;

				var crossRef = CreateEditorCrossSceneReference( fromObject, fromScene, pair.Key, toObject, toScene );
				refSubSceneMap.Add( crossRef );
			}

			return refSubSceneMap;
		}

		private static EditorCrossSceneReference CreateEditorCrossSceneReference( Object fromObject, Scene fromScene, SerializedProperty fromProperty, Object toObject, Scene toScene )
		{
			EditorCrossSceneReference crossRef = new EditorCrossSceneReference();
			crossRef.fromScene = fromScene;
			crossRef.toScene = toScene;
			crossRef.fromProperty = fromProperty;
			crossRef.toInstance = toObject;

			// Now see if we have a custom processor...
			CustomDataProcessorDelegate customDataProcessor;
			if ( _customCrossSceneReferenceDataProcessors.TryGetValue( fromObject.GetType(), out customDataProcessor ) )
			{
				var customData = customDataProcessor( crossRef );
				if ( customData != null && customData.Count > 0 )
					crossRef.data = customData;
			}

			return crossRef;
		}

		/// <summary>
		/// Find the Scene of an object instance and use a cache to help look-ups.
		/// </summary>
		/// <param name="instance">The instance to look at</param>
		/// <param name="cache">The cache to populate</param>
		/// <returns>The scene instance belongs to</returns>
		private static Scene FindSceneCached( Object instance, Dictionary<Object, Scene> cache )
		{
			// Null SubScene
			if ( !instance )
				return new Scene();

			// Quick case, it's already cached...
			Scene scene = new Scene();
			if ( cache.TryGetValue(instance, out scene) )
				return scene;

			// Persistent objects are Assets
			if ( !EditorUtility.IsPersistent(instance) )
			{
                var gameObj = GameObjectEx.EditorGetGameObjectFromComponent( instance );
                if ( gameObj )
					scene = gameObj.scene;
			}

			cache.Add(instance, scene);
			return scene;
		}

        /// <summary>
        /// Save all of the passed-in cross-scene references.  The entries in the passed-in list will be removed as they are properly accounted for.
        /// </summary>
        /// <param name="editorCrossSceneRefs"></param>
        public static void SaveCrossSceneReferences( List<EditorCrossSceneReference> editorCrossSceneRefs )
        {
            // Save all of the cross-scene references, removing them from our input list as we receive them
            for( int i = editorCrossSceneRefs.Count-1 ; i >= 0 ; --i)
            {
                var xRef = editorCrossSceneRefs[i];

				AmsDebug.Log( null, "Saving Cross-Scene Reference: {0}", xRef );
				
                try
                {
					RuntimeCrossSceneReference serializedReference = xRef.ToSerializable();

					try
					{
						// Save the object
						var initialObject = xRef.fromProperty.objectReferenceValue;
						
						// Resolve it (this can throw exceptions)
						AmsCrossSceneReferenceResolver.Resolve( serializedReference );
#if UNITY_5_6_OR_NEWER
						xRef.fromProperty.serializedObject.UpdateIfRequiredOrScript();
#else
						xRef.fromProperty.serializedObject.UpdateIfDirtyOrScript();
#endif

						// Check to make sure it resolved properly
						if ( initialObject && xRef.fromProperty.objectReferenceValue != initialObject )
							throw new ResolveException( string.Format("Resolve should have pointed to {0} ({1}) but instead resolved to {2} ({3})", initialObject ? initialObject.ToString() : "(null)", initialObject ? initialObject.GetInstanceID() : 0, xRef.fromProperty.objectReferenceValue, xRef.fromProperty.objectReferenceInstanceIDValue) );
					}
					catch ( System.Exception ex )
					{
						AmsDebug.LogError( xRef.fromObject, "Could not perform a runtime resolve on cross-scene reference {0}.\nReason: {1}. Please review Documentation.", serializedReference, ex.Message );
						continue;
					}

					// Record the cross-scene reference
					var crossSceneRefBehaviour = AmsCrossSceneReferences.GetSceneSingleton( xRef.fromScene, true );
					crossSceneRefBehaviour.AddReference( serializedReference );

                    // Add an updated reference map value
                    if ( _referenceMap != null )
                        _referenceMap.Add( new KeyValuePair<SerializedProperty, Object>(xRef.fromProperty, xRef.fromProperty.objectReferenceValue) );
                }
                catch ( UnityException ex )
                {
                    Debug.LogException( ex );
                }
            }
        }

	} // class
} // namespace