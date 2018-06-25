using UnityEngine;
using System.Linq;

namespace CodingJar
{
	/**
	 * Useful extension functions for Transform class
	 */
	public static class TransformEx
	{
		/**
		 * Destroy all of the children GameObjects for a given transform
		 */
		public static void DestroyChildren( this Transform transform )
		{
			if ( transform == null )
				return;

			for(int i = 0 ; i < transform.childCount ; ++i)
			{
				Transform child = transform.GetChild(i);

				// Deactivate first because destroy doesn't take effect until the end of the frame...
				child.gameObject.SetActive(false);
				GameObject.Destroy( child.gameObject );
			}
		}

		/**
		 * Print out the full path in "/Game Object/Child" format
		 */
		public static string FullPath( this Transform transform )
		{
			var sb = new System.Text.StringBuilder();

			while ( transform != null )
			{
				sb.Insert(0, transform.name);
				sb.Insert(0, '/');
				transform = transform.parent;
			}

			#if UNITY_EDITOR
			if (transform && UnityEditor.EditorUtility.IsPersistent(transform) )
			{
				sb.Append( " (Asset)" );
			}
			#endif

			return sb.ToString();
		}

		/**
		 * Returns a path relative to a parent path.  Useful for determining animation curve paths or for use with FindGameObject.
		 */
		public static string GetPathRelativeTo( this Transform transform, Transform parent )
		{
			if ( transform == parent )
				return "";

			if ( transform.IsChildOf(parent) )
				return transform.FullPath().Substring( parent.FullPath().Length+1 );

			return transform.FullPath();
		}

        /**
		 * Find the given component on either the-passed in transform or on any of its parents.
		 */
		public static T FindInParents<T>( this Transform transform, bool bIncludeSelf = true ) where T : Component
		{
#if UNITY_5
            var components = transform.GetComponentsInParent<T>(true);
            if ( components.Length < 1 )
                return null;

            if ( bIncludeSelf )
                return components.FirstOrDefault();

            return components.SkipWhile( x => x.transform == transform ).FirstOrDefault();
#else
			Transform current = bIncludeSelf ? transform : transform.parent;
			for ( ; current != null ; current = current.parent ) 
			{
				T comp = current.GetComponent<T>();
				if ( comp )
					return comp;
			}
			
			return null;
#endif

		}
	} // static class TransformEx
} // namespace CodingJar
