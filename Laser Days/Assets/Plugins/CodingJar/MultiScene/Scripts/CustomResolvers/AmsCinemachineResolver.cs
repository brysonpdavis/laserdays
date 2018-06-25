#if UNITY_2018_1_OR_NEWER
using System;
using System.Reflection;
using UnityEngine;

namespace CodingJar.MultiScene.CustomResolvers
{
	/// <summary>
	/// Custom resolver for Cinemachine in order to ensure the pipelines are correctly built if we've
	/// changed any of its internal variables.
	/// </summary>
	static class AmsCinemachineResolver
	{
		private static bool		hasCinemachine = false;

#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
#endif
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
		static void AddCustomResolver()
		{
			// Do a conservative check to see if we have Cinemachine in the project.
			hasCinemachine = false;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try 
                {
					foreach ( var type in assembly.GetTypes() )
					{
						if ( type.Namespace == "Cinemachine" )
						{
							hasCinemachine = true;
							break;
						}
					}
				}
                catch (System.Exception) {} // Just skip uncooperative assemblies

				if ( hasCinemachine )
					break;
            }

			if ( hasCinemachine )
				AmsCrossSceneReferenceResolver.AddCustomResolver( HandleCrossSceneReference );
		}

		/// <summary>
		/// Attempt to handle a cross-scene reference.
		/// </summary>
		static bool HandleCrossSceneReference( RuntimeCrossSceneReference xRef )
		{
			MonoBehaviour cinemachineBehaviour = xRef.fromObject as MonoBehaviour;
			if ( !cinemachineBehaviour || !cinemachineBehaviour.isActiveAndEnabled )
				return false;

			if ( !cinemachineBehaviour.GetType().Namespace.StartsWith( "Cinemachine" ) )
				return false;

			AmsDebug.LogWarning( xRef.fromObject, "xSceneRef on Cinemachine Behaviour: {0}. Disabling/Enabling to ensure pipeline is up to date.", xRef );
			cinemachineBehaviour.enabled = false;
			cinemachineBehaviour.enabled = true;

			return false;
		}
	}
}
#endif
