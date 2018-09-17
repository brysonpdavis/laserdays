using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateTagsRenderTools
	{
		private const string RenderTypeStr = " Render Type";
		private const string RenderQueueStr = " Render Queue";
		private const string EnableStr = "Use Render Helper";
		private const double ReanalizeTreshold = 0.5;
		private double m_timeSinceReanalyzeCheck = 0;

		[SerializeField]
		private RenderType m_currentRenderType = RenderType.Opaque;
		[SerializeField]
		private int m_renderTypeIndex = -1;
		[SerializeField]
		private RenderQueue m_currentRenderQueue = RenderQueue.Geometry;
		[SerializeField]
		private int m_renderQueueIndex = -1;
		[SerializeField]
		private bool m_enable = false;
		[SerializeField]
		private bool m_isLocked = true;

		[SerializeField]
		private bool m_reanalyzeTags = false;

		public void Draw( UndoParentNode owner , ref List<CustomTagData> currentTags )
		{
			if( m_isLocked )
				return;

			if( m_reanalyzeTags )
			{
				if( ( EditorApplication.timeSinceStartup - m_timeSinceReanalyzeCheck ) > ReanalizeTreshold )
				{
					m_reanalyzeTags = false;
					AnalyzeTags( ref currentTags, true );
				}
			}

			EditorGUI.BeginChangeCheck();
			m_enable = owner.EditorGUILayoutToggle( EnableStr, m_enable );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_enable )
				{
					AnalyzeTags( ref currentTags, true );
				}
			}
			bool guiEnabled = GUI.enabled;
			GUI.enabled = m_enable;
			EditorGUI.BeginChangeCheck();
			m_currentRenderType = (RenderType)owner.EditorGUILayoutEnumPopup( RenderTypeStr, m_currentRenderType );
			if( EditorGUI.EndChangeCheck() )
			{
				currentTags[ m_renderTypeIndex ].TagValue = m_currentRenderType.ToString();
			}
			
			EditorGUI.BeginChangeCheck();
			m_currentRenderQueue = (RenderQueue)owner.EditorGUILayoutEnumPopup( RenderQueueStr, m_currentRenderQueue);
			if( EditorGUI.EndChangeCheck() )
			{
				currentTags[ m_renderQueueIndex ].TagValue = m_currentRenderQueue.ToString();
			}
			GUI.enabled = guiEnabled;
		}

		public bool AnalyzeTags( ref List<CustomTagData> currentTags , bool addItems )
		{
			m_reanalyzeTags = false;
			bool foundRenderType = false;
			bool foundRenderQueue = false;
			int count = currentTags.Count;
			for( int i = 0; i < count; i++ )
			{
				if( TemplateHelperFunctions.StringToReservedTags.ContainsKey( currentTags[ i ].TagName ) )
				{
					switch( TemplateHelperFunctions.StringToReservedTags[ currentTags[ i ].TagName ] )
					{
						case TemplateSpecialTags.RenderType:
						{
							foundRenderType = true;
							m_renderTypeIndex = i;
							m_currentRenderType = ( TemplateHelperFunctions.StringToRenderType.ContainsKey( currentTags[ i ].TagValue ) ) ? TemplateHelperFunctions.StringToRenderType[ currentTags[ i ].TagValue ] : RenderType.Opaque;
						}
						break;
						case TemplateSpecialTags.Queue:
						{
							foundRenderQueue = true;
							m_renderQueueIndex = i;
							m_currentRenderQueue = ( TemplateHelperFunctions.StringToRenderQueue.ContainsKey( currentTags[ i ].TagValue ) ) ? TemplateHelperFunctions.StringToRenderQueue[ currentTags[ i ].TagValue ] : RenderQueue.Geometry;
						}
						break;
					}
				}
			}
			if( addItems && !foundRenderType )
			{
				m_renderTypeIndex = currentTags.Count - 1;
				currentTags.Add( new CustomTagData( "RenderType", "Opaque", m_renderTypeIndex ));
			}

			if( addItems && !foundRenderQueue )
			{
				m_renderQueueIndex = currentTags.Count - 1;
				currentTags.Add( new CustomTagData( "Queue", "Geometry", m_renderQueueIndex ) );
			}
			return !foundRenderType || !foundRenderQueue;
		}

		public void CheckTagUsage( int id )
		{
			if( m_isLocked )
				return;
			if( id == m_renderTypeIndex || id == m_renderQueueIndex )
			{
				m_reanalyzeTags = true;
				m_timeSinceReanalyzeCheck = EditorApplication.timeSinceStartup;
			}
		}

		public RenderType CurrentRenderType { get { return m_currentRenderType; } set { m_currentRenderType = value; } }
		public int RenderTypeIndex { get { return m_renderTypeIndex; } set { m_renderTypeIndex = value; } }
		public RenderQueue CurrentRenderQueue { get { return m_currentRenderQueue; } set { m_currentRenderQueue = value; } }
		public int RenderQueueIndex { get { return m_renderQueueIndex; } set { m_renderQueueIndex = value; } }
		public bool Enable { get { return m_enable; } set { m_enable = value; } }
		public bool IsLocked { get { return m_isLocked; } set { m_isLocked = value; } }
	}

	[Serializable]
	public class TemplateTagsModule : TemplateModuleParent
	{
		private const string CustomTagsStr = " SubShader Tags";
		private const string TagNameStr = "Name";
		private const string TagValueStr = "Value";

		private const string RenderTypeHelperStr = "RenderType";
		private const string RenderQueueHelperStr = "Queue";

		private const float ShaderKeywordButtonLayoutWidth = 15;
		private UndoParentNode m_currentOwner;

		//[SerializeField]
		//private TemplateTagsRenderTools m_renderTools = new TemplateTagsRenderTools();

		[SerializeField]
		private List<CustomTagData> m_availableTags = new List<CustomTagData>();

		private Dictionary<string, CustomTagData> m_availableTagsDict = new Dictionary<string, CustomTagData>();

		public TemplateTagsModule() : base( "SubShader Tags" ) { }

		//void DetectSpecialTags()
		//{
		//	int count = m_availableTags.Count;
		//	for( int i = 0; i < m_availableTags.Count; i++ )
		//	{
		//	}
		//}
		
		public void CopyFrom( TemplateTagsModule other )
		{
			m_availableTags.Clear();
			m_availableTagsDict.Clear();

			int count = other.AvailableTags.Count;
			for( int i = 0; i < count; i++ )
			{
				CustomTagData newData = new CustomTagData( other.AvailableTags[ i ] );
				m_availableTags.Add( newData );
				m_availableTagsDict.Add( newData.TagName, newData );
			}
			//m_renderTools.AnalyzeTags( ref m_availableTags , false );
		}
		
		public void ConfigureFromTemplateData( TemplateTagsModuleData tagsData )
		{
			bool newValidData = tagsData.DataCheck == TemplateDataCheck.Valid;
			if( newValidData && newValidData != m_validData )
			{
				m_availableTags.Clear();
				m_availableTagsDict.Clear();
				int count = tagsData.Tags.Count;
				for( int i = 0; i < count; i++ )
				{
					CustomTagData tagData = new CustomTagData( tagsData.Tags[ i ].Name, tagsData.Tags[ i ].Value, i );
					m_availableTags.Add( tagData );
					m_availableTagsDict.Add( tagsData.Tags[ i ].Name, tagData );
				}
				//m_renderTools.AnalyzeTags( ref m_availableTags, false );
			}
			m_validData = newValidData;
		}

		public override void ShowUnreadableDataMessage( ParentNode owner )
		{
			bool foldout = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags;
			NodeUtils.DrawPropertyGroup( ref foldout, CustomTagsStr, base.ShowUnreadableDataMessage );
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags = foldout;
		}

		public override void Draw( UndoParentNode owner, bool style = true )
		{
			//m_renderTools.IsLocked = style;

			m_currentOwner = owner;
			bool foldout = owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags;
			if( style )
			{
				NodeUtils.DrawPropertyGroup( ref foldout, CustomTagsStr, DrawMainBody, DrawButtons );
			}
			else
			{
				NodeUtils.DrawNestedPropertyGroup( ref foldout, CustomTagsStr, DrawMainBody, DrawButtons );
			}
			owner.ContainerGraph.ParentWindow.InnerWindowVariables.ExpandedCustomTags = foldout;
		}

		void DrawButtons()
		{
			EditorGUILayout.Separator();

			// Add tag
			if( GUILayout.Button( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				m_availableTags.Add( new CustomTagData() );
				EditorGUI.FocusTextInControl( null );
			}

			//Remove tag
			if( GUILayout.Button( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
			{
				if( m_availableTags.Count > 0 )
				{
					m_availableTags.RemoveAt( m_availableTags.Count - 1 );
					EditorGUI.FocusTextInControl( null );
				}
			}
		}

		void DrawMainBody()
		{
			//m_renderTools.Draw( m_currentOwner, ref m_availableTags );
			EditorGUI.BeginChangeCheck();
			{
				EditorGUILayout.Separator();
				int itemCount = m_availableTags.Count;

				if( itemCount == 0 )
				{
					EditorGUILayout.HelpBox( "Your list is Empty!\nUse the plus button to add one.", MessageType.Info );
				}

				int markedToDelete = -1;
				float originalLabelWidth = EditorGUIUtility.labelWidth;
				for( int i = 0; i < itemCount; i++ )
				{
					m_availableTags[ i ].TagFoldout = m_currentOwner.EditorGUILayoutFoldout( m_availableTags[ i ].TagFoldout, string.Format( "[{0}] - {1}", i, m_availableTags[ i ].TagName ) );
					if( m_availableTags[ i ].TagFoldout )
					{
						EditorGUI.indentLevel += 1;
						EditorGUIUtility.labelWidth = 70;
						//Tag Name
						EditorGUI.BeginChangeCheck();
						m_availableTags[ i ].TagName = EditorGUILayout.TextField( TagNameStr, m_availableTags[ i ].TagName );
						if( EditorGUI.EndChangeCheck() )
						{
							m_availableTags[ i ].TagName = UIUtils.RemoveShaderInvalidCharacters( m_availableTags[ i ].TagName );
							//m_renderTools.CheckTagUsage( i );
						}

						//Tag Value
						EditorGUI.BeginChangeCheck();
						m_availableTags[ i ].TagValue = EditorGUILayout.TextField( TagValueStr, m_availableTags[ i ].TagValue );
						if( EditorGUI.EndChangeCheck() )
						{
							m_availableTags[ i ].TagValue = UIUtils.RemoveShaderInvalidCharacters( m_availableTags[ i ].TagValue );
							//m_renderTools.CheckTagUsage( i );
						}

						EditorGUIUtility.labelWidth = originalLabelWidth;

						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Label( " " );
							// Add new port
							if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
							{
								m_availableTags.Insert( i + 1, new CustomTagData() );
								EditorGUI.FocusTextInControl( null );
							}

							//Remove port
							if( m_currentOwner.GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
							{
								markedToDelete = i;
							}
						}
						EditorGUILayout.EndHorizontal();

						EditorGUI.indentLevel -= 1;
					}

				}
				if( markedToDelete > -1 )
				{
					if( m_availableTags.Count > markedToDelete )
					{
						m_availableTags.RemoveAt( markedToDelete );
						EditorGUI.FocusTextInControl( null );
					}
				}
				EditorGUILayout.Separator();
			}
			if( EditorGUI.EndChangeCheck() )
			{
				m_isDirty = true;
			}
		}

		void AddTagFromRead( string data )
		{
			string[] arr = data.Split( IOUtils.VALUE_SEPARATOR );
			if( arr.Length > 1 )
			{
				string name = arr[ 0 ];
				string value = arr[ 1 ];

				if( !m_availableTagsDict.ContainsKey( name ) )
				{
					CustomTagData tagData = new CustomTagData( name, value, m_availableTags.Count - 1 );
					m_availableTags.Add( tagData );
					m_availableTagsDict.Add( name, tagData );
				}
				else
				{
					if( m_availableTagsDict[ name ].TagId > -1 &&
						m_availableTagsDict[ name ].TagId < m_availableTags.Count )
					{
						m_availableTags[ m_availableTagsDict[ name ].TagId ].TagValue = value;
					}
					else
					{
						int count = m_availableTags.Count;
						for( int i = 0; i < count; i++ )
						{
							if( m_availableTags[ i ].TagName.Equals( name ) )
							{
								m_availableTags[ i ].TagValue = value;
							}
						}
					}
				}
			}
		}

		public override void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			bool validDataOnMeta = m_validData;
			if( UIUtils.CurrentShaderVersion() > TemplatesManager.MPShaderVersion )
			{
				validDataOnMeta = Convert.ToBoolean( nodeParams[ index++ ] );
			}

			if( validDataOnMeta )
			{
				int count = Convert.ToInt32( nodeParams[ index++ ] );
				for( int i = 0; i < count; i++ )
				{
					AddTagFromRead( nodeParams[ index++ ] );
				}
			//	m_renderTools.AnalyzeTags( ref m_availableTags, false );
			}
		}

		public override void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_validData );
			if( m_validData )
			{
				int tagsCount = m_availableTags.Count;
				IOUtils.AddFieldValueToString( ref nodeInfo, tagsCount );
				for( int i = 0; i < tagsCount; i++ )
				{
					IOUtils.AddFieldValueToString( ref nodeInfo, m_availableTags[ i ].ToString() );
				}
			}
		}

		public string GenerateTags()
		{
			int tagsCount = m_availableTags.Count;
			if( tagsCount == 0 )
				return string.Empty;

			string result = "Tags { ";

			for( int i = 0; i < tagsCount; i++ )
			{
				if( m_availableTags[ i ].IsValid )
				{
					result += m_availableTags[ i ].GenerateTag();
					if( i < tagsCount - 1 )
					{
						result += " ";
					}
				}
			}

			result += " }";

			return result;
		}

		public override void Destroy()
		{
			m_availableTags.Clear();
			m_availableTags = null;
			m_currentOwner = null;
			m_availableTagsDict.Clear();
			m_availableTagsDict = null;
		//	m_renderTools = null;
		}

		public List<CustomTagData> AvailableTags { get { return m_availableTags; } }

		public bool HasRenderInfo( ref RenderType renderType, ref RenderQueue renderQueue )
		{
			if( !m_validData )
				return false;

			bool foundRenderType = false;
			bool foundRenderQueue = false;
			int count = m_availableTags.Count;
			for( int i = 0; i < count; i++ )
			{
				if( m_availableTags[ i ].TagName.Equals( RenderTypeHelperStr ) )
				{
					if( TemplateHelperFunctions.StringToRenderType.ContainsKey( m_availableTags[ i ].TagValue ) )
					{
						renderType = TemplateHelperFunctions.StringToRenderType[ m_availableTags[ i ].TagValue ];
						foundRenderType = true;
					}
				}
				else if( m_availableTags[ i ].TagName.Equals( RenderQueueHelperStr ) )
				{
					string value = m_availableTags[ i ].TagValue.Split( '+' )[ 0 ].Split( '-' )[0];
					if( TemplateHelperFunctions.StringToRenderQueue.ContainsKey( value ) )
					{
						renderQueue = TemplateHelperFunctions.StringToRenderQueue[ value ];
						foundRenderQueue = true;
					}
				}
			}	
			return foundRenderType && foundRenderQueue;
		}
	}
}
