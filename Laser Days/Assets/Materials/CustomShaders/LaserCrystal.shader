// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/LaserCrystal"
{
	Properties
	{
		_Normal("Normal", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "gray" {}
		_ColorLaser("Color Laser", Color) = (0.5,0.5,0.5,0.2)
		_ColorReal("Color Real", Color) = (0.5,0.5,0.5,0.2)
		_InteractColor("Interact Color", Color) = (0.25,0.25,0.25,0.4)
		[HDR]_ShimmerColor("Shimmer Color", Color) = (8,8,8,0)
		[PerRendererData]_TransitionState("Transition State", Range( 0 , 1)) = 1
		_TransitionStateB("Transition State B", Range( 0 , 1)) = 1
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		[HideInInspector]_Clip("Clip", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _ColorReal;
		uniform float4 _ColorLaser;
		uniform float _TransitionStateB;
		uniform float4 _InteractColor;
		uniform half _onHover;
		uniform half _onHold;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float4 _ShimmerColor;
		uniform float _TransitionState;
		uniform float _Clip;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORM182 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), 0.5 );
			o.Normal = NORM182;
			float4 lerpResult171 = lerp( _ColorReal , _ColorLaser , _TransitionStateB);
			float temp_output_169_0 = ( _InteractColor.a * 0.5 );
			float4 lerpResult235 = lerp( lerpResult171 , _InteractColor , ( temp_output_169_0 * _onHover ));
			float4 lerpResult172 = lerp( lerpResult235 , _InteractColor , ( temp_output_169_0 * _onHold ));
			float4 DIFFUSE173 = lerpResult172;
			o.Albedo = DIFFUSE173.rgb;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode2 = tex2D( _EffectMap, uv_EffectMap );
			float4 SHIMCOLOR248 = _ShimmerColor;
			float mulTime244 = _Time.y * 0.2;
			float2 panner216 = ( mulTime244 * float2( 0.1,0.13 ) + i.uv_texcoord);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV245 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode245 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV245, 1.0 ) );
			float4 lerpResult247 = lerp( ( tex2DNode2.r * SHIMCOLOR248 * tex2D( _EffectMap, panner216 ).b * 2.0 ) , SHIMCOLOR248 , fresnelNode245);
			o.Emission = lerpResult247.rgb;
			o.Alpha = 1;
			float TRANS175 = _TransitionState;
			float DISSOLVE178 = tex2DNode2.r;
			clip( step( 1.0 , ( TRANS175 + DISSOLVE178 ) ) - _Clip );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
