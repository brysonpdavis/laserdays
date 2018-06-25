using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CodingJar.MultiScene
{
	/// <summary>
	/// Allow us to toggle logging and other features on/off through the editor menu (and during runtime in the future)
	/// </summary>
	public static partial class AmsPreferences
	{
		public enum CrossSceneReferenceHandling
		{
			UnityDefault,		// Unity default handling (we do nothing, Unity probably duplicates the object into your scene)
			DoNotDuplicate,		// We zero-out the reference and warn about it, but nothing else
			Save				// We save the cross-scene reference and handle it automatically
		}

#if UNITY_EDITOR
		public static bool AllowAutoload
		{
			get { return    EditorPrefs.GetBool("AmsAllowAutoload", true); }
			set {           EditorPrefs.SetBool("AmsAllowAutoload", value); }
		}

        public static CrossSceneReferenceHandling CrossSceneReferencing
        {
            get { return    (CrossSceneReferenceHandling)EditorPrefs.GetInt("AmsCrossSceneReferencing", (int)CrossSceneReferenceHandling.Save); }
            set {           EditorPrefs.SetInt("AmsCrossSceneReferencing", (int)value);	}
        }

		public static bool VerboseLogging
		{
			get { return    EditorPrefs.GetBool("AmsDebugLog", false); }
			set {           EditorPrefs.SetBool("AmsDebugLog", value); }
		}

		public static bool PerfLogging
		{
			get { return    EditorPrefs.GetBool("AmsDebugPerfLog", false); }
			set {           EditorPrefs.SetBool("AmsDebugPerfLog", value); }
		}

		public static bool DebugEnabled
		{
			get { return	EditorPrefs.GetBool("AmsDebugEnabled", false);	}
			set { EditorPrefs.SetBool("AmsDebugEnabled", value);				}
		}
#else
#endif

#if UNITY_EDITOR
		[PreferenceItem("Multi-Scene")]
		static void AmsPreferencesOnGUI()
		{
            CrossSceneReferenceHandling newCrossSceneRefs = (CrossSceneReferenceHandling)EditorGUILayout.EnumPopup( "Cross-Scene Referencing", CrossSceneReferencing );
            bool bNewAutoload = EditorGUILayout.Toggle( "Allow SubScene Auto-Load", AllowAutoload );

			EditorGUILayout.Space();

			bool bNewVerboseLog = EditorGUILayout.Toggle( "Verbose Logging", VerboseLogging );
			bool bNewPerfLog = EditorGUILayout.Toggle( "Performance Logging", PerfLogging );

			//EditorGUILayout.Space();
			//bool bNewDebug = EditorGUILayout.Toggle( "Enable Debug Options", DebugEnabled );

			if ( GUI.changed )
			{
				CrossSceneReferencing = newCrossSceneRefs;
				AllowAutoload = bNewAutoload;
				VerboseLogging = bNewVerboseLog;
				PerfLogging = bNewPerfLog;
				//DebugEnabled = bNewDebug;

				GUI.changed = false;
			}

#if false
			if ( DebugEnabled )
			{
				EditorGUILayout.Space();

				bool bNewDisableDrawer = EditorGUILayout.Toggle( "DEBUG Disable Hierarchy Drawer", DebugDisableHierarchyDrawer );

				EditorGUI.indentLevel += 1;
				GUI.enabled = !bNewDisableDrawer;
				bool bNewDebugShowGameObjectFlags = EditorGUILayout.Toggle( "Draw GameObject Flags", DebugShowGameObjectFlags );
				GUI.enabled = true;
				EditorGUI.indentLevel -= 1;

				bool bNewDisableModificationProcessor = EditorGUILayout.Toggle( "DEBUG Disable Modification Processor", DebugDisableModificationProcessor );
				bool bNewShowBookkeepingObjects = EditorGUILayout.Toggle( "DEBUG Show Bookkeping Objects", DebugShowBookkepingObjects );

				if ( GUI.changed )
				{
					DebugDisableHierarchyDrawer = bNewDisableDrawer;
					DebugDisableModificationProcessor = bNewDisableModificationProcessor;
					DebugShowBookkepingObjects = bNewShowBookkeepingObjects;
					DebugShowGameObjectFlags = bNewDebugShowGameObjectFlags;
				}
			}
#endif
		} // static void AmsPreferencesOnGUI

#if !UNITY_5_3 // (i.e. 5.4+)
		[InitializeOnLoadMethod]
		static void InitCrossSceneReferenceHandling()
		{
			UnityEditor.SceneManagement.EditorSceneManager.preventCrossSceneReferences = (CrossSceneReferencing == CrossSceneReferenceHandling.UnityDefault);
		}
#endif	// !UNITY_5_3 (i.e. 5.4+)
#endif	// UNITY_EDITOR

	} // class 
} // namespace 
