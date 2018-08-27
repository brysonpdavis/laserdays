// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/RealOnly"
{
	Properties
	{
		_TransitionState("Transition State", Range( 0 , 1)) = 0.7388377
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_EdgeColor("Edge Color", Color) = (0,0,0,0)
		_EdgeThickness("Edge Thickness", Range( 0 , 0.5)) = 0.01
		_HoldColor("Hold Color", Color) = (0,0,0,0)
		_HoverColor("Hover Color", Color) = (0,0,0,0)
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
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
		uniform float4 _HoverColor;
		uniform half _onHover;
		uniform float4 _HoldColor;
		uniform half _onHold;
		uniform float _EdgeThickness;
		uniform float _TransitionState;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;
		uniform float4 _EdgeColor;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 lerpResult165 = lerp( tex2D( _Albedo, uv_Albedo ) , _HoverColor , ( _onHover * _HoverColor.a ));
			float4 lerpResult184 = lerp( lerpResult165 , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 Albedo187 = lerpResult184;
			o.Albedo = Albedo187.rgb;
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float4 tex2DNode2 = tex2D( _DissolveMap, uv_DissolveMap );
			o.Emission = ( step( _EdgeThickness , ( (-1.0 + (_TransitionState - 0.0) * (_EdgeThickness - -1.0) / (1.0 - 0.0)) + tex2DNode2.r ) ) * _EdgeColor ).rgb;
			o.Alpha = 1;
			clip( step( ( (-1.0 + (_TransitionState - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + tex2DNode2.r ) , 0.0 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
13;45;1680;952;2320.25;973.3263;2.025761;True;False
Node;AmplifyShaderEditor.CommentaryNode;128;-1792.163,472.5112;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;4;4;73;111;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;159;-2140.311,-514.7857;Float;False;Property;_HoverColor;Hover Color;7;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-1747.602,532.1614;Float;False;Property;_TransitionState;Transition State;0;0;Create;True;0;0;False;0;0.7388377;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-1746.475,378.242;Float;False;Property;_EdgeThickness;Edge Thickness;5;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-2142.046,-345.2109;Half;False;Property;_onHover;onHover;10;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-2255.318,-725.7436;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-1679.608,-372.7224;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1748.714,673.4712;Float;True;Property;_DissolveMap;Dissolve Map;3;0;Create;True;0;0;False;0;e28dc97a9541e3642a48c0e3886688c5;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;181;-1414.929,-386.9798;Float;False;Property;_HoldColor;Hold Color;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;183;-1491.238,-208.1753;Half;False;Property;_onHold;onHold;9;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1395.268,109.7809;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;165;-1415.963,-518.5706;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1110.371,132.5078;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-1417.627,533.6931;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1201.682,-267.2216;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;152;-612.2945,239.0584;Float;False;Property;_EdgeColor;Edge Color;4;0;Create;True;0;0;False;0;0,0,0,0;0.843776,1,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;154;-836.5375,132.4753;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;184;-912.5145,-482.8979;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1128.677,532.7899;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-589.491,-485.1602;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-8.141606,-34.64925;Float;False;187;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;157;-54.44904,115.0451;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;143;-818.6287,528.7582;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-396.0391,171.6789;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432.4345,117.531;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/RealOnly;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;8;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;168;0;167;0
WireConnection;168;1;159;4
WireConnection;148;0;4;0
WireConnection;148;4;156;0
WireConnection;165;0;78;0
WireConnection;165;1;159;0
WireConnection;165;2;168;0
WireConnection;150;0;148;0
WireConnection;150;1;2;1
WireConnection;111;0;4;0
WireConnection;186;0;181;4
WireConnection;186;1;183;0
WireConnection;154;0;156;0
WireConnection;154;1;150;0
WireConnection;184;0;165;0
WireConnection;184;1;181;0
WireConnection;184;2;186;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;187;0;184;0
WireConnection;143;0;73;0
WireConnection;151;0;154;0
WireConnection;151;1;152;0
WireConnection;0;0;189;0
WireConnection;0;1;157;0
WireConnection;0;2;151;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=D1F46463562F9F7EDD89717359AA4B2AF56E8636