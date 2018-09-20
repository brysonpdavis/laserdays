// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/Selected"
{
	Properties
	{
		_TransitionStateB("Transition State B", Range( 0 , 1)) = 0.7388377
		_MaterialMap("Material Map", 2D) = "white" {}
		_HoldColor("Hold Color", Color) = (0,0,0,0)
		_ColorLaser("Color Laser", Color) = (0,0,0,0)
		_ColorReal("Color Real", Color) = (0,0,0,0)
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HDR]_SecondaryEffectColor("Secondary Effect Color", Color) = (7.999999,7.999999,7.999999,0)
		_EffectMap("Effect Map", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		ZTest Always
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows nofog vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPosition;
		};

		uniform sampler2D _MaterialMap;
		uniform float4 _MaterialMap_ST;
		uniform float4 _ColorReal;
		uniform float4 _ColorLaser;
		uniform float _TransitionStateB;
		uniform float4 _HoldColor;
		uniform half _onHold;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float4 _SecondaryEffectColor;


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

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MaterialMap = i.uv_texcoord * _MaterialMap_ST.xy + _MaterialMap_ST.zw;
			float4 lerpResult213 = lerp( _ColorReal , _ColorLaser , _TransitionStateB);
			float4 lerpResult184 = lerp( ( tex2D( _MaterialMap, uv_MaterialMap ).r * lerpResult213 ) , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 DIFFUSE187 = lerpResult184;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen228 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither228 = Dither4x4Bayer( fmod(clipScreen228.x, 4), fmod(clipScreen228.y, 4) );
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float2 panner224 = ( 1.0 * _Time.y * float2( 0.1,0.02 ) + i.uv_texcoord);
			dither228 = step( dither228, ( tex2D( _EffectMap, uv_EffectMap ).r * tex2D( _EffectMap, panner224 ).g ) );
			o.Emission = ( DIFFUSE187 + ( dither228 * _SecondaryEffectColor.r ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;63;1636;934;1551.641;327.0826;1.580298;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;225;-1487.823,400.8296;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;224;-1228.178,440.3408;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.02;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;210;-2469.969,-490.3112;Float;False;Property;_ColorReal;Color Real;5;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;211;-2467.817,-318.1989;Float;False;Property;_ColorLaser;Color Laser;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;212;-2534.51,-137.4811;Float;False;Property;_TransitionStateB;Transition State B;0;0;Create;True;0;0;False;0;0.7388377;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;223;-982.7119,429.9475;Float;True;Property;_TextureSample0;Texture Sample 0;11;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;222;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;222;-980.817,223.4023;Float;True;Property;_EffectMap;Effect Map;11;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-2468.701,-698.2601;Float;True;Property;_MaterialMap;Material Map;2;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;183;-1980.418,-252.0171;Half;False;Property;_onHold;onHold;8;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;181;-1888.138,-432.3172;Float;False;Property;_HoldColor;Hold Color;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;213;-2130.046,-440.8289;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-473.2835,334.5246;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1674.891,-312.559;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;214;-1880.484,-595.7302;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;190;-930.4207,653.3538;Float;False;Property;_SecondaryEffectColor;Secondary Effect Color;10;1;[HDR];Create;True;0;0;False;0;7.999999,7.999999,7.999999,0;21.36125,6.71212,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DitheringNode;228;-232.1283,406.059;Float;False;0;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;184;-1385.723,-528.2351;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-1062.7,-532.3923;Float;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-13.318,406.059;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-52.4673,51.10281;Float;False;187;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1956.061,51.52676;Half;False;Property;_onHover;onHover;9;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;219;-1107.777,-38.09528;Float;False;HOVER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2532.046,-45.32433;Float;False;Property;_TransitionState;Transition State;1;0;Create;True;0;0;False;0;0.7388377;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-1546.999,-113.5943;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;227;192.6608,247.5385;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;159;-1886.916,-119.6983;Float;False;Property;_HoverColor;Hover Color;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;221;-164.4295,162.1672;Float;False;219;0;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;704.2457,126.8495;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Transition/Selected;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;Back;1;False;-1;7;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.6;True;True;0;True;Custom;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;7;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;224;0;225;0
WireConnection;223;1;224;0
WireConnection;213;0;210;0
WireConnection;213;1;211;0
WireConnection;213;2;212;0
WireConnection;226;0;222;1
WireConnection;226;1;223;2
WireConnection;186;0;181;4
WireConnection;186;1;183;0
WireConnection;214;0;78;1
WireConnection;214;1;213;0
WireConnection;228;0;226;0
WireConnection;184;0;214;0
WireConnection;184;1;181;0
WireConnection;184;2;186;0
WireConnection;187;0;184;0
WireConnection;230;0;228;0
WireConnection;230;1;190;1
WireConnection;219;0;168;0
WireConnection;168;0;159;0
WireConnection;168;1;167;0
WireConnection;168;2;159;4
WireConnection;227;0;189;0
WireConnection;227;1;230;0
WireConnection;0;2;227;0
ASEEND*/
//CHKSM=3E68EEA665E9015A1BC7A18AE395768A16B3CA0C