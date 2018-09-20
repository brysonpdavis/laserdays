// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/RealOnly"
{
	Properties
	{
		_MaterialMap("Material Map", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "gray" {}
		_HoverColor("Hover Color", Color) = (0,0,0,0)
		_HoldColor("Hold Color", Color) = (0,0,0,0)
		[HDR]_EdgeColor("Edge Color", Color) = (1,1,1,0)
		_EdgeThickness("Edge Thickness", Range( 0 , 0.5)) = 0.3560508
		[HDR]_ShimmerColor("Shimmer Color", Color) = (8,8,8,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_TransitionStateB("Transition State B", Range( 0 , 1)) = 0
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		[HideInInspector]_Clip("Clip", Float) = 1
		_ColorLaser("Color Laser", Color) = (0,0,0,0)
		_ColorReal("Color Real", Color) = (0,0,0,0)
		[HideInInspector]_Flippable("Flippable", Int) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPosition;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _MaterialMap;
		uniform float4 _MaterialMap_ST;
		uniform float4 _ColorReal;
		uniform float4 _ColorLaser;
		uniform float _TransitionStateB;
		uniform float4 _HoldColor;
		uniform half _onHold;
		uniform float _TransitionState;
		uniform float _EdgeThickness;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float4 _EdgeColor;
		uniform half _onHover;
		uniform float4 _HoverColor;
		uniform float4 _ShimmerColor;
		uniform int _Flippable;
		uniform float _Clip;


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORM195 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = NORM195;
			float2 uv_MaterialMap = i.uv_texcoord * _MaterialMap_ST.xy + _MaterialMap_ST.zw;
			float4 lerpResult253 = lerp( _ColorReal , _ColorLaser , _TransitionStateB);
			float4 lerpResult184 = lerp( ( tex2D( _MaterialMap, uv_MaterialMap ).r * lerpResult253 ) , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 DIFFUSE187 = lerpResult184;
			o.Albedo = DIFFUSE187.rgb;
			float TRANS192 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float DISSOLVE199 = tex2D( _EffectMap, uv_EffectMap ).r;
			float4 EDGE202 = ( ( step( ( ( -TRANS192 * ( _EdgeThickness + 1.0 ) ) + -_EdgeThickness + DISSOLVE199 ) , -_EdgeThickness ) * _EdgeColor ) + float4( 0,0,0,0 ) );
			float4 HOVER254 = ( _onHover * _HoverColor * _HoverColor.a );
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen263 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither263 = Dither4x4Bayer( fmod(clipScreen263.x, 4), fmod(clipScreen263.y, 4) );
			float2 panner259 = ( 1.0 * _Time.y * float2( 0.1,0.02 ) + i.uv_texcoord);
			dither263 = step( dither263, ( tex2D( _EffectMap, uv_EffectMap ).g * tex2D( _EffectMap, panner259 ).b ) );
			float ONHOLD270 = _onHold;
			float4 SHIMMER267 = ( dither263 * _ShimmerColor * ONHOLD270 * _Flippable * 1.0 );
			o.Emission = ( EDGE202 + HOVER254 + SHIMMER267 ).rgb;
			o.Alpha = 1;
			clip( step( 0.0 , ( -TRANS192 + DISSOLVE199 ) ) - _Clip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
747;61;1636;934;2474.176;-1065.194;1.399883;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2422.921,-274.1525;Float;False;Property;_TransitionState;Transition State;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2121.125,-280.8729;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;228;-2567.029,829.722;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;227;-2842.387,1070.765;Float;False;Property;_EdgeThickness;Edge Thickness;6;0;Create;True;0;0;False;0;0.3560508;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;244;-2374.654,830.2751;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;258;-2689.378,1610.472;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;246;-2521.21,926.4931;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2409.552,341.5645;Float;True;Property;_EffectMap;Effect Map;2;0;Create;True;0;0;False;0;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-2233.353,832.5923;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;259;-2429.733,1649.983;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.02;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NegateNode;245;-2333.41,969.4643;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;231;-2370.413,1055.669;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2114.869,350.865;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;266;-2185.241,1438.499;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;260;-2184.267,1639.59;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;233;-2064.34,831.3255;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;262;-1780.985,1556.803;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;152;-1844.806,1164.362;Float;False;Property;_EdgeColor;Edge Color;5;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;234;-1890.886,870.8595;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-1746.957,-454.48;Half;False;Property;_onHold;onHold;10;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;250;-2423.948,-534.4599;Float;False;Property;_ColorLaser;Color Laser;12;0;Create;True;0;0;False;0;0,0,0,0;0.118147,0.5566037,0.3502212,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;252;-2424.723,-364.3052;Float;False;Property;_TransitionStateB;Transition State B;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;249;-2423.784,-700.5218;Float;False;Property;_ColorReal;Color Real;13;0;Create;True;0;0;False;0;0,0,0,0;0.1341669,0.4245283,0.2232769,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;181;-1746.166,-636.67;Float;False;Property;_HoldColor;Hold Color;4;0;Create;True;0;0;False;0;0,0,0,0;0.3207547,0.2876169,0.1891242,0.5921569;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;253;-2043.615,-662.2018;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DitheringNode;263;-1582.794,1572.737;Float;False;0;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;272;-1553.962,1783.145;Float;False;Property;_Flippable;Flippable;14;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;1;INT;0
Node;AmplifyShaderEditor.ColorNode;209;-2110.336,1848.018;Float;False;Property;_ShimmerColor;Shimmer Color;7;1;[HDR];Create;True;0;0;False;0;8,8,8,0;8,8,8,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;270;-1422.508,-409.2375;Float;False;ONHOLD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1482.149,902.7958;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;159;-2411.138,-78.40791;Float;False;Property;_HoverColor;Hover Color;3;0;Create;True;0;0;False;0;0,0,0,0;1,0.8365737,0,0.4392157;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-2435.796,-893.547;Float;True;Property;_MaterialMap;Material Map;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;167;-2418.048,-155.0123;Half;False;Property;_onHover;onHover;11;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-1757.466,436.9623;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;271;-1577.364,1701.296;Float;False;270;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-1683.426,1877.825;Float;False;Constant;_Shimmer;Shimmer;15;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-2003.242,-179.871;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-1741.395,542.9946;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;248;-1743.991,-766.0264;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;265;-1323.547,1603.065;Float;False;5;5;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;INT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NegateNode;236;-1560.307,437.3291;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;-1030.717,1022.277;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1422.455,-523.511;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1385.599,499.1467;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;256;-1301.086,201.9795;Float;False;254;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;157;-2412.338,130.4389;Float;True;Property;_Normal;Normal;1;0;Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;184;-1271.132,-694.534;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;-1307.841,112.3084;Float;False;202;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;267;-1120.594,1602.84;Float;False;SHIMMER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;268;-1317.089,297.0398;Float;False;267;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;-1754.782,-191.0575;Float;False;HOVER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;202;-807.6281,1013.248;Float;False;EDGE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-965.4734,-7.828478;Float;False;187;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;206;-2529.33,746.8903;Float;False;Constant;_Clip;Clip;12;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;257;-1088.314,142.1926;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-965.8171,74.33318;Float;False;195;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;-2110.492,131.8767;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;143;-1149.203,503.4723;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-1051.026,-683.7703;Float;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-675.3628,108.561;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/RealOnly;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;206;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;4;0
WireConnection;244;0;228;0
WireConnection;246;0;227;0
WireConnection;247;0;244;0
WireConnection;247;1;246;0
WireConnection;259;0;258;0
WireConnection;245;0;227;0
WireConnection;199;0;2;1
WireConnection;260;1;259;0
WireConnection;233;0;247;0
WireConnection;233;1;245;0
WireConnection;233;2;231;0
WireConnection;262;0;266;2
WireConnection;262;1;260;3
WireConnection;234;0;233;0
WireConnection;234;1;245;0
WireConnection;253;0;249;0
WireConnection;253;1;250;0
WireConnection;253;2;252;0
WireConnection;263;0;262;0
WireConnection;270;0;183;0
WireConnection;151;0;234;0
WireConnection;151;1;152;0
WireConnection;168;0;167;0
WireConnection;168;1;159;0
WireConnection;168;2;159;4
WireConnection;248;0;78;1
WireConnection;248;1;253;0
WireConnection;265;0;263;0
WireConnection;265;1;209;0
WireConnection;265;2;271;0
WireConnection;265;3;272;0
WireConnection;265;4;273;0
WireConnection;236;0;197;0
WireConnection;213;0;151;0
WireConnection;186;0;181;4
WireConnection;186;1;183;0
WireConnection;73;0;236;0
WireConnection;73;1;203;0
WireConnection;184;0;248;0
WireConnection;184;1;181;0
WireConnection;184;2;186;0
WireConnection;267;0;265;0
WireConnection;254;0;168;0
WireConnection;202;0;213;0
WireConnection;257;0;205;0
WireConnection;257;1;256;0
WireConnection;257;2;268;0
WireConnection;195;0;157;0
WireConnection;143;1;73;0
WireConnection;187;0;184;0
WireConnection;0;0;189;0
WireConnection;0;1;204;0
WireConnection;0;2;257;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=3666BCD308492FB3CA3335E4F599BE07A17E7C6B