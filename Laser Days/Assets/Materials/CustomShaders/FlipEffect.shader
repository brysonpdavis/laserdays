// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/FlipEffect"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		[HDR]_ActiveColor("Active Color", Color) = (0,0,0,0)
		_Glowamout("Glow amout", Float) = 0.1
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float4 screenPosition;
		};

		uniform float4 _ActiveColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Glowamout;
		uniform float _TransitionState;
		uniform float _Cutoff = 0.2;


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
			o.Emission = _ActiveColor.rgb;
			o.Alpha = _ActiveColor.a;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen103 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither103 = Dither4x4Bayer( fmod(clipScreen103.x, 4), fmod(clipScreen103.y, 4) );
			float screenDepth53 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth53 = abs( ( screenDepth53 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Glowamout ) );
			float clampResult54 = clamp( distanceDepth53 , 0.0 , 1.0 );
			dither103 = step( dither103, ( ( 1.0 - clampResult54 ) * ( 1.0 - _TransitionState ) ) );
			clip( dither103 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
407;59;1273;781;731.3352;1351.146;2.254573;True;False
Node;AmplifyShaderEditor.RangedFloatNode;52;-1400.522,-949.6366;Float;False;Property;_Glowamout;Glow amout;2;0;Create;True;0;0;False;0;0.1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;53;-1200.726,-950.7446;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;54;-947.1453,-932.5126;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-705.1193,-476.0742;Float;False;Property;_TransitionState;Transition State;3;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;102;-343.5377,-489.7393;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-584.3735,-711.6682;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-55.54919,-569.3549;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;103;140.7053,-425.9211;Float;False;0;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;427.4025,-611.8236;Float;False;Property;_ActiveColor;Active Color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.735849,0.735849,0.735849,0.3333333;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;37;1207.083,-559.1193;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Transition/FlipEffect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;3;Custom;0.2;True;False;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Legacy Shaders/VertexLit;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;52;0
WireConnection;54;0;53;0
WireConnection;102;0;18;0
WireConnection;56;0;54;0
WireConnection;105;0;56;0
WireConnection;105;1;102;0
WireConnection;103;0;105;0
WireConnection;37;2;25;0
WireConnection;37;9;25;4
WireConnection;37;10;103;0
ASEEND*/
//CHKSM=D2FB03F2A70431E03AC7204FD14CCD81A6DCF8A1