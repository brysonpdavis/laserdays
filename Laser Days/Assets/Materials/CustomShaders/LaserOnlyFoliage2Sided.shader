// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/LaserOnlyFoliage2Side"
{
	Properties
	{
		_MaterialMap("Material Map", 2D) = "white" {}
		_pallete("pallete", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "gray" {}
		_InteractColor("Interact Color", Color) = (0.25,0.25,0.25,0.4)
		_EdgeThickness("Edge Thickness", Range( 0 , 0.5)) = 0.3560508
		[HDR]_ShimmerColor("Shimmer Color", Color) = (8,8,8,0)
		[PerRendererData]_TransitionState("Transition State", Range( 0 , 1)) = 1
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		_Shimmer("Shimmer", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		[HideInInspector]_Elapsed("Elapsed", Float) = 0
		_Flippable("Flippable", Int) = 1
		[HideInInspector]_Clip("Clip", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _MaterialMap;
		uniform float4 _MaterialMap_ST;
		uniform sampler2D _pallete;
		uniform float4 _InteractColor;
		uniform half _onHover;
		uniform half _onHold;
		uniform float _TransitionState;
		uniform float _EdgeThickness;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float4 _ShimmerColor;
		uniform float _Elapsed;
		uniform int _Flippable;
		uniform float _Shimmer;
		uniform float _Clip;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORM182 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), 0.5 );
			o.Normal = NORM182;
			float2 uv_MaterialMap = i.uv_texcoord * _MaterialMap_ST.xy + _MaterialMap_ST.zw;
			float4 tex2DNode78 = tex2D( _MaterialMap, uv_MaterialMap );
			float4 appendResult242 = (float4(i.vertexColor.r , i.vertexColor.g , 0.0 , 0.0));
			float temp_output_169_0 = ( _InteractColor.a * 0.5 );
			float4 lerpResult235 = lerp( ( tex2DNode78.r * tex2D( _pallete, appendResult242.xy ) ) , _InteractColor , ( temp_output_169_0 * _onHover ));
			float4 lerpResult172 = lerp( lerpResult235 , _InteractColor , ( temp_output_169_0 * _onHold ));
			float4 DIFFUSE173 = lerpResult172;
			o.Albedo = DIFFUSE173.rgb;
			float TRANS175 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float DISSOLVE178 = tex2D( _EffectMap, uv_EffectMap ).r;
			float4 EDGE181 = ( ( step( ( ( TRANS175 * ( 1.0 + _EdgeThickness ) ) + -_EdgeThickness + DISSOLVE178 ) , 1.0 ) * _ShimmerColor ) + float4( 0,0,0,0 ) );
			float2 panner216 = ( _Elapsed * float2( 0.1,0.13 ) + i.uv_texcoord);
			float HOLD240 = _onHold;
			float4 SHIMMER224 = ( _ShimmerColor * step( 0.4 , ( tex2D( _EffectMap, uv_EffectMap ).r * tex2D( _EffectMap, panner216 ).g ) ) * HOLD240 * _Flippable * _Shimmer );
			o.Emission = ( EDGE181 + SHIMMER224 ).rgb;
			o.Alpha = 1;
			float CUTOUT244 = tex2DNode78.a;
			clip( ( step( 1.0 , ( TRANS175 + DISSOLVE178 ) ) * CUTOUT244 ) - _Clip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
67;166;1680;953;2342.716;367.9616;1.630999;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2235.168,-459.7347;Float;False;Property;_TransitionState;Transition State;7;1;[PerRendererData];Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2410.029,576.6994;Float;False;Property;_EdgeThickness;Edge Thickness;5;0;Create;True;0;0;False;0;0.3560508;0.056;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;175;-1926.118,-458.5519;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;234;-2306.645,1411.649;Float;False;Property;_Elapsed;Elapsed;11;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;204;-2040.238,510.7202;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;-2213.222,442.2285;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;215;-2391.973,1298.265;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-2254.27,-351.1039;Float;True;Property;_EffectMap;Effect Map;3;0;Create;True;0;0;False;0;None;4213667ef65704f73b4663308f4e1b31;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-1834.629,449.0787;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.28;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;198;-1847.921,544.8185;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;243;-2736.808,-1061.124;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;180;-1905.841,632.6866;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;216;-2013.733,1332.969;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.13;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;178;-1928.723,-336.8405;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;218;-1769.241,1117.332;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;242;-2512.781,-1041.319;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;217;-1768.267,1320.973;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;168;-1676.749,-908.2079;Float;False;Property;_InteractColor;Interact Color;4;0;Create;True;0;0;False;0;0.25,0.25,0.25,0.4;0.2499999,0.2499999,0.2499999,0.4;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;237;-1674.231,-735.5812;Float;False;Constant;_float0;float 0;15;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1674.731,454.1983;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1442.787,-793.5569;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-1596.251,-648.2032;Half;False;Property;_onHover;onHover;10;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-1419.985,1168.186;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;187;-1677.815,748.2719;Float;False;Property;_ShimmerColor;Shimmer Color;6;1;[HDR];Create;True;0;0;False;0;8,8,8,0;0.5019608,4,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-2313.507,-1274.623;Float;True;Property;_MaterialMap;Material Map;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;241;-2311.467,-1062.016;Float;True;Property;_pallete;pallete;1;0;Create;True;0;0;False;0;None;89f2574592dff490f94cee9f8b31bdcc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;201;-1398.428,452.3585;Float;True;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1252.737,-647.9957;Half;False;Property;_onHold;onHold;8;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;222;-1152.502,1386.054;Float;False;240;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;228;-1119.047,1471.251;Float;False;Property;_Flippable;Flippable;12;0;Create;True;0;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;227;-1245.253,1571.118;Float;False;Property;_Shimmer;Shimmer;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;240;-925.972,-598.9337;Float;False;HOLD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-1635.399,-1009.922;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;236;-1166.29,1165.57;Float;True;2;0;FLOAT;0.4;False;1;FLOAT;0.09;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-1260.936,-862.3613;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1151.258,479.9462;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-1148.434,224.5673;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;-1149.551,146.7722;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-924.5787,-692.2678;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;235;-1084.661,-1013.472;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-689.2777,526.6241;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;-907.5472,1284.448;Float;False;5;5;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;INT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-879.4743,147.4588;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;229;-705.4197,-64.5229;Float;False;224;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;-538.2872,520.3663;Float;False;EDGE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;224;-704.5942,1281.663;Float;False;SHIMMER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;172;-613.0083,-830.1483;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;211;-710.1693,-147.6847;Float;False;181;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;244;-2003.468,-774.0802;Float;False;CUTOUT;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-2268.772,-115.0031;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.5;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;143;-655.6588,119.1242;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;245;-613.8575,336.6299;Float;False;244;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-381.8687,-823.6934;Float;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;-465.2437,-118.3773;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-2232.251,261.4507;Float;False;Constant;_Clip;Clip;13;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;182;-1951.478,-117.7006;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;174;-468.0909,-285.1394;Float;False;173;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;-390.4105,201.2572;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-464.8398,-203.6259;Float;False;182;0;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-212.0929,-140.4752;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/LaserOnlyFoliage2Side;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.27;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;2;0.7608291,0.9622642,0.1770203,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;192;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;175;0;4;0
WireConnection;204;1;156;0
WireConnection;203;0;176;0
WireConnection;203;1;204;0
WireConnection;198;0;156;0
WireConnection;216;0;215;0
WireConnection;216;1;234;0
WireConnection;178;0;2;1
WireConnection;242;0;243;1
WireConnection;242;1;243;2
WireConnection;217;1;216;0
WireConnection;150;0;203;0
WireConnection;150;1;198;0
WireConnection;150;2;180;0
WireConnection;169;0;168;4
WireConnection;169;1;237;0
WireConnection;219;0;218;1
WireConnection;219;1;217;2
WireConnection;241;1;242;0
WireConnection;201;0;150;0
WireConnection;240;0;167;0
WireConnection;208;0;78;1
WireConnection;208;1;241;0
WireConnection;236;1;219;0
WireConnection;239;0;169;0
WireConnection;239;1;165;0
WireConnection;151;0;201;0
WireConnection;151;1;187;0
WireConnection;170;0;169;0
WireConnection;170;1;167;0
WireConnection;235;0;208;0
WireConnection;235;1;168;0
WireConnection;235;2;239;0
WireConnection;190;0;151;0
WireConnection;223;0;187;0
WireConnection;223;1;236;0
WireConnection;223;2;222;0
WireConnection;223;3;228;0
WireConnection;223;4;227;0
WireConnection;73;0;177;0
WireConnection;73;1;179;0
WireConnection;181;0;190;0
WireConnection;224;0;223;0
WireConnection;172;0;235;0
WireConnection;172;1;168;0
WireConnection;172;2;170;0
WireConnection;244;0;78;4
WireConnection;143;1;73;0
WireConnection;173;0;172;0
WireConnection;213;0;211;0
WireConnection;213;1;229;0
WireConnection;182;0;157;0
WireConnection;246;0;143;0
WireConnection;246;1;245;0
WireConnection;0;0;174;0
WireConnection;0;1;183;0
WireConnection;0;2;213;0
WireConnection;0;10;246;0
ASEEND*/
//CHKSM=0408B62B120FE298C9CCF4BA46C178D35B19C8FB