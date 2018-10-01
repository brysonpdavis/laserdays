// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Trigger/WaterReal"
{
	Properties
	{
		[HDR]_RestingColor("Resting Color", Color) = (0,0,0,0)
		[HDR]_ShimmerColor("Shimmer Color", Color) = (0,0,0,0)
		[HideInInspector]_Clip("Clip", Float) = 0.5
		[HDR]_ActiveColor("Active Color", Color) = (0,0,0,0)
		_TriggerMap("Trigger Map", 2D) = "white" {}
		[Toggle]_UseFresnel("UseFresnel", Float) = 0
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		[Toggle]_isCollide("isCollide", Float) = 0
		[HideInInspector]_Elapsed("Elapsed", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _isCollide;
		uniform float4 _RestingColor;
		uniform float4 _ActiveColor;
		uniform float4 _ShimmerColor;
		uniform float _Elapsed;
		uniform float _UseFresnel;
		uniform float _TransitionState;
		uniform sampler2D _TriggerMap;
		uniform float4 _TriggerMap_ST;
		uniform float _Clip;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 panner7 = ( _Elapsed * float2( 0.1,0.1 ) + i.uv_texcoord);
			float2 panner8 = ( _Elapsed * float2( -0.03,0.2 ) + i.uv_texcoord);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV42 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode42 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV42, 0.96 ) );
			o.Emission = ( lerp(_RestingColor,_ActiveColor,_isCollide) + ( _ShimmerColor * step( 0.19 , ( tex2D( _TriggerMap, panner7 ).r * tex2D( _TriggerMap, panner8 ).g * 1.77 ) ) ) + ( lerp(0.0,step( 0.5 , fresnelNode42 ),_UseFresnel) * _ShimmerColor ) ).rgb;
			o.Alpha = 1;
			float2 uv_TriggerMap = i.uv_texcoord * _TriggerMap_ST.xy + _TriggerMap_ST.zw;
			clip( step( ( _TransitionState + tex2D( _TriggerMap, uv_TriggerMap ).a ) , 1.0 ) - _Clip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
363;67;1557;933;2479.553;1926.532;4.016307;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1726.388,-161.7387;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;-1800.898,-28.8858;Float;False;Property;_Elapsed;Elapsed;7;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;7;-1397.913,-227.0347;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;8;-1394.541,-81.4259;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-993.2715,120.9051;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;1.77;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-1149.749,-79.26834;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;42;-1203.507,-781.4529;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.96;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1149.749,-275.2684;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;58;-866.283,-697.5651;Float;True;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-733.384,-265.4219;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-480.8776,-451.3679;Float;False;Property;_ShimmerColor;Shimmer Color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.172549,0.3921569,1.247059,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-495.0202,-939.8713;Float;False;Property;_RestingColor;Resting Color;0;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.07208972,0.205481,0.3396226,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;14.75647,-150.9986;Float;False;Property;_TransitionState;Transition State;5;0;Create;True;0;0;False;0;1;0.3532375;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;59;-488.5672,-574.373;Float;False;Property;_UseFresnel;UseFresnel;4;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;15;-477.4274,-269.2221;Float;True;2;0;FLOAT;0.19;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;3.726434,-83.37792;Float;True;Property;_TriggerMap;Trigger Map;3;0;Create;True;0;0;False;0;None;3b7e16ba77998431581f2ffd2f5a036b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;25;-487.9715,-750.7066;Float;False;Property;_ActiveColor;Active Color;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.01161443,0.1057469,0.8207547,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;19;379.0711,-169.2754;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-114.4106,-598.9386;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;26;-186.1214,-809.6535;Float;False;Property;_isCollide;isCollide;6;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-134.8374,-403.4639;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;55;-1067.574,-1187.74;Float;False;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-918.3115,-1170.62;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1691.619,-1203.426;Float;False;Constant;_Float2;Float 2;5;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1721.497,-403.9798;Float;True;Constant;_Clip;Clip;2;1;[HideInInspector];Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;53;-1491.823,-1204.534;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;178.6021,-620.9443;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;20;708.1299,-233.52;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;54;-1238.243,-1186.302;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;37;1014.894,-529.5392;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Trigger/WaterReal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;3;Masked;0.2;True;False;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;62;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;6;0
WireConnection;7;1;63;0
WireConnection;8;0;6;0
WireConnection;8;1;63;0
WireConnection;4;1;8;0
WireConnection;3;1;7;0
WireConnection;58;1;42;0
WireConnection;10;0;3;1
WireConnection;10;1;4;2
WireConnection;10;2;13;0
WireConnection;59;1;58;0
WireConnection;15;1;10;0
WireConnection;19;0;18;0
WireConnection;19;1;1;4
WireConnection;60;0;59;0
WireConnection;60;1;2;0
WireConnection;26;0;17;0
WireConnection;26;1;25;0
WireConnection;16;0;2;0
WireConnection;16;1;15;0
WireConnection;55;1;54;0
WireConnection;56;0;55;0
WireConnection;53;0;52;0
WireConnection;12;0;26;0
WireConnection;12;1;16;0
WireConnection;12;2;60;0
WireConnection;20;0;19;0
WireConnection;54;0;53;0
WireConnection;37;2;12;0
WireConnection;37;10;20;0
ASEEND*/
//CHKSM=DD6DE8EF60F71703B193C033FA680E4DDB48CF53