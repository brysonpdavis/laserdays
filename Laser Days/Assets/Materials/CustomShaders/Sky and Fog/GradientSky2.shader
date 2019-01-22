// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Skybox/GradientSky2"
{
	Properties
	{
		_RealTop("Real Top", Color) = (0.2735849,0.2735849,0.2735849,0)
		_BottomColor("Bottom Color", Color) = (0,0,0,0)
		_SkyGradientSharpness("Sky Gradient Sharpness", Range( 0 , 20)) = 4.689173
		_SkyIntensity("Sky Intensity", Range( 0 , 5)) = 1
		[HDR]_HaloColor("Halo Color", Color) = (1,1,1,0)
		_HaloSharpness("Halo Sharpness", Range( 1 , 20)) = 1
		_SunColor("Sun Color", Color) = (0,0,0,0)
		_SunVector("Sun Vector", Vector) = (0,0.87,0.33,0)
		_SunIntensity("Sun Intensity", Range( 0 , 5)) = 1
		_SunControlA("Sun Control A", Range( 1 , 100)) = 100
		_SunControlB("Sun Control B", Range( 1 , 100)) = 71.37321
		_HorizonOffset("Horizon Offset", Range( -0.1 , 0.1)) = 0
		_LaserTop("Laser Top", Color) = (0,0,0,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Background"  "Queue" = "Background+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _RealTop;
		uniform float4 _LaserTop;
		uniform float _TransitionState;
		uniform float4 _BottomColor;
		uniform float _HorizonOffset;
		uniform float _SkyGradientSharpness;
		uniform float4 _HaloColor;
		uniform float _HaloSharpness;
		uniform float4 _SunColor;
		uniform float3 _SunVector;
		uniform float _SunControlA;
		uniform float _SunControlB;
		uniform float _SunIntensity;
		uniform half _SkyIntensity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 lerpResult133 = lerp( _RealTop , _LaserTop , _TransitionState);
			float4 appendResult115 = (float4(0.0 , -_HorizonOffset , 0.0 , 0.0));
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult2 = normalize( ase_worldPos );
			float WorldY49 = ( appendResult115 + float4( normalizeResult2 , 0.0 ) ).y;
			float4 lerpResult41 = lerp( lerpResult133 , _BottomColor , saturate( pow( ( 1.0 - WorldY49 ) , _SkyGradientSharpness ) ));
			float4 Sky78 = lerpResult41;
			float temp_output_117_0 = saturate( pow( ( 1.0 - saturate( abs( WorldY49 ) ) ) , _HaloSharpness ) );
			float4 Halo79 = ( _HaloColor * temp_output_117_0 );
			float3 normalizeResult130 = normalize( _SunVector );
			float dotResult95 = dot( normalizeResult2 , normalizeResult130 );
			float SunPos96 = dotResult95;
			o.Emission = ( ( Sky78 + float4( 0,0,0,0 ) + Halo79 + ( _SunColor * min( ( pow( max( SunPos96 , 0.0 ) , ( _SunControlA * 100.0 ) ) * _SunControlB ) , 1.0 ) * _SunIntensity ) ) * _SkyIntensity ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;45;1680;953;422.5946;945.3963;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;110;-3084.523,-1300.568;Float;False;Property;_HorizonOffset;Horizon Offset;12;0;Create;True;0;0;False;0;0;-0.1;-0.1;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-3112.157,-1055.987;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NegateNode;129;-2799.088,-1297.136;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;2;-2858.225,-1089.894;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;115;-2618.373,-1238.753;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;116;-2446.965,-1235.167;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector3Node;93;-2775.492,-861.733;Float;True;Property;_SunVector;Sun Vector;8;0;Create;True;0;0;False;0;0,0.87,0.33;2.16,2.8,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;3;-2282.125,-1240.794;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;68;-1119.606,78.37473;Float;False;49;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-2020.094,-1239.874;Float;True;WorldY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;130;-2504.24,-878.8921;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;95;-2223.111,-962.7363;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;69;-920.896,87.84996;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;-1905.402,-691.7876;Float;True;SunPos;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-352.0833,885.7617;Float;False;Property;_SunControlA;Sun Control A;10;0;Create;True;0;0;False;0;100;1;1;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-615.4144,599.7195;Float;False;96;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-891.8945,-889.8886;Float;False;49;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;92;-750.7833,94.79424;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;76;-557.5315,94.54079;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;97;-209.4307,601.2362;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;-33.27732,869.2548;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-679.3413,238.561;Float;False;Property;_HaloSharpness;Halo Sharpness;6;0;Create;True;0;0;False;0;1;1;1;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1147.213,-764.4108;Float;False;Property;_SkyGradientSharpness;Sky Gradient Sharpness;3;0;Create;True;0;0;False;0;4.689173;3;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;51;-475.4313,-942.4564;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-274.4993,1080.857;Float;False;Property;_SunControlB;Sun Control B;11;0;Create;True;0;0;False;0;71.37321;1;1;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;132;-1187.197,-1401.334;Float;False;Property;_LaserTop;Laser Top;13;0;Create;True;0;0;False;0;0,0,0,0;0.7735849,0.7215161,0.3685475,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;73;-353.6084,97.3737;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;45;-294.955,-970.8338;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;24;-1150.435,-1111.932;Float;False;Property;_RealTop;Real Top;1;0;Create;True;0;0;False;0;0.2735849,0.2735849,0.2735849,0;0.5380029,0.6335778,0.735849,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;134;-1259.383,-1224.944;Float;False;Property;_TransitionState;Transition State;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;100;144.4644,599.7709;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;418.1245,678.3202;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;91;-141.2325,-979.6241;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;555.9952,921.0212;Float;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;117;-54.86133,257.8809;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;72;-327.872,-63.96326;Float;False;Property;_HaloColor;Halo Color;5;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;133;-803.4349,-1438.243;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;61;-812.4371,-513.0272;Float;False;Property;_BottomColor;Bottom Color;2;0;Create;True;0;0;False;0;0,0,0,0;0.735849,0.735849,0.735849,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;150.0488,191.4732;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;104;726.1245,749.3202;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;107;652.3777,547.0274;Float;False;Property;_SunColor;Sun Color;7;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;108;587.1873,1029.438;Float;False;Property;_SunIntensity;Sun Intensity;9;0;Create;True;0;0;False;0;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;41;68.20956,-1051.874;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;1077.746,788.2324;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;122;456.1044,-279.2643;Float;False;79;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;791.3052,-945.2557;Float;False;Sky;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;365.1848,240.9206;Float;False;Halo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;462.9805,-468.8643;Float;False;78;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;887.3282,-314.8864;Half;False;Property;_SkyIntensity;Sky Intensity;4;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;754.7094,-447.9663;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;214.7873,360.418;Float;False;HaloMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;1260.688,-397.2212;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;65;-670.3911,-298.1276;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-859.7717,-301.1729;Float;False;-1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-525.363,-397.4378;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;457.4121,-376.4759;Float;False;77;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-279.7518,-395.1588;Float;False;Bottom;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;128;1852.894,-505.4773;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Skybox/GradientSky2;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;False;False;False;Off;2;False;-1;0;False;-1;False;20;False;-1;0;False;-1;False;0;Custom;0;True;False;0;True;Background;;Background;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;129;0;110;0
WireConnection;2;0;1;0
WireConnection;115;1;129;0
WireConnection;116;0;115;0
WireConnection;116;1;2;0
WireConnection;3;0;116;0
WireConnection;49;0;3;1
WireConnection;130;0;93;0
WireConnection;95;0;2;0
WireConnection;95;1;130;0
WireConnection;69;0;68;0
WireConnection;96;0;95;0
WireConnection;92;0;69;0
WireConnection;76;0;92;0
WireConnection;97;0;98;0
WireConnection;131;0;101;0
WireConnection;51;0;50;0
WireConnection;73;0;76;0
WireConnection;73;1;74;0
WireConnection;45;0;51;0
WireConnection;45;1;46;0
WireConnection;100;0;97;0
WireConnection;100;1;131;0
WireConnection;103;0;100;0
WireConnection;103;1;102;0
WireConnection;91;0;45;0
WireConnection;117;0;73;0
WireConnection;133;0;24;0
WireConnection;133;1;132;0
WireConnection;133;2;134;0
WireConnection;80;0;72;0
WireConnection;80;1;117;0
WireConnection;104;0;103;0
WireConnection;104;1;105;0
WireConnection;41;0;133;0
WireConnection;41;1;61;0
WireConnection;41;2;91;0
WireConnection;109;0;107;0
WireConnection;109;1;104;0
WireConnection;109;2;108;0
WireConnection;78;0;41;0
WireConnection;79;0;80;0
WireConnection;85;0;82;0
WireConnection;85;2;122;0
WireConnection;85;3;109;0
WireConnection;118;0;117;0
WireConnection;90;0;85;0
WireConnection;90;1;35;0
WireConnection;65;0;66;0
WireConnection;67;0;61;0
WireConnection;67;1;65;0
WireConnection;77;0;67;0
WireConnection;128;2;90;0
ASEEND*/
//CHKSM=502709EB1E9359A2C5EBE3A093BF875A1802522F