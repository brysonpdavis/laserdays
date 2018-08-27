// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/Selection"
{
	Properties
	{
		_EffectColor("Effect Color", Color) = (0.990566,0.7463643,0,0)
		_Float0("Float 0", Range( 0 , 1)) = 0.5216464
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest Always
		Blend One One
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _EffectColor;
		uniform float _Float0;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV1, 5.0 ) );
			float clampResult42 = clamp( ( pow( fresnelNode1 , 0.4266438 ) + pow( saturate( fmod( ase_worldPos.y , _Float0 ) ) , ( 0.4266438 * 2.0 ) ) ) , 0.0 , 1.0 );
			o.Emission = ( _EffectColor * clampResult42 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
0;45;1680;952;1334.775;-2.518883;1.25706;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;33;-1196.019,590.188;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;38;-976.3048,849.5817;Float;False;Property;_Float0;Float 0;2;0;Create;True;0;0;False;0;0.5216464;0.6809703;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;34;-950.9339,589.6953;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FmodOpNode;36;-566.7972,421.1929;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1168.349,301.8982;Float;False;Constant;_EdgeSharpness;Edge Sharpness;1;0;Create;True;0;0;False;0;0.4266438;0;0.01;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;39;-321.566,411.191;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;1;-979.3295,-62.16164;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-400.7792,201.1343;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;5;-556.8333,-13.03417;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;40;3.43396,241.191;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;272.434,250.1911;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-794.7543,246.3742;Float;False;Property;_EffectColor;Effect Color;1;0;Create;True;0;0;False;0;0.990566,0.7463643,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;42;488.2513,244.2547;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;630.2626,-16.70782;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;958.1733,-103.111;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Transition/Selection;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;7;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;34;0;33;2
WireConnection;36;0;34;0
WireConnection;36;1;38;0
WireConnection;39;0;36;0
WireConnection;44;0;4;0
WireConnection;5;0;1;0
WireConnection;5;1;4;0
WireConnection;40;0;39;0
WireConnection;40;1;44;0
WireConnection;41;0;5;0
WireConnection;41;1;40;0
WireConnection;42;0;41;0
WireConnection;6;0;2;0
WireConnection;6;1;42;0
WireConnection;0;2;6;0
ASEEND*/
//CHKSM=08B01954B186EBFA969644B780888C03CA2BB192