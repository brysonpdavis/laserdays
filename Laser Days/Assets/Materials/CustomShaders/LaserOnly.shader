// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/LaserOnly"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_EdgeColor("Edge Color", Color) = (0,0,0,0)
		_EdgeThickness("Edge Thickness", Range( 0 , 0.5)) = 0.01
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _TransitionState;
		uniform float _EdgeThickness;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;
		uniform float4 _EdgeColor;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float4 tex2DNode2 = tex2D( _DissolveMap, uv_DissolveMap );
			o.Emission = ( step( ( (-1.0 + (_TransitionState - 0.0) * (_EdgeThickness - -1.0) / (1.0 - 0.0)) + tex2DNode2.r ) , _EdgeThickness ) * _EdgeColor ).rgb;
			o.Alpha = 1;
			clip( step( 0.0 , ( (-1.0 + (_TransitionState - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + tex2DNode2.r ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
137;138;1343;886;2910.153;680.582;2.504947;True;False
Node;AmplifyShaderEditor.CommentaryNode;128;-1792.163,472.5112;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;4;4;73;111;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-1746.475,378.242;Float;False;Property;_EdgeThickness;Edge Thickness;6;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1747.602,532.1614;Float;False;Property;_TransitionState;Transition State;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1395.268,109.7809;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;129;-1095.613,23.24464;Float;False;814.5701;432.0292;Burn Effect - Emission;4;150;151;154;152;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;2;-1748.714,673.4712;Float;True;Property;_DissolveMap;Dissolve Map;3;0;Create;True;0;0;False;0;None;3eef1ebc0df734b078e629b5c879aae3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1110.371,132.5078;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-1417.627,533.6931;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1128.677,532.7899;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;152;-612.2945,239.0584;Float;False;Property;_EdgeColor;Edge Color;5;0;Create;True;0;0;False;0;0,0,0,0;0.843776,1,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;154;-836.5375,132.4753;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-135.7624,-27.91076;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-396.0391,171.6789;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;78;-134.0921,-247.5491;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;132;144.1929,26.72195;Float;False;765.1592;493.9802;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.StepOpNode;143;-818.6287,528.7582;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432.4345,117.531;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/LaserOnly;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;148;0;4;0
WireConnection;148;4;156;0
WireConnection;150;0;148;0
WireConnection;150;1;2;1
WireConnection;111;0;4;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;154;0;150;0
WireConnection;154;1;156;0
WireConnection;151;0;154;0
WireConnection;151;1;152;0
WireConnection;143;1;73;0
WireConnection;0;0;78;0
WireConnection;0;1;157;0
WireConnection;0;2;151;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=3C90FC6518B47AF8E538BA82AF3F160B749987DF