156;113;1162;807;3986.29;1137.992;3.647351;True;False
Node;AmplifyShaderEditor.ColorNode;168;-1676.749,-908.2079;Float;False;Property;_InteractColor;Interact Color;4;0;Create;True;0;0;False;0;0.25,0.25,0.25,0.4;0.2499998,0.2499998,0.2499998,0.4;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;237;-1674.231,-735.5812;Float;False;Constant;_float0;float 0;15;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;207;-2234.643,-703.2306;Float;False;Property;_TransitionStateB;Transition State B;8;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;205;-2233.577,-1076.185;Float;False;Property;_ColorReal;Color Real;3;0;Create;True;0;0;False;0;0.5,0.5,0.5,0.2;0.5,0.5,0.5,0.2;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;206;-2229.896,-887.77;Float;False;Property;_ColorLaser;Color Laser;2;0;Create;True;0;0;False;0;0.5,0.5,0.5,0.2;0.1494304,0.3451127,0.4339623,0.2;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1442.787,-793.5569;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-1596.251,-648.2032;Half;False;Property;_onHover;onHover;11;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;215;-2826.194,211.9171;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;244;-2824.35,369.64;Float;False;1;0;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1252.737,-647.9957;Half;False;Property;_onHold;onHold;9;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;187;-2603.604,-600.0315;Float;False;Property;_ShimmerColor;Shimmer Color;6;1;[HDR];Create;True;0;0;False;0;8,8,8,0;0.6117647,3.2,0.7058824,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;216;-2499.449,308.7698;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.13;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-1260.936,-862.3613;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2250.506,-271.7219;Float;False;Property;_TransitionState;Transition State;7;1;[PerRendererData];Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2264.982,-175.4278;Float;True;Property;_EffectMap;Effect Map;1;0;Create;True;0;0;False;0;None;14b7ac4998d6f46de830629afdc9dc89;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;171;-1787.052,-1059.662;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;243;-1607.24,167.5789;Float;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;251;-1708.47,50.24562;Float;False;248;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-1064.977,430.546;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;248;-2234.369,-618.0612;Float;False;SHIMCOLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;-1066.094,352.7509;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;175;-1936.83,-282.8758;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;235;-1084.661,-1013.472;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;178;-1939.435,-176.0205;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;217;-2221.949,265.1897;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-924.5787,-692.2678;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;-1436.667,20.4569;Float;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;245;-1499.877,-245.9991;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-2262.521,42.16785;Float;True;Property;_Normal;Normal;0;0;Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.5;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;172;-613.0083,-830.1483;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;250;-1424.362,183.4214;Float;False;248;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-796.0174,353.4375;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;222;-1152.502,1386.054;Float;False;240;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-1419.985,1168.186;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;236;-1166.29,1165.57;Float;True;2;0;FLOAT;0.4;False;1;FLOAT;0.09;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;182;-1962.19,57.97548;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;224;-704.5942,1281.663;Float;False;SHIMMER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;143;-547.7368,351.1989;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;247;-921.3765,-36.58916;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;218;-1769.241,1117.332;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;192;-1441.972,370.0958;Float;False;Constant;_Clip;Clip;13;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-2047.963,702.1029;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.28;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;198;-2061.255,797.8427;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-902.6121,779.6483;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2623.363,829.7236;Float;False;Property;_EdgeThickness;Edge Thickness;5;0;Create;True;0;0;False;0;0.3560508;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;-2426.556,695.2527;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1364.593,732.9705;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1888.066,707.2225;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;204;-2253.572,763.7444;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;180;-2119.175,885.7108;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;201;-1611.763,705.3827;Float;True;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;211;-727.9428,-261.9431;Float;False;181;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-381.8687,-823.6934;Float;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;-907.5472,1284.448;Float;False;5;5;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;INT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;227;-1245.253,1571.118;Float;False;Property;_Shimmer;Shimmer;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-464.8398,-203.6259;Float;False;182;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;240;-925.972,-598.9337;Float;False;HOLD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;-751.6216,773.3905;Float;False;EDGE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;174;-468.0909,-285.1394;Float;False;173;0;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;228;-1119.047,1471.251;Float;False;Property;_Flippable;Flippable;12;0;Create;True;0;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-212.0929,-140.4752;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/LaserCrystal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.27;True;True;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;2;0.7608291,0.9622642,0.1770203,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;192;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;169;0;168;4
WireConnection;169;1;237;0
WireConnection;216;0;215;0
WireConnection;216;1;244;0
WireConnection;239;0;169;0
WireConnection;239;1;165;0
WireConnection;171;0;205;0
WireConnection;171;1;206;0
WireConnection;171;2;207;0
WireConnection;248;0;187;0
WireConnection;175;0;4;0
WireConnection;235;0;171;0
WireConnection;235;1;168;0
WireConnection;235;2;239;0
WireConnection;178;0;2;1
WireConnection;217;1;216;0
WireConnection;170;0;169;0
WireConnection;170;1;167;0
WireConnection;242;0;2;1
WireConnection;242;1;251;0
WireConnection;242;2;217;3
WireConnection;242;3;243;0
WireConnection;172;0;235;0
WireConnection;172;1;168;0
WireConnection;172;2;170;0
WireConnection;73;0;177;0
WireConnection;73;1;179;0
WireConnection;219;0;218;1
WireConnection;236;1;219;0
WireConnection;182;0;157;0
WireConnection;224;0;223;0
WireConnection;143;1;73;0
WireConnection;247;0;242;0
WireConnection;247;1;250;0
WireConnection;247;2;245;0
WireConnection;203;0;176;0
WireConnection;203;1;204;0
WireConnection;198;0;156;0
WireConnection;190;0;151;0
WireConnection;151;0;201;0
WireConnection;151;1;187;0
WireConnection;150;0;203;0
WireConnection;150;1;198;0
WireConnection;150;2;180;0
WireConnection;204;1;156;0
WireConnection;201;0;150;0
WireConnection;173;0;172;0
WireConnection;223;0;187;0
WireConnection;223;1;236;0
WireConnection;223;2;222;0
WireConnection;223;3;228;0
WireConnection;223;4;227;0
WireConnection;240;0;167;0
WireConnection;181;0;190;0
WireConnection;0;0;174;0
WireConnection;0;1;183;0
WireConnection;0;2;247;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=ADE78960C6D2CEA62557DE43CD89B5BD531237A2