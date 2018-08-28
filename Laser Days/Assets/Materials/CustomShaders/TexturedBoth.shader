// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/BothTextured"
{
	Properties
	{
		_AlbedoReal("AlbedoReal", 2D) = "white" {}
		_AlbedoLaser("AlbedoLaser", 2D) = "white" {}
		_NormalReal("Normal Real", 2D) = "bump" {}
		_NormalLaser("Normal Laser", 2D) = "bump" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		_NormalScale("Normal Scale", Range( 0 , 10)) = 6.588235
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Off
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalScale;
		uniform float _TransitionState;
		uniform sampler2D _NormalReal;
		uniform float4 _NormalReal_ST;
		uniform sampler2D _NormalLaser;
		uniform float4 _NormalLaser_ST;
		uniform sampler2D _AlbedoReal;
		uniform float4 _AlbedoReal_ST;
		uniform sampler2D _AlbedoLaser;
		uniform float4 _AlbedoLaser_ST;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float TansState168 = _TransitionState;
			float2 uv_NormalReal = i.uv_texcoord * _NormalReal_ST.xy + _NormalReal_ST.zw;
			float2 uv_NormalLaser = i.uv_texcoord * _NormalLaser_ST.xy + _NormalLaser_ST.zw;
			float3 norm172 = BlendNormals( UnpackScaleNormal( tex2D( _NormalReal, uv_NormalReal ) ,( _NormalScale * ( 1.0 - TansState168 ) ) ) , UnpackScaleNormal( tex2D( _NormalLaser, uv_NormalLaser ) ,( _NormalScale * TansState168 ) ) );
			o.Normal = norm172;
			float2 uv_AlbedoReal = i.uv_texcoord * _AlbedoReal_ST.xy + _AlbedoReal_ST.zw;
			float2 uv_AlbedoLaser = i.uv_texcoord * _AlbedoLaser_ST.xy + _AlbedoLaser_ST.zw;
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float4 lerpResult175 = lerp( tex2D( _AlbedoReal, uv_AlbedoReal ) , tex2D( _AlbedoLaser, uv_AlbedoLaser ) , saturate( ( (-1.0 + (TansState168 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + tex2D( _DissolveMap, uv_DissolveMap ).r ) ));
			o.Albedo = lerpResult175.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
0;45;1680;952;3001.151;295.5322;1.482371;False;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2721.885,248.5768;Float;False;Property;_TransitionState;Transition State;6;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;128;-2381.052,167.1603;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;4;73;111;2;168;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2356.297,251.992;Float;False;TansState;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;-2737.552,824.5155;Float;False;168;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-2496.106,730.4395;Float;False;Property;_NormalScale;Normal Scale;10;0;Create;True;0;0;False;0;6.588235;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;170;-2437.309,819.3169;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2046.21,216.8187;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-2162.1,908.8987;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-2162.1,787.6023;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2354.249,336.1109;Float;True;Property;_DissolveMap;Dissolve Map;5;0;Create;True;0;0;False;0;e28dc97a9541e3642a48c0e3886688c5;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;157;-1975.733,747.3961;Float;True;Property;_NormalReal;Normal Real;3;0;Create;True;0;0;False;0;5b653e484c8e303439ef414b62f969f0;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1814.877,226.1585;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;159;-1968.221,949.6127;Float;True;Property;_NormalLaser;Normal Laser;4;0;Create;True;0;0;False;0;e5895469fa6440e4a876bf0672599e59;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-1709.346,-10.10107;Float;True;Property;_AlbedoReal;AlbedoReal;1;0;Create;True;0;0;False;0;84508b93f15f2b64386ec07486afc7a3;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;158;-1702.616,445.2473;Float;True;Property;_AlbedoLaser;AlbedoLaser;2;0;Create;True;0;0;False;0;6b2910686f14f5844bf4707db2d5e2ba;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;174;-1597.035,223.8237;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;171;-1658.492,872.9747;Float;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1984.159,-195.5699;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;166;-2403.49,-370.0686;Float;False;Property;_ToggleSwitch0;IsSelected;8;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;165;-2404.499,-474.0987;Float;False;Property;_IsFlippable;IsFlippable;9;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-1376.026,843.8535;Float;False;norm;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2335.365,72.89116;Float;False;Property;_EdgeThickness;Edge Thickness;7;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;175;-1200.119,187.9731;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-712.3373,206.2669;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/BothTextured;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;168;0;4;0
WireConnection;170;0;169;0
WireConnection;111;0;168;0
WireConnection;178;0;176;0
WireConnection;178;1;169;0
WireConnection;177;0;176;0
WireConnection;177;1;170;0
WireConnection;157;5;177;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;159;5;178;0
WireConnection;174;0;73;0
WireConnection;171;0;157;0
WireConnection;171;1;159;0
WireConnection;148;0;4;0
WireConnection;148;4;156;0
WireConnection;172;0;171;0
WireConnection;175;0;78;0
WireConnection;175;1;158;0
WireConnection;175;2;174;0
WireConnection;0;0;175;0
WireConnection;0;1;172;0
ASEEND*/
//CHKSM=E6D34B152E59280FC7A5B5B3E8B836E3212186E9