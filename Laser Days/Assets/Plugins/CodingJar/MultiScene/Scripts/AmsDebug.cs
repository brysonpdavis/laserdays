using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using CodingJar;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CodingJar.MultiScene
{
	/// <summary>
	/// Editor extensions for working with Multi-Scene Editing.
	/// </summary>
	public static class AmsDebug
	{
		/// <summary>
		/// Restore cross-scene references for all unlocked and opened scenes.
		/// </summary>
		internal static void EditorConditionalRestoreAllCrossSceneReferences()
		{
			/*
			foreach( var subScene in GetSubScenesSortedBottomUp() )
			{
				if ( subScene && !subScene.IsLocked() )
				{
					CrossSceneReferenceProcessor.RestoreCrossSceneReferences(subScene);
				}
			}
			 * */
		}

		/// <summary>
		/// Helper function so we can disable logging
		/// </summary>
		public static void Log( Object context, string message, params object[] parms )
		{
#if UNITY_EDITOR
			if ( AmsPreferences.VerboseLogging )
#endif
				Debug.LogFormat( context, "Ams Plugin: " + message, parms );
		}

		/// <summary>
		/// Helper function so we prefix our messages consistently
		/// </summary>
		public static void LogWarning( Object context, string message, params object[] parms )
		{
			Debug.LogWarningFormat( context, "Ams Plugin: " + message, parms );
		}

		/// <summary>
		/// Helper function so we prefix our messages consistently
		/// </summary>
		public static void LogError( Object context, string message, params object[] parms )
		{
			Debug.LogErrorFormat( context, "Ams Plugin: " + message, parms );
		}

		/// <summary>
		/// Helper function so we can disable logging
		/// </summary>
		public static void LogPerf( Object context, string message, params object[] parms )
		{
#if UNITY_EDITOR
			if ( AmsPreferences.PerfLogging )
#endif
				Debug.LogFormat( context, "Ams Perf: " + message, parms );
		}

	} // class
} // namespace 
