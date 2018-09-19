// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/BothTexturedUpdate"
{
	Properties
	{
		_MaterialMap("Material Map", 2D) = "white" {}
		_NormalReal("Normal Real", 2D) = "white" {}
		_NormalLaser("Normal Laser", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "white" {}
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
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
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
2;45;1636;934;3280.34;127.9759;1.499488;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2931.437,-46.52526;Float;False;Property;_TransitionState;Transition State;5;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2565.849,-41.89796;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2354.403,-240.992;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2664.82,50.21706;Float;True;Property;_EffectMap;Effect Map;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-2173.975,-18.3296;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;-2875.523,918.5538;Float;False;168;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;212;-2167.959,-254.5089;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;213;-1926.34,-221.9734;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;210;-1953.762,-22.26138;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;170;-2575.28,913.3553;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-2634.077,824.4778;Float;False;Property;_NormalScale;Normal Scale;7;0;Create;True;0;0;False;0;6.588235;0.1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;179;-2662.352,-722.3334;Half;False;Property;_ColorReal;Color Real;9;0;Create;True;0;0;False;0;0,0,0,0;0.5660378,0.3444286,0.3599125,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;197;-2663.435,-549.0589;Half;False;Property;_ColorLaser;Color Laser;8;0;Create;True;0;0;False;0;0,0,0,0;0.6320754,0.6320754,0.6320754,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;183;-2663.246,-933.8585;Float;True;Property;_MaterialMap;Material Map;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-2300.071,1002.937;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-2300.071,881.6407;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;211;-1645.552,-72.12125;Float;False;Property;_SteppedTransition;Stepped Transition;10;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;202;-2226.251,-619.8319;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;201;-2230.207,-858.4153;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;185;-1326.758,3.78045;Float;False;MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-2112.623,842.5143;Float;True;Property;_NormalReal;Normal Real;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;159;-2105.111,1044.731;Float;True;Property;_NormalLaser;Normal Laser;3;0;Create;True;0;0;False;0;e5895469fa6440e4a876bf0672599e59;e5895469fa6440e4a876bf0672599e59;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;186;-1989.211,-550.5268;Float;False;185;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;171;-1796.462,967.0132;Float;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;203;-1743.115,-739.4244;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-974.4355,269.9512;Float;False;172;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;215;-2379.146,278.647;Float;True;Property;_TextureSample0;Texture Sample 0;11;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;156;-2931.943,-140.6217;Float;False;Property;_EdgeThickness;Edge Thickness;6;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;-950.912,177.9324;Float;False;204;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;-1692.562,341.1016;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;228;-2001.151,394.0056;Float;False;myVarName4;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;218;-2377.126,478.1297;Float;True;Property;_TextureSample1;Texture Sample 1;11;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-1513.996,937.8918;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;221;-2940.092,425.1817;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;225;-2935.749,529.4003;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;219;-2680.419,304.5216;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;220;-2672.308,506.2406;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;226;-3177.479,203.717;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;223;-3303.41,506.2406;Float;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;204;-1527.72,-721.6079;Half;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;222;-3303.41,428.0766;Float;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;224;-3151.424,373.0723;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-712.3373,206.2669;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Transition/BothTexturedUpdate;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;168;0;4;0
WireConnection;111;0;168;0
WireConnection;73;0;168;0
WireConnection;73;1;2;1
WireConnection;212;0;111;0
WireConnection;212;1;2;1
WireConnection;213;0;212;0
WireConnection;210;1;73;0
WireConnection;170;0;169;0
WireConnection;178;0;176;0
WireConnection;178;1;169;0
WireConnection;177;0;176;0
WireConnection;177;1;170;0
WireConnection;211;0;213;0
WireConnection;211;1;210;0
WireConnection;202;0;183;2
WireConnection;202;1;197;0
WireConnection;201;0;183;1
WireConnection;201;1;179;0
WireConnection;185;0;211;0
WireConnection;157;5;177;0
WireConnection;159;5;178;0
WireConnection;171;0;157;0
WireConnection;171;1;159;0
WireConnection;203;0;201;0
WireConnection;203;1;202;0
WireConnection;203;2;186;0
WireConnection;215;1;219;0
WireConnection;227;0;215;4
WireConnection;227;1;218;3
WireConnection;227;2;228;0
WireConnection;218;1;220;0
WireConnection;172;0;171;0
WireConnection;225;0;224;0
WireConnection;219;0;226;0
WireConnection;219;2;224;0
WireConnection;219;1;221;0
WireConnection;220;0;226;0
WireConnection;220;2;225;0
WireConnection;220;1;221;0
WireConnection;204;0;203;0
WireConnection;224;0;222;0
WireConnection;224;1;223;0
WireConnection;0;0;194;0
WireConnection;0;1;214;0
ASEEND*/
//CHKSM=F105EEC2AFE9FCACCE0E7B149D3750FA92DF3EC8