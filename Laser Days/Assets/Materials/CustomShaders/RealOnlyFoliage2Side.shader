// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/RealOnlyFoliage2Side"
{
	Properties
	{
		_MaterialMap("Material Map", 2D) = "white" {}
		_pallete("pallete", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "gray" {}
		_InteractColor("Interact Color", Color) = (0,0,0,0)
		_EdgeThickness("Edge Thickness", Range( 0 , 0.5)) = 0.3560508
		[HDR]_ShimmerColor("Shimmer Color", Color) = (8,8,8,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		[HideInInspector]_Flippable("Flippable", Int) = 1
		[HideInInspector]_Elapsed("Elapsed", Float) = 0
		[HideInInspector]_Clip("Clip", Float) = 0.58
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
		uniform float _Clip;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORM195 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), 0.4 );
			o.Normal = NORM195;
			float2 uv_MaterialMap = i.uv_texcoord * _MaterialMap_ST.xy + _MaterialMap_ST.zw;
			float4 tex2DNode78 = tex2D( _MaterialMap, uv_MaterialMap );
			float4 appendResult287 = (float4(i.vertexColor.r , i.vertexColor.g , 0.0 , 0.0));
			float temp_output_186_0 = ( _InteractColor.a * 0.5 );
			float4 lerpResult184 = lerp( ( tex2DNode78.r * tex2D( _pallete, appendResult287.xy ) ) , _InteractColor , ( temp_output_186_0 * _onHover ));
			float4 lerpResult281 = lerp( lerpResult184 , _InteractColor , ( _onHold * temp_output_186_0 ));
			float4 DIFFUSE187 = lerpResult281;
			o.Albedo = DIFFUSE187.rgb;
			float TRANS192 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float DISSOLVE199 = tex2D( _EffectMap, uv_EffectMap ).r;
			float4 EDGE202 = ( ( step( ( ( -TRANS192 * ( _EdgeThickness + 1.0 ) ) + -_EdgeThickness + DISSOLVE199 ) , -_EdgeThickness ) * _ShimmerColor ) + float4( 0,0,0,0 ) );
			float2 panner259 = ( _Elapsed * float2( 0.1,0.13 ) + i.uv_texcoord);
			float ONHOLD270 = _onHold;
			float4 SHIMMER267 = ( _ShimmerColor * step( 0.4 , ( tex2D( _EffectMap, uv_EffectMap ).b * tex2D( _EffectMap, panner259 ).g ) ) * ONHOLD270 * _Flippable * 1.0 );
			o.Emission = ( EDGE202 + SHIMMER267 ).rgb;
			o.Alpha = 1;
			float CUTOUT289 = tex2DNode78.g;
			clip( step( 0.24 , ( step( 0.0 , ( -TRANS192 + DISSOLVE199 ) ) * CUTOUT289 ) ) - _Clip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
42;186;1162;807;3049.347;1145.14;1.813244;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2422.921,-274.1525;Float;False;Property;_TransitionState;Transition State;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2121.125,-280.8729;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;227;-2842.387,1070.765;Float;False;Property;_EdgeThickness;Edge Thickness;5;0;Create;True;0;0;False;0;0.3560508;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;228;-2567.029,829.722;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;246;-2521.21,926.4931;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;244;-2374.654,830.2751;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;277;-2691.158,1751.136;Float;False;Property;_Elapsed;Elapsed;11;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2409.552,341.5645;Float;True;Property;_EffectMap;Effect Map;3;0;Create;True;0;0;False;0;None;9923cd53423e6459ba29f6a488f208bf;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;258;-2689.378,1610.472;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;259;-2429.733,1649.983;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.13;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;288;-3045.268,-545.1119;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2114.869,350.865;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;231;-2370.413,1055.669;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;245;-2333.41,969.4643;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-2233.353,832.5923;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;181;-1746.166,-651.7408;Float;False;Property;_InteractColor;Interact Color;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;233;-2064.34,831.3255;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;260;-2184.267,1639.59;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;287;-2676.451,-675.3588;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;266;-2185.241,1438.499;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;278;-1686.856,-489.2479;Float;False;Constant;_Float0;Float 0;14;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-1757.466,436.9623;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;262;-1780.985,1556.803;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;284;-2422.944,-686.2702;Float;True;Property;_pallete;pallete;1;0;Create;True;0;0;False;0;None;99f54d771de7f485ba4fa24f5c6ceb40;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-2435.796,-893.547;Float;True;Property;_MaterialMap;Material Map;0;0;Create;True;0;0;False;0;None;2277fed7a54fe42f4990418c9d5e838d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;183;-1637.947,-299.0639;Half;False;Property;_onHold;onHold;8;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;209;-1906.043,1154.761;Float;False;Property;_ShimmerColor;Shimmer Color;6;1;[HDR];Create;True;0;0;False;0;8,8,8,0;0.007577688,0.02830189,0.006541476,0.4627451;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;234;-1890.886,870.8595;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1811.329,-405.5229;Half;False;Property;_onHover;onHover;9;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-1741.395,542.9946;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1432.502,-575.6055;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;236;-1560.307,437.3291;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-1683.426,1877.825;Float;False;Constant;_Shimmer;Shimmer;15;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;271;-1577.364,1701.296;Float;False;270;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;282;-1567.114,1556.566;Float;False;2;0;FLOAT;0.4;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;272;-1553.962,1783.145;Float;False;Property;_Flippable;Flippable;10;1;[HideInInspector];Create;True;0;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1482.149,902.7958;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;270;-1144.252,-271.7979;Float;False;ONHOLD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;-1239.228,-515.1492;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;248;-1754.307,-822.7675;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1385.599,499.1467;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;-1030.717,1022.277;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;279;-1081.887,-373.9983;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;265;-1323.547,1603.065;Float;True;5;5;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;INT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;184;-1084.132,-688.534;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;143;-1130.596,481.3024;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;289;-1997.065,-528.3889;Float;False;CUTOUT;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;290;-1171.153,730.7219;Float;False;289;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;268;-1307.576,192.5512;Float;False;267;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;157;-2413.338,129.4389;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.4;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;267;-1120.594,1602.84;Float;False;SHIMMER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;202;-807.6281,1013.248;Float;False;EDGE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;-890.7852,556.4704;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;-1307.841,112.3084;Float;False;202;0;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;281;-870.2275,-433.1492;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;295;-2441.15,2523.236;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;299;-1807.45,2473.107;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;296;-2216.876,2558.589;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;297;-2063.811,2464.197;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;300;-1641.532,2587.07;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;206;-1298.535,1171.78;Float;False;Constant;_Clip;Clip;12;1;[HideInInspector];Create;True;0;0;False;0;0.58;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;305;-1366.716,2778.874;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;257;-996.6942,156.19;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;285;-3038.819,-723.0729;Float;False;Property;_colShiftX;colShiftX;13;0;Create;True;0;0;False;0;0;0.8200001;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;293;-1697.092,2230.25;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-0.1;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;-2110.492,131.8767;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;304;-2306.259,2293.123;Float;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-965.8171,74.33318;Float;False;195;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-965.4734,-7.828478;Float;False;187;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-683.3878,-451.2804;Float;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-1204.726,2560.417;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;298;-1927.712,2592.063;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;12;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;307;-637.0769,523.791;Float;True;2;0;FLOAT;0.24;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;286;-3038.82,-644.4089;Float;False;Property;_colShiftY;colShiftY;12;0;Create;True;0;0;False;0;0;0.8200001;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-149.6436,272.3096;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/RealOnlyFoliage2Side;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;206;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;4;0
WireConnection;246;0;227;0
WireConnection;244;0;228;0
WireConnection;259;0;258;0
WireConnection;259;1;277;0
WireConnection;199;0;2;1
WireConnection;245;0;227;0
WireConnection;247;0;244;0
WireConnection;247;1;246;0
WireConnection;233;0;247;0
WireConnection;233;1;245;0
WireConnection;233;2;231;0
WireConnection;260;1;259;0
WireConnection;287;0;288;1
WireConnection;287;1;288;2
WireConnection;262;0;266;3
WireConnection;262;1;260;2
WireConnection;284;1;287;0
WireConnection;234;0;233;0
WireConnection;234;1;245;0
WireConnection;186;0;181;4
WireConnection;186;1;278;0
WireConnection;236;0;197;0
WireConnection;282;1;262;0
WireConnection;151;0;234;0
WireConnection;151;1;209;0
WireConnection;270;0;183;0
WireConnection;280;0;186;0
WireConnection;280;1;167;0
WireConnection;248;0;78;1
WireConnection;248;1;284;0
WireConnection;73;0;236;0
WireConnection;73;1;203;0
WireConnection;213;0;151;0
WireConnection;279;0;183;0
WireConnection;279;1;186;0
WireConnection;265;0;209;0
WireConnection;265;1;282;0
WireConnection;265;2;271;0
WireConnection;265;3;272;0
WireConnection;265;4;273;0
WireConnection;184;0;248;0
WireConnection;184;1;181;0
WireConnection;184;2;280;0
WireConnection;143;1;73;0
WireConnection;289;0;78;2
WireConnection;267;0;265;0
WireConnection;202;0;213;0
WireConnection;291;0;143;0
WireConnection;291;1;290;0
WireConnection;281;0;184;0
WireConnection;281;1;181;0
WireConnection;281;2;279;0
WireConnection;299;0;298;0
WireConnection;296;0;295;1
WireConnection;296;1;295;3
WireConnection;297;0;304;0
WireConnection;297;1;296;0
WireConnection;305;0;300;1
WireConnection;305;1;300;2
WireConnection;305;2;300;3
WireConnection;257;0;205;0
WireConnection;257;1;268;0
WireConnection;293;0;299;0
WireConnection;195;0;157;0
WireConnection;187;0;281;0
WireConnection;302;0;293;0
WireConnection;302;1;305;0
WireConnection;298;0;297;0
WireConnection;307;1;291;0
WireConnection;0;0;189;0
WireConnection;0;1;204;0
WireConnection;0;2;257;0
WireConnection;0;10;307;0
ASEEND*/
//CHKSM=B66A3716F27E46B89344E68E418427FD56CDD9F0