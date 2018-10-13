// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Trigger/RealGoopOpaque"
{
	Properties
	{
		[HDR]_RestingColor("Resting Color", Color) = (0,0,0,0)
		[HDR]_ShimmerColor("Shimmer Color", Color) = (0,0,0,0)
		[HDR]_ActiveColor("Active Color", Color) = (0,0,0,0)
		[HideInInspector]_Clip("Clip", Float) = 0.5
		_TriggerMap("Trigger Map", 2D) = "white" {}
		[Toggle]_UseFresnel("UseFresnel", Float) = 1
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		[Toggle]_isCollide("isCollide", Float) = 0
		[HideInInspector]_Elapsed("Elapsed", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float _Elapsed;
		uniform float _isCollide;
		uniform float4 _RestingColor;
		uniform float4 _ActiveColor;
		uniform float4 _ShimmerColor;
		uniform float _UseFresnel;
		uniform float _TransitionState;
		uniform sampler2D _TriggerMap;
		uniform float4 _TriggerMap_ST;
		uniform float _Clip;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_vertexNormal = v.normal.xyz;
			v.normal = ( (-0.06 + (sin( ( ( _Elapsed + ( ase_vertex3Pos.x * 2.0 ) ) * 2.0 ) ) - -1.0) * (0.06 - -0.06) / (1.0 - -1.0)) * ase_vertexNormal );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float3 temp_cast_0 = (0.0).xxx;
			o.Albedo = temp_cast_0;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV42 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode42 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV42, 1.91 ) );
			float2 panner7 = ( _Elapsed * float2( 0.1,0.1 ) + i.uv_texcoord);
			float2 panner8 = ( _Elapsed * float2( -0.03,0.2 ) + i.uv_texcoord);
			float clampResult81 = clamp( ( lerp(0.0,step( 0.6 , fresnelNode42 ),_UseFresnel) + step( 0.19 , ( tex2D( _TriggerMap, panner7 ).r * tex2D( _TriggerMap, panner8 ).g * 1.77 ) ) ) , 0.0 , 1.0 );
			o.Emission = ( lerp(_RestingColor,_ActiveColor,_isCollide) + ( _ShimmerColor * clampResult81 ) ).rgb;
			o.Alpha = 1;
			float2 uv_TriggerMap = i.uv_texcoord * _TriggerMap_ST.xy + _TriggerMap_ST.zw;
			clip( step( ( _TransitionState + tex2D( _TriggerMap, uv_TriggerMap ).a ) , 1.0 ) - _Clip );
		}

		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
407;66;1273;781;2574.785;1389.741;2.494073;True;False
Node;AmplifyShaderEditor.RangedFloatNode;63;-1774.02,53.81596;Float;False;Property;_Elapsed;Elapsed;7;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1726.388,-161.7387;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;8;-1394.541,-81.4259;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;7;-1397.913,-227.0347;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;4;-1149.749,-79.26834;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;70;-1275.696,310.1857;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-993.2715,120.9051;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;1.77;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1149.749,-275.2684;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;42;-1624.613,-707.2766;Float;True;Standard;TangentNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1.91;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-733.384,-265.4219;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1053.042,347.1588;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;58;-1287.389,-624.3325;Float;True;2;0;FLOAT;0.6;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;15;-477.4274,-269.2221;Float;True;2;0;FLOAT;0.19;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-899.9771,252.7665;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;59;-976.8994,-590.5269;Float;False;Property;_UseFresnel;UseFresnel;4;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-763.8776,380.6335;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-226.055,-528.6957;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-41.58624,-669.389;Float;False;Property;_ShimmerColor;Shimmer Color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0.5960785,1.207843,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;81;31.8949,-505.6645;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;75;-643.6158,261.6767;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;-487.9715,-750.7066;Float;False;Property;_ActiveColor;Active Color;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.0202019,0.01401744,0.1415094,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;14.75647,-150.9986;Float;False;Property;_TransitionState;Transition State;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;1.983427,-29.34471;Float;True;Property;_TriggerMap;Trigger Map;3;0;Create;True;0;0;False;0;None;3b7e16ba77998431581f2ffd2f5a036b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-495.0202,-939.8713;Float;False;Property;_RestingColor;Resting Color;0;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0.945098;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;67;-460.1054,153.2697;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-0.06;False;4;FLOAT;0.06;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;349.44,-3.689739;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;65;-477.6974,375.6402;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;309.4696,-543.5373;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;26;-186.1214,-809.6535;Float;False;Property;_isCollide;isCollide;6;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;56;-613.2604,-1111.074;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;625.9278,-570.0176;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;64;546.75,-773.0884;Float;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1721.497,-403.9798;Float;True;Constant;_Clip;Clip;2;1;[HideInInspector];Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;55;-762.5229,-1128.194;Float;False;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;53;-1186.772,-1144.988;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;20;673.3834,-151.5959;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-40.89135,348.9874;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;54;-933.1919,-1126.756;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1386.568,-1143.88;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;37;1204.828,-559.1193;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Trigger/RealGoopOpaque;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;3;Custom;0.2;True;False;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Legacy Shaders/VertexLit;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;62;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;6;0
WireConnection;8;1;63;0
WireConnection;7;0;6;0
WireConnection;7;1;63;0
WireConnection;4;1;8;0
WireConnection;3;1;7;0
WireConnection;10;0;3;1
WireConnection;10;1;4;2
WireConnection;10;2;13;0
WireConnection;76;0;70;1
WireConnection;58;1;42;0
WireConnection;15;1;10;0
WireConnection;74;0;63;0
WireConnection;74;1;76;0
WireConnection;59;1;58;0
WireConnection;79;0;74;0
WireConnection;80;0;59;0
WireConnection;80;1;15;0
WireConnection;81;0;80;0
WireConnection;75;0;79;0
WireConnection;67;0;75;0
WireConnection;19;0;18;0
WireConnection;19;1;1;4
WireConnection;84;0;2;0
WireConnection;84;1;81;0
WireConnection;26;0;17;0
WireConnection;26;1;25;0
WireConnection;56;0;55;0
WireConnection;85;0;26;0
WireConnection;85;1;84;0
WireConnection;55;1;54;0
WireConnection;53;0;52;0
WireConnection;20;0;19;0
WireConnection;68;0;67;0
WireConnection;68;1;65;0
WireConnection;54;0;53;0
WireConnection;37;0;64;0
WireConnection;37;2;85;0
WireConnection;37;10;20;0
WireConnection;37;12;68;0
ASEEND*/
//CHKSM=F7A47E43D73AE5EE263DA9D79E73C48B752AE14E