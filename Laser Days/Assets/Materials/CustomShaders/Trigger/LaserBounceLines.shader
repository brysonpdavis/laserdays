// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Trigger/LaserBounceLines"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (0.7830189,0,0,0.945098)
		_PannerTex("Panner Tex", 2D) = "white" {}
		_MaskTex("Mask Tex", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform float _TransitionState;
		uniform sampler2D _MaskTex;
		uniform float4 _MaskTex_ST;
		uniform sampler2D _PannerTex;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color.rgb;
			float2 uv_MaskTex = i.uv_texcoord * _MaskTex_ST.xy + _MaskTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MaskTex, uv_MaskTex );
			float2 panner8 = ( 1.0 * _Time.y * float2( 0,-0.2 ) + uv_MaskTex);
			o.Alpha = ( _TransitionState * _Color.a * ( tex2DNode1.r * tex2D( _PannerTex, panner8 ).r ) );
		}

		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
156;113;1162;807;1937.993;553.4524;1.495441;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1726.388,-161.7387;Float;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;8;-1478.718,-112.4386;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1220.684,-363.3456;Float;True;Property;_MaskTex;Mask Tex;2;0;Create;True;0;0;False;0;8d83abcffe82e444c885759b7790d3fb;8d83abcffe82e444c885759b7790d3fb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1227.982,-157.9859;Float;True;Property;_PannerTex;Panner Tex;1;0;Create;True;0;0;False;0;58e5051522c8d4a4b845ef9bdc360745;c4e81539e0fcd4491966d8a1485bba79;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;142.3647,-858.1044;Float;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;0.7830189,0,0,0.945098;0.9622642,0.4130474,0.6654837,0.6;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-56.95597,-442.0233;Float;False;Property;_TransitionState;Transition State;3;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-777.7625,-384.5434;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;70;-1275.696,310.1857;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;67;-460.1054,153.2697;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-0.06;False;4;FLOAT;0.06;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-763.8776,380.6335;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;92;598.1365,158.0813;Float;False;Property;_Animated;Animated;4;0;Create;True;0;0;False;0;0;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;75;-643.6158,261.6767;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;55;-776.4763,-933.9506;Float;True;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-528.6346,-931.8315;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1053.042,347.1588;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1721.497,-403.9798;Float;True;Constant;_Clip;Clip;2;1;[HideInInspector];Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;542.9884,-415.0884;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;54;-947.1453,-932.5126;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;53;-1200.726,-950.7446;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;94;-1461.627,46.87195;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;93;-1224.26,34.18814;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;58e5051522c8d4a4b845ef9bdc360745;58e5051522c8d4a4b845ef9bdc360745;True;0;False;white;Auto;False;Instance;4;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-658.0507,-98.81799;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-40.89135,348.9874;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1400.522,-949.6366;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;65;-477.6974,375.6402;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-899.9771,252.7665;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;37;1204.828,-559.1193;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Trigger/LaserBounceLines;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;3;Transparent;0.2;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Legacy Shaders/VertexLit;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;62;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;6;0
WireConnection;4;1;8;0
WireConnection;10;0;1;1
WireConnection;10;1;4;1
WireConnection;67;0;75;0
WireConnection;79;0;74;0
WireConnection;92;1;68;0
WireConnection;75;0;79;0
WireConnection;55;1;54;0
WireConnection;56;0;55;0
WireConnection;76;0;70;1
WireConnection;76;1;70;3
WireConnection;86;0;18;0
WireConnection;86;1;17;4
WireConnection;86;2;10;0
WireConnection;54;0;53;0
WireConnection;53;0;52;0
WireConnection;94;0;6;0
WireConnection;93;1;94;0
WireConnection;90;0;1;4
WireConnection;68;0;67;0
WireConnection;68;1;65;0
WireConnection;74;1;76;0
WireConnection;37;2;17;0
WireConnection;37;9;86;0
ASEEND*/
//CHKSM=62DD7B5DC082BA42F112E0DFEA64DCAC8AD53C86