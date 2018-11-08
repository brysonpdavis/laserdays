// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/FlipEffect2"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		[HDR]_Color("Color", Color) = (0,0,0,0)
		_DepthThinkness("Depth Thinkness", Float) = 0.1
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_Mask("Mask", 2D) = "white" {}
		_Opacity("Opacity", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _Color;
		uniform float _Opacity;
		uniform float _TransitionState;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthThinkness;
		uniform float _Cutoff = 0.2;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color.rgb;
			o.Alpha = _Opacity;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth53 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth53 = abs( ( screenDepth53 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthThinkness ) );
			float clampResult54 = clamp( distanceDepth53 , 0.0 , 1.0 );
			clip( ( ( -(-1.0 + (_TransitionState - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + tex2D( _Mask, uv_Mask ).r ) * ( 1.0 - clampResult54 ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
239;46;1273;781;1067.487;993.4354;1.767279;True;False
Node;AmplifyShaderEditor.RangedFloatNode;52;-541.0579,-575.6536;Float;False;Property;_DepthThinkness;Depth Thinkness;2;0;Create;True;0;0;False;0;0.1;0.18;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-387.2358,-415.8263;Float;False;Property;_TransitionState;Transition State;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;53;-341.2618,-576.7616;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-12.13503,-404.8806;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;101;-391.4961,-282.7967;Float;True;Property;_Mask;Mask;4;0;Create;True;0;0;False;0;None;0f9deff7cdfa54eb0ae31d1428c54bae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;112;206.8958,-363.9237;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;54;-87.68121,-558.5295;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;231.1287,-480.8069;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;103;380.7774,-324.9845;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;785.5397,-475.423;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;518.7964,-676.168;Float;False;Property;_Color;Color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;764.2273,-329.1493;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;37;1204.828,-559.1193;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Transition/FlipEffect2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;3;Custom;0.2;True;False;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Legacy Shaders/VertexLit;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;52;0
WireConnection;111;0;18;0
WireConnection;112;0;111;0
WireConnection;54;0;53;0
WireConnection;56;0;54;0
WireConnection;103;0;112;0
WireConnection;103;1;101;1
WireConnection;114;0;103;0
WireConnection;114;1;56;0
WireConnection;37;2;25;0
WireConnection;37;9;116;0
WireConnection;37;10;114;0
ASEEND*/
//CHKSM=3C5C5C42A4920EA913D6BFE97EC28DE6BDB56B4E