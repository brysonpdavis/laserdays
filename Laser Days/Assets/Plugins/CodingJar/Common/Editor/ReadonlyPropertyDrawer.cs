using UnityEngine;
using UnityEditor;

namespace CodingJar.Editor
{
    /// <summary>
    /// Draws any property marked up with CodingJar.Readonly
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    class ReadonlyPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Get the height of all of the children
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <summary>
        /// Draw the property as read-only
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

	[CustomPropertyDrawer(typeof(BeginReadonlyAttribute))]
	class BeginReadonlyPropertyDrawer : DecoratorDrawer
	{
		public override float GetHeight()
		{
			return 0.0f;
		}

		public override void OnGUI( Rect position )
		{
			GUI.enabled = false;
		}
	}

	[CustomPropertyDrawer(typeof(EndReadonlyAttribute))]
	class EndReadonlyPropertyDrawer : DecoratorDrawer
	{
		public override float GetHeight()
		{
			return 0.0f;
		}

		public override void OnGUI( Rect position )
		{
			GUI.enabled = true;
		}
	}
}
