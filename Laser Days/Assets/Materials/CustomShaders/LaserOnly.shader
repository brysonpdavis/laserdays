// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/LaserOnly"
{
	Properties
	{
		_TransitionState("Transition State", Range( 0 , 1)) = 0.7977871
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_HoldColor("Hold Color", Color) = (0,0,0,0)
		_EdgeColor("Edge Color", Color) = (0,0,0,0)
		_HoverColor("Hover Color", Color) = (0,0,0,0)
		_EdgeThickness("Edge Thickness", Range( 0 , 0.5)) = 0.01
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.27
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
		uniform float _TransitionState;
		uniform float _EdgeThickness;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;
		uniform float4 _EdgeColor;
		uniform float _Cutoff = 0.27;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 lerpResult171 = lerp( tex2D( _Albedo, uv_Albedo ) , _HoverColor , ( _onHover * _HoverColor.a ));
			float4 lerpResult172 = lerp( lerpResult171 , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 Albed173 = lerpResult172;
			o.Albedo = Albed173.rgb;
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
0;45;1680;952;2372.106;1293.125;2.243205;True;False
Node;AmplifyShaderEditor.ColorNode;164;-1682.562,-639.1486;Float;False;Property;_HoverColor;Hover Color;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;165;-1679.811,-456.1145;Half;False;Property;_onHover;onHover;10;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-1764.106,334.1649;Float;False;Property;_EdgeThickness;Edge Thickness;7;0;Create;True;0;0;False;0;0.01;0.01;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1776.987,423.4377;Float;False;Property;_TransitionState;Transition State;0;0;Create;True;0;0;False;0;0.7977871;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1314.46,51.01137;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1031.246,-332.5381;Half;False;Property;_onHold;onHold;8;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;168;-957.1796,-511.3426;Float;False;Property;_HoldColor;Hold Color;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1789.194,531.0343;Float;True;Property;_DissolveMap;Dissolve Map;3;0;Create;True;0;0;False;0;None;3dccaab28b6f14a279bb0f791c815a1c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1221.859,-497.0852;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-1719.777,-857.8359;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;823e8f59cd24e40f0ab3d7a1074aa7e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-743.9326,-391.5842;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-1417.627,533.6931;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1030.419,60.2928;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;171;-958.2136,-642.9335;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1128.677,532.7899;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;154;-756.5851,60.2603;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;152;-519.6148,130.9758;Float;False;Property;_EdgeColor;Edge Color;5;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;172;-454.7654,-607.2607;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;157;-387.072,-126.6589;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-131.7419,-609.5231;Float;False;Albed;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;143;-818.6287,528.7582;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-310.3016,93.67879;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;174;48.31205,-196.1973;Float;False;173;0;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432.4345,-136.469;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/LaserOnly;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.27;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;2;0.7608291,0.9622642,0.1770203,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;9;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;148;0;4;0
WireConnection;148;4;156;0
WireConnection;169;0;165;0
WireConnection;169;1;164;4
WireConnection;170;0;168;4
WireConnection;170;1;167;0
WireConnection;111;0;4;0
WireConnection;150;0;148;0
WireConnection;150;1;2;1
WireConnection;171;0;78;0
WireConnection;171;1;164;0
WireConnection;171;2;169;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;154;0;150;0
WireConnection;154;1;156;0
WireConnection;172;0;171;0
WireConnection;172;1;168;0
WireConnection;172;2;170;0
WireConnection;173;0;172;0
WireConnection;143;1;73;0
WireConnection;151;0;154;0
WireConnection;151;1;152;0
WireConnection;0;0;174;0
WireConnection;0;1;157;0
WireConnection;0;2;151;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=CF887D444C68135275DCDA269509241E2581047C