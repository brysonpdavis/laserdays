using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace CodingJar.MultiScene
{
    public static class AmsCrossSceneReferenceResolver
    {
		/// <summary>
		/// The method signature for a custom resolver.  A custom resolver can resolve complex cases for cross-scene references.
		/// </summary>
		/// <param name="xRef">The cross-scene reference data</param>
		/// <returns>true iff this resolver has handled the cross-scene reference (and thus no further resolving is necessary)</returns>
		public delegate bool ResolveCrossSceneReferenceDelegate( RuntimeCrossSceneReference xRef );

		private static List<ResolveCrossSceneReferenceDelegate>		_resolvers = new List<ResolveCrossSceneReferenceDelegate>();

		static AmsCrossSceneReferenceResolver()
		{
			_resolvers.Clear();
			_resolvers.Add( DefaultResolve );
		}

		/// <summary>
		/// Add a custom resolver to the resolution process.
		/// </summary>
		/// <param name="customResolver">The method that can resolve a custom case.</param>
		public static void AddCustomResolver( ResolveCrossSceneReferenceDelegate customResolver )
		{
			_resolvers.Add( customResolver );
		}

		/// <summary>
		/// Perform a resolve on a cross-scene reference.
		/// This functions throws an exception if it fails.
		/// </summary>
		public static void Resolve( RuntimeCrossSceneReference xRef )
		{
			var fromObject = xRef.fromObject;
			if ( !fromObject )
				throw new ResolveException( string.Format( "Cross-Scene Ref: {0}. fromObject is null.", xRef ) );

			Object toObject = xRef.toObject;
			if ( !toObject )
				throw new ResolveException( string.Format( "Cross-Scene Ref: {0}. Could not Resolve toObject {1}", xRef, toObject ) );

			// Try all of the resolvers (oldest last)
			for ( int i = _resolvers.Count - 1; i >= 0; --i )
			{
				if ( _resolvers[i]( xRef ) )
					break;
			}
		}

		/// <summary>
		/// The default resolver will set the source's field to point to the destination
		/// </summary>
		private static bool DefaultResolve( RuntimeCrossSceneReference xRef )
		{
			ResolveToField( xRef.fromObject, xRef.toObject, xRef.sourceField, xRef );
			return true;
		}

		/// <summary>
		/// Resolve a cross-scene reference piecewise.  This does the heavy lifting of figuring out how to parse the path.
		/// </summary>
		/// <param name="fromObject">The object that is the source of the cross-scene reference</param>
		/// <param name="toObject">The object that the cross-scene reference is referring to</param>
		/// <param name="fromFieldPath">The path of the field that fromObject uses to point to</param>
		/// <param name="debugThis">Debug information about which cross-scene reference this is coming from</param>
		private static void ResolveToField( System.Object fromObject, Object toObject, string fromFieldPath, RuntimeCrossSceneReference debugThis )
		{
			// Sub-object path is indicated by a dot
			string[] splitPaths = fromFieldPath.Split('.');

			// Since the property is of the form: field1.field2.arrayName,arrayIndex.final_field or simply final_field, we need to chase
			// the property down the rabbit hole starting with the base fromObject and going all the way to final_field.
			for (int i = 0 ; i < splitPaths.Length - 1 ; ++i)
			{
				try
				{
					fromObject = GetObjectFromField( fromObject, splitPaths[i] );
					if ( fromObject == null )
					{
						throw new ResolveException( string.Format("Cross-Scene Ref: {0}. Could not follow path {1} because {2} was null", debugThis, fromFieldPath, splitPaths[i]) );
					}
#if !NETFX_CORE
					else if ( !fromObject.GetType().IsClass )
#else
					else if ( !fromObject.GetType().GetTypeInfo().IsClass )
#endif
					{
						throw new ResolveException( string.Format("Cross-Scene Ref: {0}. Could not follow path {1} because {2} was not a class (probably a struct). This is unsupported.", debugThis, fromFieldPath, splitPaths[i]) );
					}
				}
				catch ( System.Exception ex )
				{
					throw new ResolveException( string.Format("Cross-Scene Ref: {0}. {1}", debugThis, ex.Message) );
				}
			}

			// Finally, get the final field.
			FieldInfo field;
			PropertyInfo property;
			int arrayIndex;
			string fieldName = splitPaths[ splitPaths.Length-1 ];

			if ( !GetFieldFromObject(fromObject, fieldName, out field, out property, out arrayIndex ) )
				throw new ResolveException( string.Format("Cross-Scene Ref: {0}. Could not parse piece of path {1} from {2}", debugThis, fieldName, fromFieldPath) );

			// Now we can finally assign it!
			AssignField( fromObject, toObject, field, property, arrayIndex );
		}

		/// <summary>
		/// Try to find the field that belongs to fromObject.
		/// </summary>
		/// <param name="fromObject">The source object that has a field named fromField</param>
		/// <param name="fromField">The field name to check for the reference. Can be of the form fieldName,index</param>
		/// <param name="field">The field that represents fromField</param>
		/// <param name="arrayIndex">The array index represented in fromField, or -1 if not an array</param>
		/// <returns>The object referred to by fromObject.fromField</returns>
		private static bool	GetFieldFromObject( System.Object fromObject, string fromField, out FieldInfo field, out PropertyInfo property, out int arrayIndex )
		{
			arrayIndex = -1;
			field = null;
			property = null;

			string[] parseField = fromField.Split(',');
			string fieldName = parseField[0];
			
			// Check if it's an array
			if ( parseField.Length > 1 )
			{
				if ( !int.TryParse(parseField[1], out arrayIndex) )
					return false;
			}

			// Find the field.  Need to go through all base classes.
			System.Type objectType = fromObject.GetType();
			while ( objectType != null && (field == null && property == null) )
			{
#if !NETFX_CORE
				field = objectType.GetField( fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
				if ( field == null && fieldName.StartsWith( "m_" ) )
				{
					string propertyName = char.ToLower( fieldName[2] ) + fieldName.Substring(3);
					property = objectType.GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
				}

				objectType = objectType.BaseType;
#else
				var typeInfo = objectType.GetTypeInfo();

				field = typeInfo.GetDeclaredField( fieldName );
				if ( field == null && fieldName.StartsWith("m_") )
					property = typeInfo.GetDeclaredProperty( fieldName.Substring(2) );

				objectType = typeInfo.BaseType;
#endif
			}

			return (field != null) || (property != null);
		}

		/// <summary>
		/// Try to find the object that fromObject.fromField refers to
		/// </summary>
		/// <param name="fromObject">The source object that has a field named fromField</param>
		/// <param name="fromField">The field name to check for the reference</param>
		/// <returns>The object referred to by fromObject.fromField</returns>
		private static System.Object	GetObjectFromField( System.Object fromObject, string fromField )
		{
			int arrayIndex;
			FieldInfo field;
			PropertyInfo property;

			if ( !GetFieldFromObject( fromObject, fromField, out field, out property, out arrayIndex ) )
				throw new ResolveException( string.Format( "Could not find Field {0}", fromField ) );

			string fieldName = field != null ? field.Name : property.Name;
			System.Type fieldType = field != null ? field.FieldType : property.PropertyType;

			bool isArray = arrayIndex >= 0;
			if ( isArray )
			{
				var list = (field != null ? field.GetValue( fromObject ) : property.GetValue( fromObject, null )) as System.Collections.IList;
				if ( list == null )
					throw new ResolveException( string.Format( "Expected collection of elements for property {0} but field type is {1}", fieldName, fieldType.Name ) );
				else if ( list.Count <= arrayIndex )
					throw new ResolveException( string.Format( "Expected collection of at least {0} elements from property {1}", arrayIndex+1, fieldName ) );

				// Found it!  Here's the entry.
				return list[arrayIndex];
			}

			// TODO: Move the IsClass check into here, because it may not be wrong on a property

			return (field != null) ? field.GetValue( fromObject ) : property.GetValue( fromObject, null );
		}

		/// <summary>
		/// Assigns a specific field fromObject.field[arrayIndex] -> toObject
		/// </summary>
		/// <param name="fromObject">The object to assign from</param>
		/// <param name="toObject">The target object</param>
		/// <param name="field">The field that should be assigned</param>
		/// <param name="arrayIndex">The array index of that field (or negative if it's not an array)</param>
		private static void AssignField( System.Object fromObject, Object toObject, FieldInfo field, PropertyInfo property, int arrayIndex )
		{
			string fieldName = field != null ? field.Name : property.Name;
			System.Type fieldType = field != null ? field.FieldType : property.PropertyType;

			bool isArray = arrayIndex >= 0;
			if ( isArray )
			{
				var listObj = (field != null) ? field.GetValue( fromObject ) : property.GetValue( fromObject, null );
				var list = listObj as System.Collections.IList;
				if ( list == null )
					throw new ResolveException( string.Format( "Expected collection of elements for property {0} but field type is {1}", fieldName, fieldType.Name ) );

				// If it's for a specific index that doesn't yet exist, add it.
				if ( list.Count <= arrayIndex )
				{
					list.Insert( arrayIndex, toObject );
					return;
				}

				// Successful assign!
				try
				{
					list[arrayIndex] = toObject;
				}
				catch ( System.Exception ex )
				{
					// Wrap that exception so we know we must fix it
					throw new ResolveException( string.Format( "Cross-Scene Reference Resolve FAIL on {0}.'{1}'[{2}]. Manual fix required. Exception said '{3}'", fromObject, fieldName, arrayIndex, ex.ToString()) );
				}

				return;
			}
#if !NETFX_CORE
			else if ( toObject && !fieldType.IsAssignableFrom( toObject.GetType() ) )
#else
			else if ( toObject && fieldType.GetTypeInfo().IsAssignableFrom( toObject.GetType().GetTypeInfo() ) )
#endif
			{
				throw new ResolveException( string.Format( "Field {0} of type {1} is not compatible with {2} of type {3}", fieldName, fieldType, toObject, toObject.GetType().Name ) );
			}

			// Successful assign!
			if ( field != null )
				field.SetValue( fromObject, toObject );
			else if ( property != null )
				property.SetValue( fromObject, toObject, null );
		}

#if UNITY_EDITOR
		public static void EditorOnly_ResolveToField( System.Object fromObject, Object toObject, string fromFieldPath, RuntimeCrossSceneReference debugThis )
		{
			ResolveToField( fromObject, toObject, fromFieldPath, debugThis );
		}
#endif
	} // struct 
} // namespace 