// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/BothTexturedUpdate"
{
	Properties
	{
		_MaterialMap("Material Map", 2D) = "white" {}
		_NormalReal("Normal Real", 2D) = "bump" {}
		_NormalLaser("Normal Laser", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "gray" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		_NormalScale("Normal Scale", Range( 0 , 10)) = 6.588235
		_ColorLaser("Color Laser", Color) = (0,0,0,0)
		_ColorReal("Color Real", Color) = (0,0,0,0)
		[Toggle]_SteppedTransition("Stepped Transition", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
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
		uniform sampler2D _MaterialMap;
		uniform float4 _MaterialMap_ST;
		uniform half4 _ColorReal;
		uniform half4 _ColorLaser;
		uniform float _SteppedTransition;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float TRANS168 = _TransitionState;
			float2 uv_NormalReal = i.uv_texcoord * _NormalReal_ST.xy + _NormalReal_ST.zw;
			float2 uv_NormalLaser = i.uv_texcoord * _NormalLaser_ST.xy + _NormalLaser_ST.zw;
			float3 NORM172 = BlendNormals( UnpackScaleNormal( tex2D( _NormalReal, uv_NormalReal ), ( _NormalScale * ( 1.0 - TRANS168 ) ) ) , UnpackScaleNormal( tex2D( _NormalLaser, uv_NormalLaser ), ( _NormalScale * TRANS168 ) ) );
			o.Normal = NORM172;
			float2 uv_MaterialMap = i.uv_texcoord * _MaterialMap_ST.xy + _MaterialMap_ST.zw;
			float4 tex2DNode183 = tex2D( _MaterialMap, uv_MaterialMap );
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode2 = tex2D( _EffectMap, uv_EffectMap );
			float MASK185 = lerp(saturate( ( (-1.0 + (TRANS168 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + tex2DNode2.r ) ),step( 1.0 , ( TRANS168 + tex2DNode2.r ) ),_SteppedTransition);
			float4 lerpResult203 = lerp( ( tex2DNode183.r * _ColorReal ) , ( tex2DNode183.g * _ColorLaser ) , MASK185);
			half4 DIFFUSE204 = lerpResult203;
			o.Albedo = DIFFUSE204.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;44;1680;953;3288.347;1214.379;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2931.437,-46.52526;Float;False;Property;_TransitionState;Transition State;4;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2565.849,-41.89796;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2664.82,50.21706;Float;True;Property;_EffectMap;Effect Map;3;0;Create;True;0;0;False;0;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2354.403,-240.992;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-2173.975,-18.3296;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;212;-2167.959,-254.5089;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;-2890.415,449.4748;Float;False;168;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;210;-1953.762,-22.26138;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;213;-1926.34,-221.9734;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-2648.969,355.399;Float;False;Property;_NormalScale;Normal Scale;6;0;Create;True;0;0;False;0;6.588235;6.588235;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;170;-2590.172,444.2763;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;179;-2666.911,-729.9308;Half;False;Property;_ColorReal;Color Real;8;0;Create;True;0;0;False;0;0,0,0,0;0.5188678,0.3937089,0.3255161,0.4745098;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;197;-2663.435,-549.0589;Half;False;Property;_ColorLaser;Color Laser;7;0;Create;True;0;0;False;0;0,0,0,0;0.4409999,0.4379551,0.4101299,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;211;-1645.552,-72.12125;Float;False;Property;_SteppedTransition;Stepped Transition;9;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-2314.963,412.5617;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-2314.963,533.8585;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;183;-2669.324,-932.3391;Float;True;Property;_MaterialMap;Material Map;0;0;Create;True;0;0;False;0;None;d161e9327bd3c4d789aebc2daa8dcfbb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;186;-1989.211,-550.5268;Float;False;185;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;185;-1326.758,3.78045;Float;False;MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;201;-2230.207,-858.4153;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;159;-2120.003,575.6524;Float;True;Property;_NormalLaser;Normal Laser;2;0;Create;True;0;0;False;0;e5895469fa6440e4a876bf0672599e59;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;157;-2127.515,373.4354;Float;True;Property;_NormalReal;Normal Real;1;0;Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;202;-2226.251,-619.8319;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;203;-1743.115,-739.4244;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;171;-1811.354,497.9345;Float;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-974.4355,269.9512;Float;False;172;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;-950.912,177.9324;Float;False;204;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-1528.888,468.8129;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;204;-1527.72,-721.6079;Half;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2931.943,-140.6217;Float;False;Property;_EdgeThickness;Edge Thickness;5;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-710.8378,206.2669;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Transition/BothTexturedUpdate;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;168;0;4;0
WireConnection;111;0;168;0
WireConnection;73;0;168;0
WireConnection;73;1;2;1
WireConnection;212;0;111;0
WireConnection;212;1;2;1
WireConnection;210;1;73;0
WireConnection;213;0;212;0
WireConnection;170;0;169;0
WireConnection;211;0;213;0
WireConnection;211;1;210;0
WireConnection;177;0;176;0
WireConnection;177;1;170;0
WireConnection;178;0;176;0
WireConnection;178;1;169;0
WireConnection;185;0;211;0
WireConnection;201;0;183;1
WireConnection;201;1;179;0
WireConnection;159;5;178;0
WireConnection;157;5;177;0
WireConnection;202;0;183;2
WireConnection;202;1;197;0
WireConnection;203;0;201;0
WireConnection;203;1;202;0
WireConnection;203;2;186;0
WireConnection;171;0;157;0
WireConnection;171;1;159;0
WireConnection;172;0;171;0
WireConnection;204;0;203;0
WireConnection;0;0;194;0
WireConnection;0;1;214;0
ASEEND*/
//CHKSM=E9342ED42210E4E5A95B36520F348CBE63E614EF