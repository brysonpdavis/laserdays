#if UNITY_2017_1_OR_NEWER
using System.Collections.Generic;
using UnityEngine.Playables;

namespace CodingJar.MultiScene.Editor
{
	/// <summary>
	/// Custom class that allows us to add data to a PlayableDirector cross-scene reference in order for it to be restorable at runtime.
	/// </summary>
	internal static class AmsPlayableDirectorCrossSceneData
	{
		[UnityEditor.InitializeOnLoadMethod]
		static void RegisterCustomDataProcessor()
		{
			AmsCrossSceneReferenceProcessor.AddCustomCrossSceneDataProcessor<PlayableDirector>( GetCustomCrossSceneReferenceData );
		}

		/// <summary>
		/// Check a cross-scene reference for data that cannot be saved by simple field look-ups at runtime.
		/// </summary>
		static List<GenericData> GetCustomCrossSceneReferenceData( EditorCrossSceneReference crossRef )
		{
			var playableDirector = crossRef.fromObject as PlayableDirector;
			if ( !playableDirector )
				throw new System.ArgumentException( "crossRef.fromObject contained an incompatible class" );

			List<GenericData> genericData = new List<GenericData>();

			var fromProperty = crossRef.fromProperty;
			string propertyPath = fromProperty.propertyPath;
			var serializedObject = fromProperty.serializedObject;

			if ( propertyPath.StartsWith( "m_SceneBindings" ) && propertyPath.EndsWith( "value" ) )
			{
				var spElement = serializedObject.FindProperty( fromProperty.propertyPath.Substring( 0, fromProperty.propertyPath.Length - fromProperty.name.Length - 1 ) );
				genericData.Add( spElement.FindPropertyRelative( "key" ).objectReferenceValue );
			}
			else if ( propertyPath.StartsWith( "m_ExposedReferences" ) && propertyPath.EndsWith( "second" ) )
			{
				var spElement = serializedObject.FindProperty( propertyPath.Substring( 0, propertyPath.Length - fromProperty.name.Length - 1 ) );
				genericData.Add( spElement.FindPropertyRelative( "first" ).stringValue );
			}

			return genericData;
		}

	}
}
#endif