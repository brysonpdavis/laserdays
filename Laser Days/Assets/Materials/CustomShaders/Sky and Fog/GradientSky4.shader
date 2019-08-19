// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Skybox/GradientSky4"
{
	Properties
	{
		_RealTop("Real Top", Color) = (0.2735849,0.2735849,0.2735849,0)
		_SkyIntensity("Sky Intensity", Range( 0 , 5)) = 1
		[HDR]_HaloColor("Halo Color", Color) = (1,1,1,0)
		_HaloSharpness("Halo Sharpness", Range( 1 , 20)) = 1
		_HorizonOffset("Horizon Offset", Range( -0.1 , 0.1)) = 0
		_LaserTop("Laser Top", Color) = (0,0,0,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_CloudTexture("CloudTexture", CUBE) = "white" {}
		[HDR]_LightCloudColor("LightCloudColor", Color) = (0,0,0,0)
		_DarkCloudColor("DarkCloudColor", Color) = (0,0,0,0)
		_PlainSkyStepValue("PlainSkyStepValue", Range( 0 , 1)) = 0
		_LightDarkPoint("LightDarkPoint", Float) = 0
		_SecondaryPanningSpeed("Secondary PanningSpeed", Range( -1 , 0)) = 0
		_MainPanningSpeed("Main Panning Speed", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Background"  "Queue" = "Background+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _RealTop;
		uniform float4 _LaserTop;
		uniform float _TransitionState;
		uniform float4 _HaloColor;
		uniform float _HorizonOffset;
		uniform float _HaloSharpness;
		uniform half _SkyIntensity;
		uniform float4 _DarkCloudColor;
		uniform float4 _LightCloudColor;
		uniform samplerCUBE _CloudTexture;
		uniform float _MainPanningSpeed;
		uniform float _SecondaryPanningSpeed;
		uniform float _LightDarkPoint;
		uniform float _PlainSkyStepValue;


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 lerpResult133 = lerp( _RealTop , _LaserTop , _TransitionState);
			float4 Sky78 = lerpResult133;
			float4 appendResult115 = (float4(0.0 , -_HorizonOffset , 0.0 , 0.0));
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult2 = normalize( ase_worldPos );
			float WorldY49 = ( appendResult115 + float4( normalizeResult2 , 0.0 ) ).y;
			float temp_output_117_0 = saturate( pow( ( 1.0 - saturate( abs( WorldY49 ) ) ) , _HaloSharpness ) );
			float4 Halo79 = ( _HaloColor * temp_output_117_0 );
			float3 rotatedValue160 = RotateAroundAxis( float3( 0,0,0 ), ase_worldPos, float3( 0,1,0 ), ( _Time.y * _MainPanningSpeed ) );
			float3 rotatedValue167 = RotateAroundAxis( float3( 0,0,0 ), ase_worldPos, float3( 0,1,0 ), ( _Time.y * _SecondaryPanningSpeed ) );
			float TRANS162 = _TransitionState;
			float lerpResult172 = lerp( ( texCUBE( _CloudTexture, rotatedValue160 ).r * texCUBE( _CloudTexture, rotatedValue167 ).g ) , 1.0 , TRANS162);
			float Clouds138 = lerpResult172;
			float4 lerpResult147 = lerp( _DarkCloudColor , _LightCloudColor , saturate( ( Clouds138 + _LightDarkPoint ) ));
			float smoothstepResult148 = smoothstep( 0.0 , _PlainSkyStepValue , Clouds138);
			float4 lerpResult151 = lerp( lerpResult147 , float4( 1,1,1,0 ) , smoothstepResult148);
			o.Emission = ( ( Sky78 + Halo79 ) * _SkyIntensity * lerpResult151 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
234;130;1162;807;5181.352;2610.843;7.481684;True;False
Node;AmplifyShaderEditor.RangedFloatNode;110;-1369.784,-973.382;Float;False;Property;_HorizonOffset;Horizon Offset;5;0;Create;True;0;0;False;0;0;-0.037;-0.1;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-1397.419,-728.801;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NegateNode;129;-1084.349,-969.95;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;115;-903.6345,-911.567;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NormalizeNode;2;-1143.487,-762.708;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;116;-732.2267,-907.981;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;164;-1611.341,61.60717;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-1795.763,589.6929;Float;False;Property;_SecondaryPanningSpeed;Secondary PanningSpeed;13;0;Create;True;0;0;False;0;0;-0.005;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;166;-1744.006,162.4324;Float;False;Property;_MainPanningSpeed;Main Panning Speed;14;0;Create;True;0;0;False;0;0;0.007;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;168;-1663.098,488.8677;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;3;-567.3865,-913.6079;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1410.151,526.0138;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-1358.394,98.7533;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;157;-1670.126,284.1823;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-305.3557,-912.688;Float;False;WorldY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;167;-1134.464,501.9754;Float;False;False;4;0;FLOAT3;0,1,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;160;-1132.977,326.5742;Float;False;False;4;0;FLOAT3;0,1,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-1424.804,-329.9927;Float;False;49;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;163;-641.4445,560.9758;Float;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;faccccc43d64042a6b0d337a363d00f1;eced2b8ac1f094cb88597059d034114c;True;0;False;white;LockedToCube;False;Instance;137;Auto;Cube;6;0;SAMPLER2D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;134;-1425.914,-1193.488;Float;False;Property;_TransitionState;Transition State;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;137;-639.8505,342.4093;Float;True;Property;_CloudTexture;CloudTexture;8;0;Create;True;0;0;False;0;faccccc43d64042a6b0d337a363d00f1;d592b180643b14c73abc725e4397b512;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;6;0;SAMPLER2D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;69;-1226.094,-323.1977;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-155.0996,508.2324;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;92;-1055.981,-316.2534;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;162;-993.9954,-1192.234;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-173.8463,637.1113;Float;False;162;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-984.5389,-172.4867;Float;False;Property;_HaloSharpness;Halo Sharpness;4;0;Create;True;0;0;False;0;1;3.68;1;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;76;-862.7295,-316.5069;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;172;53.07119,544.738;Float;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;73;-658.8065,-313.674;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;376.7785,-947.5286;Float;True;138;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;646.1823,-1048.236;Float;False;Property;_LightDarkPoint;LightDarkPoint;12;0;Create;True;0;0;False;0;0;0.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;199.3225,461.3569;Float;False;Clouds;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;117;-360.0592,-153.1669;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;72;-633.0702,-475.011;Float;False;Property;_HaloColor;Halo Color;3;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0.7169812,0.5715557,0.6650435,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;132;-1372.231,-1379.13;Float;False;Property;_LaserTop;Laser Top;6;0;Create;True;0;0;False;0;0,0,0,0;0.5582461,0.5353399,0.58,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;154;868.5117,-1063.032;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;24;-1385.428,-1568.966;Float;False;Property;_RealTop;Real Top;1;0;Create;True;0;0;False;0;0.2735849,0.2735849,0.2735849,0;0.411764,0.6053513,0.7921569,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;141;1048.903,-754.6825;Float;False;Property;_LightCloudColor;LightCloudColor;9;1;[HDR];Create;True;0;0;False;0;0,0,0,0;2.121562,2.121562,2.121562,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-155.149,-219.5745;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;142;1041.38,-931.7888;Float;False;Property;_DarkCloudColor;DarkCloudColor;10;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;150;518.1229,-489.4324;Float;False;Property;_PlainSkyStepValue;PlainSkyStepValue;11;0;Create;True;0;0;False;0;0;0.27;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;146;1110.109,-1047.018;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;133;-803.4349,-1438.243;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-501.3434,-1432.153;Float;False;Sky;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;1093.884,-1594.185;Float;False;78;0;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;148;866.9818,-529.9415;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;59.98701,-170.1272;Float;False;Halo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;147;1285.781,-1029.665;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;122;1099.272,-1504.191;Float;False;79;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;1188.71,-1288.133;Half;False;Property;_SkyIntensity;Sky Intensity;2;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;85;1384.397,-1563.555;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;151;1463.679,-656.7881;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;174;-1660.995,-14.29969;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;152;1222.772,-453.8426;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;-90.4105,-50.62975;Float;False;HaloMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;-1283.085,666.8749;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;1615.114,-1229.806;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;-1367.974,816.2853;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinTimeNode;177;-1557.011,852.2289;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;175;-1571.433,1092.931;Float;False;Constant;_Vector0;Vector 0;15;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;178;-1622.576,993.4319;Float;False;Constant;_Float0;Float 0;16;0;Create;True;0;0;False;0;2000;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;128;1901.734,-1324.903;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Skybox/GradientSky4;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;False;False;False;Off;2;False;-1;0;False;-1;False;20;False;-1;0;False;-1;False;0;Custom;0;True;False;0;True;Background;;Background;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;129;0;110;0
WireConnection;115;1;129;0
WireConnection;2;0;1;0
WireConnection;116;0;115;0
WireConnection;116;1;2;0
WireConnection;3;0;116;0
WireConnection;169;0;168;0
WireConnection;169;1;170;0
WireConnection;165;0;164;0
WireConnection;165;1;166;0
WireConnection;49;0;3;1
WireConnection;167;1;169;0
WireConnection;167;3;157;0
WireConnection;160;1;165;0
WireConnection;160;3;157;0
WireConnection;163;1;167;0
WireConnection;137;1;160;0
WireConnection;69;0;68;0
WireConnection;171;0;137;1
WireConnection;171;1;163;2
WireConnection;92;0;69;0
WireConnection;162;0;134;0
WireConnection;76;0;92;0
WireConnection;172;0;171;0
WireConnection;172;2;173;0
WireConnection;73;0;76;0
WireConnection;73;1;74;0
WireConnection;138;0;172;0
WireConnection;117;0;73;0
WireConnection;154;0;139;0
WireConnection;154;1;155;0
WireConnection;80;0;72;0
WireConnection;80;1;117;0
WireConnection;146;0;154;0
WireConnection;133;0;24;0
WireConnection;133;1;132;0
WireConnection;133;2;134;0
WireConnection;78;0;133;0
WireConnection;148;0;139;0
WireConnection;148;2;150;0
WireConnection;79;0;80;0
WireConnection;147;0;142;0
WireConnection;147;1;141;0
WireConnection;147;2;146;0
WireConnection;85;0;82;0
WireConnection;85;1;122;0
WireConnection;151;0;147;0
WireConnection;151;2;148;0
WireConnection;152;0;148;0
WireConnection;118;0;117;0
WireConnection;176;0;157;0
WireConnection;176;1;179;0
WireConnection;90;0;85;0
WireConnection;90;1;35;0
WireConnection;90;2;151;0
WireConnection;179;0;177;1
WireConnection;179;1;178;0
WireConnection;179;2;175;0
WireConnection;128;2;90;0
ASEEND*/
//CHKSM=649CF315D59C9546C868CE6FF3FFD28A99A8FD3B