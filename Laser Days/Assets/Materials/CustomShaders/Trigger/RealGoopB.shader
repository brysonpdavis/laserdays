// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Trigger/RealGoopOpaque"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		_TriggerMap("Trigger Map", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_LaserAccent("Laser Accent", Color) = (0,0,0,0)
		_RealAccent("Real Accent", Color) = (0,0,0,0)
		_LaserBass("Laser Bass", Color) = (0,0,0,0)
		_REALBASE("REAL BASE", Color) = (0,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Off
		ZWrite On
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _REALBASE;
		uniform float4 _LaserBass;
		uniform float _TransitionState;
		uniform float4 _RealAccent;
		uniform float4 _LaserAccent;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform sampler2D _TriggerMap;
		uniform float4 _TriggerMap_ST;
		uniform float _Cutoff = 0.2;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( (-0.1 + (sin( ( ( _Time.y + ase_vertex3Pos.y + (-5.0 + (v.color.g - -1.0) * (5.0 - -5.0) / (1.0 - -1.0)) + ase_vertex3Pos.x ) * 2.0 ) ) - -1.0) * (0.1 - -0.1) / (1.0 - -1.0)) * ase_vertexNormal * v.color.r * 1.8 );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float4 lerpResult101 = lerp( _REALBASE , _LaserBass , _TransitionState);
			float4 lerpResult103 = lerp( _RealAccent , _LaserAccent , _TransitionState);
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode95 = tex2D( _TextureSample0, uv_TextureSample0 );
			float temp_output_97_0 = step( 0.5 , tex2DNode95.r );
			float4 lerpResult96 = lerp( lerpResult101 , lerpResult103 , temp_output_97_0);
			o.Albedo = lerpResult96.rgb;
			float3 temp_cast_1 = (( temp_output_97_0 * 0.125 )).xxx;
			o.Specular = temp_cast_1;
			o.Alpha = 1;
			float2 uv_TriggerMap = i.uv_texcoord * _TriggerMap_ST.xy + _TriggerMap_ST.zw;
			float temp_output_20_0 = step( ( _TransitionState + tex2D( _TriggerMap, uv_TriggerMap ).r ) , 1.0 );
			clip( ( temp_output_20_0 * tex2DNode95.a ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
234;130;1162;807;760.1092;1222.211;1.907748;True;False
Node;AmplifyShaderEditor.VertexColorNode;89;-1769.689,471.9817;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;86;-1899.806,-40.36675;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;70;-1872.217,111.189;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;91;-1524.155,366.0551;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-5;False;4;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-988.1284,259.2962;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-808.1491,297.1988;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-255.9204,-415.1375;Float;True;Property;_TriggerMap;Trigger Map;1;0;Create;True;0;0;False;0;None;f83798656917e460288e14ca743332d6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-1096.257,-973.7046;Float;False;Property;_TransitionState;Transition State;2;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;95;-184.3808,-1313.247;Float;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;False;0;None;dfcee1668629140f79597604322df4ae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;100;-452.1123,-969.8364;Float;False;Property;_LaserBass;Laser Bass;5;0;Create;True;0;0;False;0;0,0,0,0;0.7924528,0.7588109,0.7906899,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;75;-643.6158,261.6767;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;93;-456.1513,-1146.92;Float;False;Property;_REALBASE;REAL BASE;6;0;Create;True;0;0;False;0;0,0,0,0;0.6877933,0.6886792,0.6789337,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;102;-758.0786,-571.3688;Float;False;Property;_LaserAccent;Laser Accent;3;0;Create;True;0;0;False;0;0,0,0,0;1,0.5668885,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;19;115.4243,-486.7412;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;94;-743.1158,-732.8499;Float;False;Property;_RealAccent;Real Accent;4;0;Create;True;0;0;False;0;0,0,0,0;0.7547169,0.5845551,0.2883588,0.3647059;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;101;-195.9542,-1062.338;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;97;371.0455,-1218.33;Float;False;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;103;-153.2612,-829.8983;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;65;-384.8031,382.17;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;20;423.7515,-450.3679;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-500.0196,643.5593;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;1.8;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;67;-458.2423,152.6666;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-0.1;False;4;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-69.6041,435.1256;Float;False;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;96;638.6926,-1073.832;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;99;670.9194,-224.3081;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;104;414.883,-640.7159;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-993.2715,120.9051;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;1.77;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;905.2933,-305.8108;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;803.8782,-645.1284;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.125;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;37;1204.828,-559.1193;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Trigger/RealGoopOpaque;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;3;Custom;0.2;True;False;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Legacy Shaders/VertexLit;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;91;0;89;2
WireConnection;74;0;86;0
WireConnection;74;1;70;2
WireConnection;74;2;91;0
WireConnection;74;3;70;1
WireConnection;79;0;74;0
WireConnection;75;0;79;0
WireConnection;19;0;18;0
WireConnection;19;1;1;1
WireConnection;101;0;93;0
WireConnection;101;1;100;0
WireConnection;101;2;18;0
WireConnection;97;1;95;1
WireConnection;103;0;94;0
WireConnection;103;1;102;0
WireConnection;103;2;18;0
WireConnection;20;0;19;0
WireConnection;67;0;75;0
WireConnection;68;0;67;0
WireConnection;68;1;65;0
WireConnection;68;2;89;1
WireConnection;68;3;92;0
WireConnection;96;0;101;0
WireConnection;96;1;103;0
WireConnection;96;2;97;0
WireConnection;99;0;20;0
WireConnection;104;0;95;4
WireConnection;104;1;95;3
WireConnection;104;2;18;0
WireConnection;88;0;20;0
WireConnection;88;1;95;4
WireConnection;98;0;97;0
WireConnection;37;0;96;0
WireConnection;37;3;98;0
WireConnection;37;10;88;0
WireConnection;37;11;68;0
ASEEND*/
//CHKSM=AD375C21CD895D2C66DC733B0442978CE3F46847