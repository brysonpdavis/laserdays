// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/RealOnly"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_HoverColor("Hover Color", Color) = (0,0,0,0)
		_HoldColor("Hold Color", Color) = (0,0,0,0)
		[HDR]_DissolveEdgeColor("Dissolve Edge Color", Color) = (1,1,1,0)
		_DissolveEdgeThickness("Dissolve Edge Thickness", Range( 0 , 0.5)) = 0
		_SelectedEffectMap("Selected Effect Map", 2D) = "white" {}
		[HDR]_SelectedEffectColor("Selected Effect Color", Color) = (8,8,8,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0.8157373
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		[HideInInspector]_MaskClip("Mask Clip", Float) = 0.5
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
		uniform float _DissolveEdgeThickness;
		uniform float _TransitionState;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;
		uniform float4 _DissolveEdgeColor;
		uniform sampler2D _SelectedEffectMap;
		uniform float4 _SelectedEffectMap_ST;
		uniform float4 _SelectedEffectColor;
		uniform float _MaskClip;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 Norm195 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = Norm195;
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 lerpResult165 = lerp( tex2D( _Diffuse, uv_Diffuse ) , _HoverColor , ( _onHover * _HoverColor.a ));
			float4 lerpResult184 = lerp( lerpResult165 , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 Diffuse187 = lerpResult184;
			o.Albedo = Diffuse187.rgb;
			float TransState192 = _TransitionState;
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float DisMap199 = tex2D( _DissolveMap, uv_DissolveMap ).r;
			float2 uv_SelectedEffectMap = i.uv_texcoord * _SelectedEffectMap_ST.xy + _SelectedEffectMap_ST.zw;
			float4 Temp212 = ( tex2D( _SelectedEffectMap, uv_SelectedEffectMap ) * _SelectedEffectColor * 0.0 );
			float4 Emission202 = ( ( step( _DissolveEdgeThickness , ( (-1.0 + (TransState192 - 0.0) * (_DissolveEdgeThickness - -1.0) / (1.0 - 0.0)) + DisMap199 ) ) * _DissolveEdgeColor ) + Temp212 );
			o.Emission = Emission202.rgb;
			o.Alpha = 1;
			clip( step( ( (-1.0 + (TransState192 - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + DisMap199 ) , 0.0 ) - _MaskClip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
3;45;1680;952;4205.484;833.1784;3.613435;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2441.227,15.4229;Float;False;Property;_TransitionState;Transition State;9;0;Create;True;0;0;False;0;0.8157373;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2438.184,407.2493;Float;True;Property;_DissolveMap;Dissolve Map;2;0;Create;True;0;0;False;0;None;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;196;-2325.589,1042.523;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2428.075,1150.391;Float;False;Property;_DissolveEdgeThickness;Dissolve Edge Thickness;6;0;Create;True;0;0;False;0;0;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2162.731,13.69514;Float;False;TransState;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;201;-2088.927,1151.885;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-2087.678,985.0059;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2124.974,414.8655;Float;False;DisMap;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;159;-2342.782,-496.4802;Float;False;Property;_HoverColor;Hover Color;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;207;-1449.216,1557.152;Float;True;Property;_SelectedEffectMap;Selected Effect Map;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;209;-1378.563,1744.182;Float;False;Property;_SelectedEffectColor;Selected Effect Color;8;1;[HDR];Create;True;0;0;False;0;8,8,8,0;8,8,8,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;211;-1140.353,1674.615;Float;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1649.675,991.7365;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-2412.469,-319.1395;Half;False;Property;_onHover;onHover;11;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;152;-1372.909,1207.604;Float;False;Property;_DissolveEdgeColor;Dissolve Edge Color;5;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0.843776,1,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-1990.801,-387.422;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-2432.55,-691.9063;Float;True;Property;_Diffuse;Diffuse;0;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;154;-1375.841,991.704;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-1685.435,233.8919;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-992.7681,1630.223;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-1798.548,-213.1676;Half;False;Property;_onHold;onHold;10;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;181;-1797.757,-378.7144;Float;False;Property;_HoldColor;Hold Color;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-802.7346,1629.735;Float;False;Temp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1104.243,996.502;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1474.046,-283.8626;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-1497.713,238.9826;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-883.6964,1068.204;Float;False;212;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-1494.383,401.4658;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;165;-1800.933,-517.7383;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1262.406,328.9556;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-2440.97,201.1763;Float;True;Property;_Normal;Normal;1;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;184;-1322.723,-454.885;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;-701.3194,1016.251;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-965.8171,74.33318;Float;False;195;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-965.4734,-7.828478;Float;False;187;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-1140.895,-460.764;Float;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;206;-2353.131,718.0885;Float;False;Constant;_MaskClip;Mask Clip;12;1;[HideInInspector];Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;202;-478.2301,1007.222;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;-959.3441,158.4809;Float;False;202;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;-2149.674,197.5614;Float;False;Norm;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;143;-991.1627,331.3245;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-675.3628,108.561;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/RealOnly;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;206;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;4;0
WireConnection;148;0;196;0
WireConnection;148;4;156;0
WireConnection;199;0;2;1
WireConnection;150;0;148;0
WireConnection;150;1;201;0
WireConnection;168;0;167;0
WireConnection;168;1;159;4
WireConnection;154;0;156;0
WireConnection;154;1;150;0
WireConnection;210;0;207;0
WireConnection;210;1;209;0
WireConnection;210;2;211;0
WireConnection;212;0;210;0
WireConnection;151;0;154;0
WireConnection;151;1;152;0
WireConnection;186;0;181;4
WireConnection;186;1;183;0
WireConnection;111;0;197;0
WireConnection;165;0;78;0
WireConnection;165;1;159;0
WireConnection;165;2;168;0
WireConnection;73;0;111;0
WireConnection;73;1;203;0
WireConnection;184;0;165;0
WireConnection;184;1;181;0
WireConnection;184;2;186;0
WireConnection;213;0;151;0
WireConnection;213;1;214;0
WireConnection;187;0;184;0
WireConnection;202;0;213;0
WireConnection;195;0;157;0
WireConnection;143;0;73;0
WireConnection;0;0;189;0
WireConnection;0;1;204;0
WireConnection;0;2;205;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=087B502A7401C254CCB23291FB50E03539AFC17B