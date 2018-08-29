// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	/*ase_pass_options
	DefineOnConnected:portId:definevalue	
	DefineOnUnconnected:portId:definevalue
	Options:name:defaultOption:opt0:opt1:opt2
	SetVisible:PortId:OptionName:OptionValue
	*/

	public enum TemplateOptionType
	{
		DefineOnConnected,
		DefineOnUnconnected,
		Options,
		SetVisible
	}

	[Serializable]
	public class TemplateOptionsData
	{
		public TemplateOptionType OptionType;
		public string[] Values;
		public TemplateOptionsData( string[] values )
		{
			try
			{
				OptionType = (TemplateOptionType)Enum.Parse( typeof( TemplateOptionType ), values[ 0 ] );
				Values = values;
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		public void Destroy()
		{
			Values = null;
		}
	}

	[Serializable]
	public class TemplateOptionsContainer
	{
		[SerializeField]
		public List<TemplateOptionsData> Options = new List<TemplateOptionsData>();
		public void Destroy()
		{
			int count = Options.Count;
			for( int i = 0; i < count; i++ )
			{
				Options[ i ].Destroy();
			}
			Options.Clear();
			Options = null;
		}
	}

	[Serializable]
	public class TemplateOptionsHelperItem
	{
		[SerializeField]
		private string m_name;

		[SerializeField]
		private string[] m_options;

		[SerializeField]
		private int m_nodeId;

		[SerializeField]
		private int m_portId;

		[SerializeField]
		private int m_currentOption;
	}
}
