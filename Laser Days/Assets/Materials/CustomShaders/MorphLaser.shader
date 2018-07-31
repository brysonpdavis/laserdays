// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/Morph_StartLaser"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_AlbedoReal("AlbedoReal", 2D) = "white" {}
		_TintReal("Tint Real", Color) = (0.2325116,0.8962264,0.714236,0)
		_AlbedoLaser("AlbedoLaser", 2D) = "white" {}
		_TintLaser("Tint Laser", Color) = (0.7264151,0.1816038,0.1816038,0)
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		_TransitionStateB("Transition State B", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _TransitionStateB;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;
		uniform sampler2D _AlbedoLaser;
		uniform float4 _AlbedoLaser_ST;
		uniform float4 _TintLaser;
		uniform sampler2D _AlbedoReal;
		uniform float4 _AlbedoReal_ST;
		uniform float4 _TintReal;
		uniform float _TransitionState;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float4 tex2DNode2 = tex2D( _DissolveMap, uv_DissolveMap );
			float temp_output_73_0 = ( (-1.0 + (_TransitionStateB - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + tex2DNode2.r );
			float2 uv_AlbedoLaser = i.uv_texcoord * _AlbedoLaser_ST.xy + _AlbedoLaser_ST.zw;
			float2 uv_AlbedoReal = i.uv_texcoord * _AlbedoReal_ST.xy + _AlbedoReal_ST.zw;
			o.Albedo = ( ( step( 0.0 , temp_output_73_0 ) * tex2D( _AlbedoLaser, uv_AlbedoLaser ) * _TintLaser ) + ( step( temp_output_73_0 , 0.0 ) * tex2D( _AlbedoReal, uv_AlbedoReal ) * _TintReal ) ).rgb;
			o.Alpha = 1;
			clip( step( 0.0 , ( (-1.0 + (_TransitionState - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + tex2DNode2.r ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
94;45;1441;883;2444.914;-122.8993;2.070308;True;False
Node;AmplifyShaderEditor.CommentaryNode;128;-2381.052,167.1603;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;3;73;111;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-2852.584,225.876;Float;False;Property;_TransitionStateB;Transition State B;9;0;Create;True;0;0;False;0;0;0.596;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2006.518,228.3421;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2450.428,368.1202;Float;True;Property;_DissolveMap;Dissolve Map;7;0;Create;True;0;0;False;0;e28dc97a9541e3642a48c0e3886688c5;e28dc97a9541e3642a48c0e3886688c5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1717.568,227.4389;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2834.445,736.1694;Float;False;Property;_TransitionState;Transition State;8;0;Create;True;0;0;False;0;1;0.791;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;143;-1256.878,-147.9096;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;162;-1336.942,479.5555;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;168;-2429.225,759.8628;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;158;-1338.322,74.88245;Float;True;Property;_AlbedoLaser;AlbedoLaser;3;0;Create;True;0;0;False;0;6b2910686f14f5844bf4707db2d5e2ba;6b2910686f14f5844bf4707db2d5e2ba;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-1417.61,708.145;Float;True;Property;_AlbedoReal;AlbedoReal;1;0;Create;True;0;0;False;0;84508b93f15f2b64386ec07486afc7a3;84508b93f15f2b64386ec07486afc7a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;166;-1028.535,225.0501;Float;False;Property;_TintLaser;Tint Laser;4;0;Create;True;0;0;False;0;0.7264151,0.1816038,0.1816038,0;0.7264151,0.1816037,0.1816037,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;167;-1026.039,599.4079;Float;False;Property;_TintReal;Tint Real;2;0;Create;True;0;0;False;0;0.2325116,0.8962264,0.714236,0;0.2325115,0.8962264,0.714236,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-794.0563,511.8181;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-794.9706,183.9878;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;169;-1750.72,746.8652;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2563.16,-76.2112;Float;False;Property;_EdgeThickness;Edge Thickness;10;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;164;-456.8293,411.5062;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;171;-649.0032,799.3185;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-2345.274,-400.7707;Float;True;Property;_NormalReal;NormalReal;5;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1984.159,-195.5699;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;132;-32,240;Float;False;607.5798;560.8974;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;159;-2345.87,-604.0601;Float;True;Property;_NormalLaser;Normal Laser;6;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;96,336;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/Morph_StartLaser;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;111;0;165;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;143;1;73;0
WireConnection;162;0;73;0
WireConnection;168;0;4;0
WireConnection;163;0;162;0
WireConnection;163;1;78;0
WireConnection;163;2;167;0
WireConnection;161;0;143;0
WireConnection;161;1;158;0
WireConnection;161;2;166;0
WireConnection;169;0;168;0
WireConnection;169;1;2;1
WireConnection;164;0;161;0
WireConnection;164;1;163;0
WireConnection;171;1;169;0
WireConnection;148;0;165;0
WireConnection;148;4;156;0
WireConnection;0;0;164;0
WireConnection;0;10;171;0
ASEEND*/
//CHKSM=C1D4550B94FD5880CDC5EA4E49D1A7B875462123