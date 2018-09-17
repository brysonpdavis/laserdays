// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/RealOnlyCel"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_ColorReal("Color Real", Color) = (0,0,0,0)
		_MaterialMapReal("Material Map Real", 2D) = "white" {}
		_EffectMap("Effect Map", 2D) = "white" {}
		_HoverColor("Hover Color", Color) = (0,0,0,0)
		_HoldColor("Hold Color", Color) = (0,0,0,0)
		[HDR]_DissolveEdgeColor("Dissolve Edge Color", Color) = (1,1,1,0)
		_DissolveEdgeThickness("Dissolve Edge Thickness", Range( 0 , 0.5)) = 0.3560508
		_ToonRamp("ToonRamp", 2D) = "white" {}
		[HDR]_SelectedEffectColor("Selected Effect Color", Color) = (8,8,8,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0.8157373
		_ShadowBoost("Shadow Boost", Range( 0 , 1)) = 0
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		LOD 100
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
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
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _TransitionState;
		uniform float _DissolveEdgeThickness;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float4 _DissolveEdgeColor;
		uniform float4 _SelectedEffectColor;
		uniform float4 _HoverColor;
		uniform half _onHover;
		uniform sampler2D _ToonRamp;
		uniform float _ShadowBoost;
		uniform float4 _ColorReal;
		uniform sampler2D _MaterialMapReal;
		uniform float4 _MaterialMapReal_ST;
		uniform float4 _HoldColor;
		uniform half _onHold;
		uniform float _Cutoff = 0.5;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float TRANS192 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode2 = tex2D( _EffectMap, uv_EffectMap );
			float DISSOLVE199 = tex2DNode2.r;
			float MASK255 = step( 0.0 , ( -TRANS192 + DISSOLVE199 ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult259 = dot( ase_worldNormal , ase_worldlightDir );
			float2 temp_cast_2 = ((dotResult259*0.5 + 0.5)).xx;
			float3 hsvTorgb250 = RGBToHSV( _ColorReal.rgb );
			float2 uv_MaterialMapReal = i.uv_texcoord * _MaterialMapReal_ST.xy + _MaterialMapReal_ST.zw;
			float3 hsvTorgb252 = HSVToRGB( float3(hsvTorgb250.x,hsvTorgb250.y,( hsvTorgb250.z * tex2D( _MaterialMapReal, uv_MaterialMapReal ).r )) );
			float4 lerpResult184 = lerp( float4( hsvTorgb252 , 0.0 ) , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 DIFFUSE187 = lerpResult184;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			UnityGI gi265 = gi;
			float3 diffNorm265 = ase_worldNormal;
			gi265 = UnityGI_Base( data, 1, diffNorm265 );
			float3 indirectDiffuse265 = gi265.indirect.diffuse + diffNorm265 * 0.0001;
			c.rgb = ( saturate( ( tex2D( _ToonRamp, temp_cast_2 ) + _ShadowBoost ) ) * DIFFUSE187 * ( ase_lightColor * float4( ( indirectDiffuse265 + ase_lightAtten ) , 0.0 ) ) ).rgb;
			c.a = 1;
			clip( MASK255 - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float TRANS192 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode2 = tex2D( _EffectMap, uv_EffectMap );
			float DISSOLVE199 = tex2DNode2.r;
			float4 Temp212 = ( float4( 0,0,0,0 ) * _SelectedEffectColor );
			float4 EDGE202 = ( ( step( ( ( -TRANS192 * ( _DissolveEdgeThickness + 1.0 ) ) + -_DissolveEdgeThickness + DISSOLVE199 ) , -_DissolveEdgeThickness ) * _DissolveEdgeColor ) + Temp212 );
			float4 temp_cast_0 = (0).xxxx;
			float4 lerpResult165 = lerp( temp_cast_0 , _HoverColor , ( _onHover * _HoverColor.a ));
			float4 HOVER274 = lerpResult165;
			o.Emission = ( EDGE202 + HOVER274 ).rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

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
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
Version=15600
44;44;1636;934;2689.796;1611.484;1.644718;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2187.757,-92.30191;Float;False;Property;_TransitionState;Transition State;11;0;Create;True;0;0;False;0;0.8157373;0.222;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;228;-1889.959,1177.93;Float;False;192;TRANS;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-1909.261,-94.02967;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;227;-2165.316,1418.973;Float;False;Property;_DissolveEdgeThickness;Dissolve Edge Thickness;8;0;Create;True;0;0;False;0;0.3560508;0.301;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;244;-1697.584,1178.483;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2184.714,298.0259;Float;True;Property;_EffectMap;Effect Map;4;0;Create;True;0;0;False;0;None;4213667ef65704f73b4663308f4e1b31;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;246;-1844.139,1274.701;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;258;-663.4705,88.88606;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NegateNode;245;-1656.34,1317.673;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;248;-2165.352,-1334.275;Float;False;Property;_ColorReal;Color Real;1;0;Create;True;0;0;False;0;0,0,0,0;0.6528543,0.6698113,0.116901,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-1871.504,307.1407;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;-1556.283,1180.801;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;257;-620.4433,-62.30478;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;231;-1693.343,1403.877;Float;False;199;DISSOLVE;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;259;-383.0721,-55.91293;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;249;-2249.227,-1165.674;Float;True;Property;_MaterialMapReal;Material Map Real;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RGBToHSVNode;250;-1956.226,-1333.673;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;233;-1387.27,1179.534;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;209;-1180.852,1785.646;Float;False;Property;_SelectedEffectColor;Selected Effect Color;10;1;[HDR];Create;True;0;0;False;0;8,8,8,0;8,8,8,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;260;-412.0543,-150.2271;Float;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;152;-1210.398,1449.594;Float;False;Property;_DissolveEdgeColor;Dissolve Edge Color;7;1;[HDR];Create;True;0;0;False;0;1,1,1,0;4,4,4,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;167;-2226.122,-460.606;Half;False;Property;_onHover;onHover;14;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;181;-1568.648,-1148.464;Float;False;Property;_HoldColor;Hold Color;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-1728.518,-1227.846;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-1636.873,-956.6011;Half;False;Property;_onHold;onHold;13;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;234;-1213.816,1219.068;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-914.3616,1774.066;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;261;-196.9003,-140.9387;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;159;-2156.435,-646.1703;Float;False;Property;_HoverColor;Hover Color;5;0;Create;True;0;0;False;0;0,0,0,0;1,0.6221464,0,0.2156863;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;197;-1341.55,612.7163;Float;False;192;TRANS;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-805.0779,1251.004;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-1802.81,-537.1121;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;264;30.72893,-207.3148;Float;True;Property;_ToonRamp;ToonRamp;9;0;Create;True;0;0;False;0;None;4a056241e2722dc46a7262a8e7073fd9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.HSVToRGBNode;252;-1583.226,-1309.674;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1337.042,-997.6908;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;236;-1144.391,613.0831;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-724.3278,1773.578;Float;False;Temp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;262;31.36296,-283.8892;Float;False;Property;_ShadowBoost;Shadow Boost;12;0;Create;True;0;0;False;0;0;0.398;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-1325.479,718.7487;Float;False;199;DISSOLVE;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;265;-264.3373,248.9848;Float;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;253;-2157.325,-818.4759;Float;False;Constant;_Black;Black;13;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;214;-536.0238,1422.437;Float;False;212;Temp;0;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;263;-241.9303,325.9048;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;266;-197.6693,125.9303;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;184;-1238.35,-1209.832;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;267;360.363,-231.8892;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;268;53.30096,255.3887;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-969.6831,674.9008;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;165;-1614.587,-667.4284;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;-353.6458,1370.485;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;202;-130.5568,1361.456;Float;False;EDGE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;269;514.3629,-228.8892;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;143;-733.2874,671.4636;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;274;-1419.7,-665.4682;Float;False;HOVER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-948.504,-1313.767;Float;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;271;304.8269,124.2384;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;270;396.6489,-11.80645;Float;False;187;DIFFUSE;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;526.1768,-791.3524;Float;True;202;EDGE;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;276;538.4359,-590.0571;Float;True;274;HOVER;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;157;-2187.5,93.45151;Float;True;Property;_NormalMap;Normal Map;3;0;Create;True;0;0;False;0;None;1a7ed036f24cd9f4895e6d9870d7a171;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;256;-843.5694,-42.21655;Float;False;195;NORM;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;-1869.666,384.449;Float;False;SELECT;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;277;882.4346,-555.9976;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;-1896.204,89.83661;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;273;-478.7562,249.9078;Float;False;195;NORM;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;275;1508.471,8.724857;Float;False;255;MASK;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;272;753.6694,-50.01495;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;206;-1676.061,1066.297;Float;False;Constant;_Clip;Clip;12;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;255;-507.8346,656.6758;Float;False;MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1739.549,-138.1903;Float;False;True;2;Float;ASEMaterialInspector;100;0;CustomLighting;Transition/RealOnlyCel;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;100;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;206;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;4;0
WireConnection;244;0;228;0
WireConnection;246;0;227;0
WireConnection;245;0;227;0
WireConnection;199;0;2;1
WireConnection;247;0;244;0
WireConnection;247;1;246;0
WireConnection;259;0;257;0
WireConnection;259;1;258;0
WireConnection;250;0;248;0
WireConnection;233;0;247;0
WireConnection;233;1;245;0
WireConnection;233;2;231;0
WireConnection;251;0;250;3
WireConnection;251;1;249;1
WireConnection;234;0;233;0
WireConnection;234;1;245;0
WireConnection;210;1;209;0
WireConnection;261;0;259;0
WireConnection;261;1;260;0
WireConnection;261;2;260;0
WireConnection;151;0;234;0
WireConnection;151;1;152;0
WireConnection;168;0;167;0
WireConnection;168;1;159;4
WireConnection;264;1;261;0
WireConnection;252;0;250;1
WireConnection;252;1;250;2
WireConnection;252;2;251;0
WireConnection;186;0;181;4
WireConnection;186;1;183;0
WireConnection;236;0;197;0
WireConnection;212;0;210;0
WireConnection;184;0;252;0
WireConnection;184;1;181;0
WireConnection;184;2;186;0
WireConnection;267;0;264;0
WireConnection;267;1;262;0
WireConnection;268;0;265;0
WireConnection;268;1;263;0
WireConnection;73;0;236;0
WireConnection;73;1;203;0
WireConnection;165;0;253;1
WireConnection;165;1;159;0
WireConnection;165;2;168;0
WireConnection;213;0;151;0
WireConnection;213;1;214;0
WireConnection;202;0;213;0
WireConnection;269;0;267;0
WireConnection;143;1;73;0
WireConnection;274;0;165;0
WireConnection;187;0;184;0
WireConnection;271;0;266;0
WireConnection;271;1;268;0
WireConnection;254;0;2;2
WireConnection;277;0;205;0
WireConnection;277;1;276;0
WireConnection;195;0;157;0
WireConnection;272;0;269;0
WireConnection;272;1;270;0
WireConnection;272;2;271;0
WireConnection;255;0;143;0
WireConnection;0;2;277;0
WireConnection;0;10;275;0
WireConnection;0;13;272;0
ASEEND*/
//CHKSM=9A2E218F81E735ACB8AEBE111B94FB889C382224