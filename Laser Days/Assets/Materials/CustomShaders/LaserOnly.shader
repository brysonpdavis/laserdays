// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/LaserOnly"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_HoverColor("Hover Color", Color) = (0.5,0.5,0.5,0.2)
		_HoldColor("Hold Color", Color) = (0.25,0.25,0.25,0.4)
		[HDR]_DissolveEdgeColor("Dissolve Edge Color", Color) = (0,0,0,0)
		_DissolveEdgeThickness("Dissolve Edge Thickness", Range( 0 , 0.5)) = 0.01
		_SelectedEffectMap("Selected Effect Map", 2D) = "white" {}
		[HDR]_SelectedEffectColor("Selected Effect Color", Color) = (8,8,8,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0.7977871
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		_Clip("Clip", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float4 _HoverColor;
		uniform half _onHover;
		uniform float4 _HoldColor;
		uniform half _onHold;
		uniform float _TransitionState;
		uniform float _DissolveEdgeThickness;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;
		uniform float4 _DissolveEdgeColor;
		uniform sampler2D _SelectedEffectMap;
		uniform float4 _SelectedEffectMap_ST;
		uniform float4 _SelectedEffectColor;
		uniform float _Clip;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 Norm182 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = Norm182;
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 lerpResult171 = lerp( tex2D( _Diffuse, uv_Diffuse ) , _HoverColor , ( _onHover * _HoverColor.a ));
			float4 lerpResult172 = lerp( lerpResult171 , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 Albed173 = lerpResult172;
			o.Albedo = Albed173.rgb;
			float TransState175 = _TransitionState;
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float DisMap178 = tex2D( _DissolveMap, uv_DissolveMap ).r;
			float2 uv_SelectedEffectMap = i.uv_texcoord * _SelectedEffectMap_ST.xy + _SelectedEffectMap_ST.zw;
			float4 Tem189 = ( tex2D( _SelectedEffectMap, uv_SelectedEffectMap ) * _SelectedEffectColor * 0.0 );
			float4 Emission181 = ( ( step( ( (-1.0 + (TransState175 - 0.0) * (_DissolveEdgeThickness - -1.0) / (1.0 - 0.0)) + DisMap178 ) , _DissolveEdgeThickness ) * _DissolveEdgeColor ) + Tem189 );
			o.Emission = Emission181.rgb;
			o.Alpha = 1;
			clip( step( 0.0 , ( (-1.0 + (TransState175 - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + DisMap178 ) ) - _Clip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
3;45;1680;952;2873.027;219.7877;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2243.987,-292.5623;Float;False;Property;_TransitionState;Transition State;9;0;Create;True;0;0;False;0;0.7977871;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;-2190.49,471.1954;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2199.169,626.5402;Float;False;Property;_DissolveEdgeThickness;Dissolve Edge Thickness;6;0;Create;True;0;0;False;0;0.01;0.01;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2243.348,-175.5945;Float;True;Property;_DissolveMap;Dissolve Map;2;0;Create;True;0;0;False;0;None;3dccaab28b6f14a279bb0f791c815a1c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;175;-1919.868,-296.0479;Float;False;TransState;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1907.88,467.3825;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;180;-1839.417,641.5033;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;178;-1928.723,-172.2532;Float;False;DisMap;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;186;-1107.02,1074.889;Float;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;164;-2235.287,-742.5825;Float;False;Property;_HoverColor;Hover Color;3;0;Create;True;0;0;False;0;0.5,0.5,0.5,0.2;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;165;-2232.536,-561.0424;Half;False;Property;_onHover;onHover;11;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;185;-1416.064,957.4266;Float;True;Property;_SelectedEffectMap;Selected Effect Map;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1623.839,476.6639;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;187;-1340.571,1144.456;Float;False;Property;_SelectedEffectColor;Selected Effect Color;8;1;[HDR];Create;True;0;0;False;0;8,8,8,0;8,8,8,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;168;-1475.543,-592.3674;Float;False;Property;_HoldColor;Hold Color;4;0;Create;True;0;0;False;0;0.25,0.25,0.25,0.4;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;177;-1333.635,81.64419;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-959.4354,1030.497;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;154;-1410.145,475.3062;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;152;-1424.418,694.7152;Float;False;Property;_DissolveEdgeColor;Dissolve Edge Color;5;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;167;-1549.609,-413.5626;Half;False;Property;_onHold;onHold;10;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1740.222,-578.11;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-2238.141,-938.8607;Float;True;Property;_Diffuse;Diffuse;0;0;Create;True;0;0;False;0;None;823e8f59cd24e40f0ab3d7a1074aa7e1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;179;-987.0429,159.3281;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;189;-769.4019,1030.009;Float;False;Tem;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1262.296,-472.6089;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-1155.685,79.46755;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;171;-1476.577,-723.9583;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1151.258,479.9462;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;191;-1123.567,695.8534;Float;False;189;0;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;172;-973.1292,-688.2855;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-818.9297,87.52805;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-835.4854,527.9783;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;157;-2235.063,39.16732;Float;True;Property;_Normal;Normal;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;174;-468.0909,-285.1394;Float;False;173;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;-684.4948,471.1503;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-475.2973,-116.9779;Float;False;181;0;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;143;-508.8815,83.49632;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-650.1054,-690.5479;Float;False;Albed;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-2077.027,279.2123;Float;False;Constant;_Clip;Clip;13;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;182;-1886.893,80.22084;Float;False;Norm;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-464.8398,-203.6259;Float;False;182;0;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-212.0929,-140.4752;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/LaserOnly;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.27;True;True;0;False;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;2;0.7608291,0.9622642,0.1770203,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;12;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;192;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;175;0;4;0
WireConnection;148;0;176;0
WireConnection;148;4;156;0
WireConnection;178;0;2;1
WireConnection;150;0;148;0
WireConnection;150;1;180;0
WireConnection;188;0;185;0
WireConnection;188;1;187;0
WireConnection;188;2;186;0
WireConnection;154;0;150;0
WireConnection;154;1;156;0
WireConnection;169;0;165;0
WireConnection;169;1;164;4
WireConnection;189;0;188;0
WireConnection;170;0;168;4
WireConnection;170;1;167;0
WireConnection;111;0;177;0
WireConnection;171;0;78;0
WireConnection;171;1;164;0
WireConnection;171;2;169;0
WireConnection;151;0;154;0
WireConnection;151;1;152;0
WireConnection;172;0;171;0
WireConnection;172;1;168;0
WireConnection;172;2;170;0
WireConnection;73;0;111;0
WireConnection;73;1;179;0
WireConnection;190;0;151;0
WireConnection;190;1;191;0
WireConnection;181;0;190;0
WireConnection;143;1;73;0
WireConnection;173;0;172;0
WireConnection;182;0;157;0
WireConnection;0;0;174;0
WireConnection;0;1;183;0
WireConnection;0;2;184;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=3AAF3498D8A5FB99805E9DDAF64634BA3530FF5A