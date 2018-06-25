using UnityEngine;
using UnityEditor;

using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace CodingJar.MultiScene.Editor
{
	/// <summary>
	/// Structure containing an serialized cross-scene reference.
	/// </summary>
	public struct EditorCrossSceneReference
	{
		public SerializedProperty	fromProperty;
		public Object				toInstance;
			
		public Scene		fromScene;
		public Scene		toScene;

		public List<GenericData>	data;

		public Object		fromObject
		{
			get {	return fromProperty.serializedObject.targetObject;	}
		}

        public override string ToString()
        {
			string fromString = fromObject ? fromObject.ToString() : "(null)";
			string toString = toInstance ? toInstance.ToString() : "(null)";
			
			var fromGameObject = GameObjectEx.EditorGetGameObjectFromComponent(fromObject);
			if ( fromGameObject )
				fromString = string.Format( "{0} ({1})", fromGameObject.GetFullName(), fromObject.GetType() );

			var toGameObject = GameObjectEx.EditorGetGameObjectFromComponent(toInstance);
			if ( toGameObject )
				toString = string.Format( "{0} ({1})", toGameObject.GetFullName(), toInstance.GetType() );

            return string.Format("{0}.{1} => {2}", fromString, fromProperty.propertyPath, toString);
        }

        public override bool Equals( object obj )
        {
            if ( obj is EditorCrossSceneReference )
            {
                var other = (EditorCrossSceneReference)obj;
                return ( other.fromProperty == fromProperty && other.toInstance == toInstance );
            }

            return base.Equals( obj );
        }

        public override int GetHashCode()
        {
            return fromProperty.GetHashCode() + toInstance.GetHashCode();
        }

		/// <summary>
		/// Give us a runtime resolvable version of this cross-scene reference.
		/// This is a best-attempt and is not guaranteed to be resolvable.
		/// </summary>
		/// <returns>The runtime resolvable version.</returns>
		public RuntimeCrossSceneReference	ToSerializable()
		{
			string fromField = ToRuntimeSerializableField( fromProperty );
			return new RuntimeCrossSceneReference( fromObject, fromField, new UniqueObject(toInstance), data );
		}

		/// <summary>
		/// Given a property, let's return a runtime serializeable field string.
		/// </summary>
		/// <param name="property">The property to capture the field from</param>
		/// <returns>The field string which can be parsed at runtime</returns>
		private string ToRuntimeSerializableField( SerializedProperty property )
		{
			const string ARRAY_INDICATOR = "@ArrayIndex[";
			int arrayIndicatorLength = ARRAY_INDICATOR.Length;

			// Give us an easy sentinel value to scan for in case of arrays
			string parseablePropertyPath = property.propertyPath.Replace( ".Array.data[", "."+ARRAY_INDICATOR );
			var splitPaths = parseablePropertyPath.Split( '.' );

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0 ; i < splitPaths.Length ; ++i)
			{
				string pathPiece = splitPaths[i];

				bool bIsArrayIndex = pathPiece.StartsWith(ARRAY_INDICATOR);
				if ( !bIsArrayIndex )
				{
					// Append the . if we're a nested object
					if ( i > 0 )
						sb.Append( '.' );

					sb.Append( pathPiece );
				}
				else
				{
					// It's an array, so we're doing the index portion of @ArrayIndex[index]
					string indexString = pathPiece.Substring( arrayIndicatorLength, pathPiece.Length - arrayIndicatorLength - 1 );

					int arrayIndex = 0;
					if ( int.TryParse(indexString, out arrayIndex) )
					{
						// Arrays are of the form fieldName,arrayIndex
						sb.Append( ',' );
						sb.Append( arrayIndex );
					}
					else
					{
						AmsDebug.LogError( null, "Could not parse array index for property path {0}", property.propertyPath );
					}
				}
			}

			return sb.ToString();
		}
	} // struct
}
