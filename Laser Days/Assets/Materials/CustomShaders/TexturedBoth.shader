// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/Both"
{
	Properties
	{
		_AlbedoReal("AlbedoReal", 2D) = "white" {}
		_AlbedoLaser("AlbedoLaser", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 0.7666255
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

		uniform float _TransitionState;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;
		uniform sampler2D _AlbedoLaser;
		uniform float4 _AlbedoLaser_ST;
		uniform sampler2D _AlbedoReal;
		uniform float4 _AlbedoReal_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float temp_output_73_0 = ( (-1.0 + (_TransitionState - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + tex2D( _DissolveMap, uv_DissolveMap ).r );
			float2 uv_AlbedoLaser = i.uv_texcoord * _AlbedoLaser_ST.xy + _AlbedoLaser_ST.zw;
			float2 uv_AlbedoReal = i.uv_texcoord * _AlbedoReal_ST.xy + _AlbedoReal_ST.zw;
			o.Albedo = ( ( step( temp_output_73_0 , 0.0 ) * tex2D( _AlbedoLaser, uv_AlbedoLaser ) ) + ( step( 0.0 , temp_output_73_0 ) * tex2D( _AlbedoReal, uv_AlbedoReal ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
59;104;1343;886;2584.319;526.9595;2.337584;False;False
Node;AmplifyShaderEditor.CommentaryNode;128;-2381.052,167.1603;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;4;4;73;111;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2336.492,226.8104;Float;False;Property;_TransitionState;Transition State;6;0;Create;True;0;0;False;0;0.7666255;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2337.604,368.1202;Float;True;Property;_DissolveMap;Dissolve Map;5;0;Create;True;0;0;False;0;e28dc97a9541e3642a48c0e3886688c5;3eef1ebc0df734b078e629b5c879aae3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2006.518,228.3421;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1717.568,227.4389;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;143;-1318.518,-115.8567;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;158;-1323.627,123.866;Float;True;Property;_AlbedoLaser;AlbedoLaser;2;0;Create;True;0;0;False;0;6b2910686f14f5844bf4707db2d5e2ba;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;162;-1327.146,522.0078;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;-1344.708,732.4458;Float;True;Property;_AlbedoReal;AlbedoReal;1;0;Create;True;0;0;False;0;84508b93f15f2b64386ec07486afc7a3;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-812.1306,224.9081;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;129;-643.7097,1094.88;Float;False;814.5701;432.0292;Burn Effect - Emission;4;150;151;154;152;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-813.8564,577.8184;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;55.86415,1243.314;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2335.365,72.89116;Float;False;Property;_EdgeThickness;Edge Thickness;8;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;132;-32,240;Float;False;607.5798;560.8974;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;164;-456.8293,411.5062;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;152;-160.3912,1310.694;Float;False;Property;_EdgeColor;Edge Color;7;0;Create;True;0;0;False;0;0,0,0,0;0.843776,1,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;159;-1317.79,336.3121;Float;True;Property;_NormalLaser;Normal Laser;4;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1984.159,-195.5699;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-658.4675,1204.143;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-1336.788,944.8921;Float;True;Property;_NormalReal;NormalReal;3;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;154;-384.634,1204.11;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;96,336;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/Both;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;111;0;4;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;143;0;73;0
WireConnection;162;1;73;0
WireConnection;161;0;143;0
WireConnection;161;1;158;0
WireConnection;163;0;162;0
WireConnection;163;1;78;0
WireConnection;151;0;154;0
WireConnection;151;1;152;0
WireConnection;164;0;161;0
WireConnection;164;1;163;0
WireConnection;148;0;4;0
WireConnection;148;4;156;0
WireConnection;154;1;150;0
WireConnection;0;0;164;0
ASEEND*/
//CHKSM=22547A7514CFA60185737EB2B683F69CCF53893B