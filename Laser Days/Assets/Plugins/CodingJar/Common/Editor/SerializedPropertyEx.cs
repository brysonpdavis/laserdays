using UnityEngine;
using UnityEditor;

namespace CodingJar
{
	/// <summary>
	/// The point of SerializedPropertyEx is to be able to reconstruct a SerializedProperty at runtime for comparison purposes.
	/// Note: Since SerializedProperty only exists in the Editor, we can only use this in the Editor.
	/// </summary>
	[System.Serializable]
	public class SerializedPropertyEx
	{
		public SerializedPropertyType	propertyType;
		public string 					propertyPath;
		public string					value;
			
		public SerializedPropertyEx( SerializedProperty property )
		{
			propertyType = property.propertyType;
			propertyPath = property.propertyPath;
				
			value = GetPropertyStringValue( property );
		}
			
		public bool Equals( SerializedProperty otherProperty )
		{
			return Equals( new SerializedPropertyEx(otherProperty) );
		}
			
		public bool Equals( SerializedPropertyEx otherProperty )
		{
			return propertyPath.Equals( otherProperty.propertyPath ) &&
				propertyType.Equals( otherProperty.propertyType ) &&
					value.Equals( otherProperty.value );
		}
			
		public static bool Supports( SerializedProperty property )
		{
			switch (property.propertyType)
			{
			case SerializedPropertyType.AnimationCurve:
			case SerializedPropertyType.ArraySize:
			case SerializedPropertyType.Character:
			case SerializedPropertyType.Generic:
			case SerializedPropertyType.Gradient:
				return false;
			}
				
			return true;
		}
			
		/**
		 * Copy the value from one property type to another, assuming that the types are equal
		 */
		public static string GetPropertyStringValue( SerializedProperty property )
		{
			switch ( property.propertyType )
			{
			case SerializedPropertyType.AnimationCurve:
				goto default;
					
			case SerializedPropertyType.ArraySize:
				return property.arraySize.ToString();
					
			case SerializedPropertyType.Boolean:
				return property.boolValue.ToString();
					
			case SerializedPropertyType.Bounds:
				return property.boundsValue.ToString();
					
			case SerializedPropertyType.Character:
				goto default;
					
			case SerializedPropertyType.Color:
				return property.colorValue.ToString();
		
			case SerializedPropertyType.Enum:
				return property.enumNames[property.enumValueIndex];
		
			case SerializedPropertyType.Float:
				return property.floatValue.ToString();
		
			case SerializedPropertyType.Generic:
				goto default;
		
			case SerializedPropertyType.Gradient:
				goto default;
					
			case SerializedPropertyType.Integer:
			case SerializedPropertyType.LayerMask:
				return property.intValue.ToString();
					
			case SerializedPropertyType.ObjectReference:
				return property.objectReferenceValue ? property.objectReferenceValue.name : null;
					
			case SerializedPropertyType.Rect:
				return property.rectValue.ToString();
					
			case SerializedPropertyType.String:
				return property.stringValue;
					
			case SerializedPropertyType.Vector2:
				return property.vector2Value.ToString();
					
			case SerializedPropertyType.Vector3:
				return property.vector3Value.ToString();
					
			default:
				Debug.LogError( "GetPropertyStringValue type not supported: " + property.propertyType );
				return null;
			}
		}
		
	}
}
