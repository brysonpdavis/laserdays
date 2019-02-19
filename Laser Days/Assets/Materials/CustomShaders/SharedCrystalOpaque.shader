// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/CrystalCoreOpaque"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		_colorful("colorful", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow exclude_path:forward 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _Color;
		uniform sampler2D _colorful;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Color.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV288 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode288 = ( 0.0 + 1.3 * pow( 1.0 - fresnelNdotV288, 1.0 ) );
			float3 normalizeResult319 = normalize( ase_worldNormal );
			float4 tex2DNode290 = tex2D( _colorful, normalizeResult319.xy );
			float4 PAL320 = ( tex2DNode290 * 4.0 );
			o.Emission = ( fresnelNode288 * PAL320 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
168;448;1179;828;3149.113;844.2535;1;True;False
Node;AmplifyShaderEditor.WorldNormalVector;295;-3312.845,-622.7229;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;319;-3024.614,-661.8746;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;332;-2483.113,-671.2535;Float;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;290;-2782.945,-727.8611;Float;True;Property;_colorful;colorful;5;0;Create;True;0;0;False;0;None;89258654212924302bc125dd9c07a179;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;331;-2340.113,-770.2535;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;288;-1952.6,-191.8394;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1.3;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;321;-1713.017,151.5802;Float;False;320;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;320;-2172.641,-782.7651;Float;False;PAL;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;322;-1376.075,622.2737;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;197;-1609.538,1036.743;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;143;-1001.275,1054.995;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;325;-1051.263,816.5227;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;324;-1432.358,793.3204;Float;False;Constant;_Vector0;Vector 0;11;0;Create;True;0;0;False;0;0,-1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;309;-2594.062,465.3455;Float;False;Property;_Boost;Boost;9;0;Create;True;0;0;False;0;0;0.81;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-3039.057,-877.1799;Float;False;Property;_TransitionState;Transition State;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;330;-1676.264,711.6781;Float;False;Property;_OffsetMag;OffsetMag;11;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;301;-2779.241,-511.3371;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;265;-1831.882,239.4368;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;314;-1395.986,-2.126298;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-2171.52,377.9096;Float;False;Property;_Shimmer;Shimmer;10;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;317;-2561.696,-507.1378;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;236;-1412.379,1037.11;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;306;-2565.801,-399.9937;Float;False;Property;_ShineBlend;Shine Blend;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;326;-857.5228,544.8295;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;323;-1025.278,603.0876;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;283;-1098.229,344.5551;Float;False;Property;_Opacity;Opacity;7;0;Create;True;0;0;False;0;0.5;0.654;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;300;-3074.791,-522.4196;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;249;-935.019,-50.071;Float;False;Property;_Color;Color;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;262;-2267.803,220.0709;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;308;-2930.56,-502.0296;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2610.575,57.191;Float;True;Property;_EffectMap;Effect Map;1;0;Create;True;0;0;False;0;None;14b7ac4998d6f46de830629afdc9dc89;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1237.671,1098.928;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-1593.467,1142.776;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;287;-3168.407,466.8228;Float;False;1;0;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;258;-3129.511,261.0578;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-2228.354,-616.8446;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2734.309,-883.9003;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2305.516,27.5541;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;260;-2607.368,259.5178;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;209;-1955.151,591.752;Float;False;Property;_ShimmerColor;Shimmer Color;2;1;[HDR];Create;True;0;0;False;0;8,8,8,0;1.45098,1.984314,1.803922,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;311;-3289.593,-468.0421;Float;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;294;-1103.469,249.1055;Float;False;Property;_Refraction;Refraction;6;0;Create;True;0;0;False;0;0;0.344;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;259;-2869.866,300.5688;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.13;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-621.3309,110.8753;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/CrystalCoreOpaque;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;DeferredOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;319;0;295;0
WireConnection;290;1;319;0
WireConnection;331;0;290;0
WireConnection;331;1;332;0
WireConnection;320;0;331;0
WireConnection;143;1;73;0
WireConnection;325;0;324;0
WireConnection;325;1;197;0
WireConnection;325;2;330;0
WireConnection;301;0;308;0
WireConnection;265;0;262;0
WireConnection;265;1;273;0
WireConnection;314;0;288;0
WireConnection;314;1;321;0
WireConnection;317;0;301;0
WireConnection;236;0;197;0
WireConnection;326;0;323;0
WireConnection;323;0;322;0
WireConnection;323;1;197;0
WireConnection;323;2;330;0
WireConnection;300;0;295;0
WireConnection;300;1;311;0
WireConnection;262;0;2;1
WireConnection;262;1;260;2
WireConnection;262;2;309;0
WireConnection;308;0;300;0
WireConnection;73;0;236;0
WireConnection;73;1;203;0
WireConnection;302;0;290;0
WireConnection;302;1;317;0
WireConnection;302;2;306;0
WireConnection;192;0;4;0
WireConnection;199;0;2;1
WireConnection;260;1;259;0
WireConnection;259;0;258;0
WireConnection;259;1;287;0
WireConnection;0;0;249;0
WireConnection;0;2;314;0
ASEEND*/
//CHKSM=11A2C7403607061C691D536D83FEA9452A86513A