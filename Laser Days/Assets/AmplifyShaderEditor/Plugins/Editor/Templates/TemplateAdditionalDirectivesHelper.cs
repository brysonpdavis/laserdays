// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace AmplifyShaderEditor
{
	public enum AdditionalLineType
	{
		Include,
		Define,
		Pragma,
		Custom
	}


	[Serializable]
	public class AdditionalDirectiveContainerSaveItem 
	{
		public AdditionalLineType LineType;
		public string LineValue;
		public AdditionalDirectiveContainerSaveItem( AdditionalLineType lineType, string lineValue )
		{
			LineType = lineType;
			LineValue = lineValue;
		}
	}
	
	[Serializable]
	public class AdditionalDirectiveContainer : ScriptableObject
	{
		private const string IncludeFormat = "#include \"{0}\"";
		private const string PragmaFormat = "#pragma {0}";
		private const string DefineFormat = "#define {0}";

		public AdditionalLineType LineType;
		public string LineValue;
		public AdditionalDirectiveContainer()
		{
			LineType = AdditionalLineType.Include;
			LineValue = string.Empty;
		}

		public AdditionalDirectiveContainer( AdditionalLineType lineType, string lineValue )
		{
			LineType = lineType;
			LineValue = lineValue;
		}

		public string FormattedValue
		{
			get
			{
				switch( LineType )
				{
					case AdditionalLineType.Include:
					return string.Format( IncludeFormat, LineValue );
					case AdditionalLineType.Define:
					return string.Format( DefineFormat, LineValue );
					case AdditionalLineType.Pragma:
					return string.Format( PragmaFormat, LineValue );
				}
				return LineValue;
			}
		}
	}

	public enum ReordableAction
	{
		None,
		Add,
		Remove
	}

	[Serializable]
	public sealed class TemplateAdditionalDirectivesHelper : TemplateModuleParent
	{
		private string NativeFoldoutStr = "Native";

		[SerializeField]
		private List<AdditionalDirectiveContainer> m_additionalDirectives = new List<AdditionalDirectiveContainer>();
		
		[SerializeField]
		private List<AdditionalDirectiveContainer> m_shaderFunctionDirectives = new List<AdditionalDirectiveContainer>();

		[SerializeField]
		private List<string> m_nativeDirectives = new List<string>();

		[SerializeField]
		private bool m_nativeDirectivesFoldout = false;

		//ONLY USED BY SHADER FUNCTIONS
		// Since AdditionalDirectiveContainer must be a ScriptableObject because of serialization shenanigans it will not serialize the info correctly into the shader function when saving it into a file ( it only saves the id )
		// For it to properly work, each AdditionalDirectiveContainer should be added to the SF asset, but that would make it to have children ( which are seen on the project inspector )
		// Must revisit this later on and come up with a proper solution
		[SerializeField]
		private List<AdditionalDirectiveContainerSaveItem> m_directivesSaveItems = new List<AdditionalDirectiveContainerSaveItem>();


		private ReordableAction m_actionType = ReordableAction.None;
		private int m_actionIndex = 0;
		private ReorderableList m_reordableList = null;
		private GUIStyle m_propertyAdjustment;


		public TemplateAdditionalDirectivesHelper( string moduleName ) : base( moduleName ) { }
		
		//public void AddShaderFunctionItem( AdditionalLineType type, string item )
		//{
		//	UpdateShaderFunctionDictionary();
		//	string id = type + item;
		//	if( !m_shaderFunctionDictionary.ContainsKey( id ) )
		//	{
		//		AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
		//		newItem.LineType = type;
		//		newItem.LineValue = item;
		//		newItem.hideFlags = HideFlags.HideAndDontSave;
		//		m_shaderFunctionDirectives.Add( newItem );
		//		m_shaderFunctionDictionary.Add( id, newItem );
		//	}
		//}

		public void AddShaderFunctionItems( List<AdditionalDirectiveContainer> functionList )
		{
			if( functionList.Count > 0 )
				m_shaderFunctionDirectives.AddRange( functionList );				
		}

		public void RemoveShaderFunctionItems( List<AdditionalDirectiveContainer> functionList )
		{
			for( int i = 0; i < functionList.Count; i++ )
			{
				m_shaderFunctionDirectives.Remove( functionList[ i ] );
			}
		}

		//public void RemoveShaderFunctionItem( AdditionalLineType type, string item )
		//{
		//	m_shaderFunctionDirectives.RemoveAll( x => x.LineType == type && x.LineValue.Equals( item ) );
		//}

		public void AddItems( AdditionalLineType type, List<string> items )
		{
			int count = items.Count;
			for( int i = 0; i < count; i++ )
			{
				AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
				newItem.LineType = type;
				newItem.LineValue = items[ i ];
				newItem.hideFlags = HideFlags.HideAndDontSave;
				m_additionalDirectives.Add( newItem );
			}
		}

		public void FillNativeItems( List<string> nativeItems )
		{
			m_nativeDirectives.Clear();
			m_nativeDirectives.AddRange( nativeItems );
		}

		void DrawNativeItems()
		{
			EditorGUILayout.Separator();
			EditorGUI.indentLevel++;
			int count = m_nativeDirectives.Count;
			for( int i = 0; i < count; i++ )
			{
				EditorGUILayout.LabelField( m_nativeDirectives[ i ] );
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add keyword
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( Constants.PlusMinusButtonLayoutWidth ) ) )
			{
				AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
				newItem.hideFlags = HideFlags.HideAndDontSave;
				m_additionalDirectives.Add( newItem );
				EditorGUI.FocusTextInControl( null );
				m_isDirty = true;
			}

			//Remove keyword
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( Constants.PlusMinusButtonLayoutWidth ) ) )
			{
				if( m_additionalDirectives.Count > 0 )
				{
					AdditionalDirectiveContainer itemToDelete = m_additionalDirectives[ m_additionalDirectives.Count - 1 ];
					m_additionalDirectives.RemoveAt( m_additionalDirectives.Count - 1 );
					ScriptableObject.DestroyImmediate( itemToDelete );
					EditorGUI.FocusTextInControl( null );
				}
				m_isDirty = true;
			}
		}

		public override void Draw( UndoParentNode owner, bool style = true )
		{
			if( m_reordableList == null )
			{
				m_reordableList = new ReorderableList( m_additionalDirectives, typeof( AdditionalDirectiveContainer ), true, false, false, false )
				{
					headerHeight = 0,
					footerHeight = 0,
					showDefaultBackground = false,
					drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
					{
						if( m_additionalDirectives[ index ] != null )
						{
							if( style )
							{
								rect.xMin -= 10;
							}
							else
							{
								rect.xMin -= 1;
							}

							float popUpWidth = style?75:65f;

							Rect popupPos = new Rect( rect.x, rect.y, popUpWidth, EditorGUIUtility.singleLineHeight );
							Rect buttonPlusPos = new Rect( rect.x + rect.width - 2* Constants.PlusMinusButtonLayoutWidth, rect.y - 2, Constants.PlusMinusButtonLayoutWidth, Constants.PlusMinusButtonLayoutWidth );
							Rect buttonMinusPos = new Rect( rect.x + rect.width - Constants.PlusMinusButtonLayoutWidth, rect.y - 2, Constants.PlusMinusButtonLayoutWidth, Constants.PlusMinusButtonLayoutWidth );
							Rect labelPos = new Rect( rect.x + popupPos.width, rect.y, rect.width - popupPos.width - buttonPlusPos.width - buttonMinusPos.width, rect.height );
							labelPos.height = EditorGUIUtility.singleLineHeight;
							m_additionalDirectives[ index ].LineType = (AdditionalLineType)owner.EditorGUIEnumPopup( popupPos, m_additionalDirectives[ index ].LineType );
							m_additionalDirectives[ index ].LineValue = owner.EditorGUITextField( labelPos, string.Empty, m_additionalDirectives[ index ].LineValue );
							if( GUI.Button( buttonPlusPos, string.Empty, UIUtils.PlusStyle ) )
							{
								m_actionType = ReordableAction.Add;
								m_actionIndex = index;
							}

							if( GUI.Button( buttonMinusPos, string.Empty, UIUtils.MinusStyle ) )
							{
								m_actionType = ReordableAction.Remove;
								m_actionIndex = index;
							}
						}
					}
				};
			}

			if( m_actionType != ReordableAction.None )
			{
				switch( m_actionType )
				{
					case ReordableAction.Add:
					AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
					newItem.hideFlags = HideFlags.HideAndDontSave;
					m_additionalDirectives.Insert( m_actionIndex + 1, newItem );
					break;
					case ReordableAction.Remove:
					AdditionalDirectiveContainer itemToDelete = m_additionalDirectives[ m_actionIndex ];
					m_additionalDirectives.RemoveAt( m_actionIndex );
					ScriptableObject.DestroyImmediate( itemToDelete );
					break;
				}
				m_isDirty = true;
				m_actionType = ReordableAction.None;
				EditorGUI.FocusTextInControl( null );
			}
			bool foldoutValue = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedAdditionalDirectives;
			if( style )
			{
				NodeUtils.DrawPropertyGroup( ref foldoutValue, m_moduleName, DrawReordableList, DrawButtons );
			}
			else
			{
				NodeUtils.DrawNestedPropertyGroup( ref foldoutValue, m_moduleName, DrawReordableList, DrawButtons );
			}
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedAdditionalDirectives = foldoutValue;
		}

		void DrawReordableList()
		{
			if( m_reordableList != null )
			{
				if( m_propertyAdjustment == null )
				{
					m_propertyAdjustment = new GUIStyle();
					m_propertyAdjustment.padding.left = 17;
				}
				//EditorGUILayout.BeginVertical( m_propertyAdjustment );
				EditorGUILayout.Space();
				if( m_nativeDirectives.Count > 0 )
				{
					NodeUtils.DrawNestedPropertyGroup( ref m_nativeDirectivesFoldout, NativeFoldoutStr, DrawNativeItems, 4 );
				}
				if( m_additionalDirectives.Count == 0 )
				{
					EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
				}
				else
				{
					m_reordableList.DoLayoutList();
				}
				EditorGUILayout.Space();
				//EditorGUILayout.EndVertical();
			}
		}

		public void AddAllToDataCollector( ref MasterNodeDataCollector dataCollector, TemplateIncludePragmaContainter nativesContainer )
		{
			AddToDataCollector( ref dataCollector, nativesContainer, false );
			AddToDataCollector( ref dataCollector, nativesContainer, true );
		}

		public void AddAllToDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			AddToDataCollector( ref dataCollector, false );
			AddToDataCollector( ref dataCollector, true );
		}

		void AddToDataCollector( ref MasterNodeDataCollector dataCollector, TemplateIncludePragmaContainter nativesContainer, bool fromSF )
		{
			List<AdditionalDirectiveContainer> list = fromSF ? m_shaderFunctionDirectives : m_additionalDirectives;
			int count = list.Count;
			for( int i = 0; i < count; i++ )
			{
				switch( list[ i ].LineType )
				{
					case AdditionalLineType.Include:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) &&
						  !nativesContainer.HasInclude( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Define:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) &&
						  !nativesContainer.HasDefine( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Pragma:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) &&
						  !nativesContainer.HasPragma( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					default:
					case AdditionalLineType.Custom:
					dataCollector.AddToMisc( list[ i ].LineValue );
					break;
				}
			}
		}

		void AddToDataCollector( ref MasterNodeDataCollector dataCollector, bool fromSF )
		{
			List<AdditionalDirectiveContainer> list = fromSF ? m_shaderFunctionDirectives : m_additionalDirectives;
			int count = list.Count;
			for( int i = 0; i < count; i++ )
			{
				switch( list[ i ].LineType )
				{
					case AdditionalLineType.Include:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Define:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					case AdditionalLineType.Pragma:
					{
						if( !string.IsNullOrEmpty( list[ i ].LineValue ) )
						{
							dataCollector.AddToMisc( list[ i ].FormattedValue );
						}
					}
					break;
					default:
					case AdditionalLineType.Custom:
					dataCollector.AddToMisc( list[ i ].LineValue );
					break;
				}
			}
		}


		public override void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			try
			{
				int count = Convert.ToInt32( nodeParams[ index++ ] );
				for( int i = 0; i < count; i++ )
				{
					AdditionalLineType lineType = (AdditionalLineType)Enum.Parse( typeof( AdditionalLineType ), nodeParams[ index++ ] );
					string lineValue = nodeParams[ index++ ];
					AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
					newItem.hideFlags = HideFlags.HideAndDontSave;
					newItem.LineType = lineType;
					newItem.LineValue = lineValue;
					m_additionalDirectives.Add( newItem );
				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}
		
		public override void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDirectives.Count );
			for( int i = 0; i < m_additionalDirectives.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDirectives[ i ].LineType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_additionalDirectives[ i ].LineValue );	
			}
		}

		// read comment on m_directivesSaveItems declaration
		public void UpdateSaveItemsFromDirectives()
		{
			bool foundNull = false;
			m_directivesSaveItems.Clear();
			for( int i = 0; i < m_additionalDirectives.Count; i++ )
			{
				if( m_additionalDirectives[ i ] != null )
				{
					m_directivesSaveItems.Add( new AdditionalDirectiveContainerSaveItem( m_additionalDirectives[ i ].LineType, m_additionalDirectives[ i ].LineValue ) );
				}
				else
				{
					foundNull = true;
				}
			}

			if( foundNull )
			{
				m_additionalDirectives.RemoveAll( item => item == null);
			}
		}

		// read comment on m_directivesSaveItems declaration
		public void UpdateDirectivesFromSaveItems()
		{
			if( m_directivesSaveItems.Count > 0 )
			{
				for( int i = 0; i < m_additionalDirectives.Count; i++ )
				{
					if( m_additionalDirectives[ i ] != null )
						ScriptableObject.DestroyImmediate( m_additionalDirectives[ i ] );
				}

				m_additionalDirectives.Clear();

				for( int i = 0; i < m_directivesSaveItems.Count; i++ )
				{
					AdditionalDirectiveContainer newItem = ScriptableObject.CreateInstance<AdditionalDirectiveContainer>();
					newItem.hideFlags = HideFlags.HideAndDontSave;
					newItem.LineType = m_directivesSaveItems[ i ].LineType;
					newItem.LineValue = m_directivesSaveItems[ i ].LineValue;
					m_additionalDirectives.Add( newItem );
				}
				m_directivesSaveItems.Clear();
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_nativeDirectives.Clear();
			m_nativeDirectives = null;

			m_additionalDirectives.Clear();
			m_additionalDirectives = null;

			m_propertyAdjustment = null;
			m_reordableList = null;
		}

		public List<AdditionalDirectiveContainer> DirectivesList { get { return m_additionalDirectives; } }
		public bool IsValid { get { return m_validData; } set { m_validData = value; } }
	}
}
