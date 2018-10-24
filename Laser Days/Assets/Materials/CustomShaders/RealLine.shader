// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Trigger/RealLineRender"
{
	Properties
	{
		_ActiveColor("Active Color", Color) = (0,0,0,0)
		_RestingColor("RestingColor", Color) = (0,0,0,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		[Toggle]_isCollide("isCollide", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
		};

		uniform float _isCollide;
		uniform float4 _RestingColor;
		uniform float4 _ActiveColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _TransitionState;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth6 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth6 = abs( ( screenDepth6 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 0.12 ) );
			float clampResult10 = clamp( distanceDepth6 , 0.0 , 1.0 );
			float4 lerpResult22 = lerp( lerp(_RestingColor,_ActiveColor,_isCollide) , float4(1,1,1,0) , ( 1.0 - clampResult10 ));
			o.Emission = lerpResult22.rgb;
			o.Alpha = ( _RestingColor.a * ( 1.0 - _TransitionState ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
407;59;1273;781;1517.513;406.5369;1.609789;True;False
Node;AmplifyShaderEditor.DepthFade;6;-1116.555,-183.3452;Float;False;True;1;0;FLOAT;0.12;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-598.3224,282.4531;Float;False;Property;_TransitionState;Transition State;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;10;-882.6004,-187.7287;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;26;-997.5508,137.5721;Float;False;Property;_ActiveColor;Active Color;0;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0.7215686;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-996.0971,-16.55585;Float;False;Property;_RestingColor;RestingColor;1;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;24;-228.0709,282.4529;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;27;-654.6661,-66.87109;Float;False;Property;_isCollide;isCollide;3;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;9;-697.6266,-193.4457;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;-678.8119,87.66875;Float;False;Constant;_Color1;Color 1;1;0;Create;True;0;0;False;0;1,1,1,0;0,0.5452871,0.8113207,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;22;-191.0464,-92.62746;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-36.84258,225.1021;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;180.8869,-44.64199;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Trigger/RealLineRender;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;6;0
WireConnection;24;0;23;0
WireConnection;27;0;2;0
WireConnection;27;1;26;0
WireConnection;9;0;10;0
WireConnection;22;0;27;0
WireConnection;22;1;25;0
WireConnection;22;2;9;0
WireConnection;16;0;2;4
WireConnection;16;1;24;0
WireConnection;0;2;22;0
WireConnection;0;9;16;0
ASEEND*/
//CHKSM=D8C2064C85CA4181A09686B0615CD2FB50DA6899