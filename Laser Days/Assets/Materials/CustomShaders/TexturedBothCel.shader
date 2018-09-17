// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/BothTexturedCel"
{
	Properties
	{
		_ColorReal("Color Real", Color) = (0,0,0,0)
		_MaterialMapReal("Material Map Real", 2D) = "white" {}
		_NormalMapReal("Normal Map Real", 2D) = "bump" {}
		_ColorLaser("Color Laser", Color) = (0,0,0,0)
		_MaterialMapLaser("Material Map Laser", 2D) = "white" {}
		_NormalMapLaser("Normal Map Laser", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_NormalScale("Normal Scale", Range( 0 , 10)) = 6.588235
		_ShadowBoost("Shadow Boost", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "Sobel"="True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityStandardUtils.cginc"
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
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
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

		uniform sampler2D _ToonRamp;
		uniform float _NormalScale;
		uniform float _TransitionState;
		uniform sampler2D _NormalMapReal;
		uniform float4 _NormalMapReal_ST;
		uniform sampler2D _NormalMapLaser;
		uniform float4 _NormalMapLaser_ST;
		uniform float _ShadowBoost;
		uniform float4 _ColorLaser;
		uniform sampler2D _MaterialMapLaser;
		uniform float4 _MaterialMapLaser_ST;
		uniform float4 _ColorReal;
		uniform sampler2D _MaterialMapReal;
		uniform float4 _MaterialMapReal_ST;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;


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
			float TRANS230 = _TransitionState;
			float2 uv_NormalMapReal = i.uv_texcoord * _NormalMapReal_ST.xy + _NormalMapReal_ST.zw;
			float2 uv_NormalMapLaser = i.uv_texcoord * _NormalMapLaser_ST.xy + _NormalMapLaser_ST.zw;
			float3 NORM241 = BlendNormals( UnpackScaleNormal( tex2D( _NormalMapReal, uv_NormalMapReal ), ( _NormalScale * ( 1.0 - TRANS230 ) ) ) , UnpackScaleNormal( tex2D( _NormalMapLaser, uv_NormalMapLaser ), ( _NormalScale * TRANS230 ) ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult202 = dot( (WorldNormalVector( i , NORM241 )) , ase_worldlightDir );
			float2 temp_cast_0 = ((dotResult202*0.5 + 0.5)).xx;
			float3 hsvTorgb276 = RGBToHSV( _ColorLaser.rgb );
			float2 uv_MaterialMapLaser = i.uv_texcoord * _MaterialMapLaser_ST.xy + _MaterialMapLaser_ST.zw;
			float3 hsvTorgb277 = HSVToRGB( float3(hsvTorgb276.x,hsvTorgb276.y,( hsvTorgb276.z * tex2D( _MaterialMapLaser, uv_MaterialMapLaser ).r )) );
			float3 hsvTorgb274 = RGBToHSV( _ColorReal.rgb );
			float2 uv_MaterialMapReal = i.uv_texcoord * _MaterialMapReal_ST.xy + _MaterialMapReal_ST.zw;
			float3 hsvTorgb275 = HSVToRGB( float3(hsvTorgb274.x,hsvTorgb274.y,( hsvTorgb274.z * tex2D( _MaterialMapReal, uv_MaterialMapReal ).r )) );
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float clampResult284 = clamp( ( (-1.0 + (TRANS230 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + tex2D( _EffectMap, uv_EffectMap ).r ) , 0.0 , 1.0 );
			float MASK252 = ( 1.0 - clampResult284 );
			float3 lerpResult215 = lerp( hsvTorgb277 , hsvTorgb275 , MASK252);
			float3 DIFFUSE193 = lerpResult215;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			UnityGI gi270 = gi;
			float3 diffNorm270 = WorldNormalVector( i , NORM241 );
			gi270 = UnityGI_Base( data, 1, diffNorm270 );
			float3 indirectDiffuse270 = gi270.indirect.diffuse + diffNorm270 * 0.0001;
			c.rgb = ( saturate( ( tex2D( _ToonRamp, temp_cast_0 ) + _ShadowBoost ) ) * float4( DIFFUSE193 , 0.0 ) * ( ase_lightColor * float4( ( indirectDiffuse270 + ase_lightAtten ) , 0.0 ) ) ).rgb;
			c.a = 1;
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
476;45;1636;934;4902.01;2088.345;2.932591;True;False
Node;AmplifyShaderEditor.RangedFloatNode;228;-4023.072,-53.84336;Float;False;Property;_TransitionState;Transition State;7;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;229;-3998.278,635.965;Float;False;230;TRANS;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;230;-3739.177,-49.77846;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;232;-3814.743,539.6619;Float;False;Property;_NormalScale;Normal Scale;9;0;Create;True;0;0;False;0;6.588235;0.28;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;231;-3694.57,629.5276;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;-3422.827,720.3479;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;234;-3422.827,599.0519;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;236;-3235.38,559.9259;Float;True;Property;_NormalMapReal;Normal Map Real;2;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;235;-3226.868,762.142;Float;True;Property;_NormalMapLaser;Normal Map Laser;5;0;Create;True;0;0;False;0;e5895469fa6440e4a876bf0672599e59;e5895469fa6440e4a876bf0672599e59;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;237;-2919.219,684.424;Float;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;241;-2636.752,654.1475;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;-3971.468,-632.7993;Float;False;241;NORM;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;239;-3828.071,49.06698;Float;True;Property;_EffectMap;Effect Map;6;0;Create;True;0;0;False;0;None;667ae0b259a0d42ca882e292026397e2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;283;-3472.333,-97.45674;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;199;-3748.342,-652.8875;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;201;-3791.369,-501.6968;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;242;-3265.291,-56.58331;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;179;-3835.185,-1698.14;Float;False;Property;_ColorReal;Color Real;0;0;Create;True;0;0;False;0;0,0,0,0;0.5754716,0.4098875,0.446884,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;213;-3831.542,-1191.638;Float;False;Property;_ColorLaser;Color Laser;3;0;Create;True;0;0;False;0;0,0,0,0;0.6886792,0.6886792,0.6886792,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;224;-3525.29,-737.8773;Float;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;202;-3510.971,-646.4957;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;276;-3606.046,-1193.221;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RGBToHSVNode;274;-3626.06,-1697.538;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;272;-3919.06,-1529.539;Float;True;Property;_MaterialMapReal;Material Map Real;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;284;-3008.713,-35.68703;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;273;-3910.979,-1020.168;Float;True;Property;_MaterialMapLaser;Material Map Laser;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;281;-2845.245,-46.96689;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;282;-3606.655,-340.6752;Float;False;241;NORM;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;-3398.352,-1591.711;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;279;-3378.657,-1084.892;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;223;-3324.799,-731.5215;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;275;-3253.06,-1673.539;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;286;-3096.536,-874.472;Float;False;Property;_ShadowBoost;Shadow Boost;10;0;Create;True;0;0;False;0;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;203;-3097.17,-797.8975;Float;True;Property;_ToonRamp;ToonRamp;8;0;Create;True;0;0;False;0;None;4a056241e2722dc46a7262a8e7073fd9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;280;-3247.639,-1392.514;Float;False;252;MASK;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;270;-3392.236,-341.5983;Float;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;252;-2580.753,-51.79032;Float;True;MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;277;-3226.046,-1168.221;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightAttenuation;218;-3369.829,-264.6783;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;215;-2946.555,-1424.473;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;271;-3074.598,-335.1943;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;285;-2767.536,-822.472;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;220;-3319.23,-483.6677;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;222;-2823.072,-466.3445;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;-2627.462,-1429.726;Float;True;DIFFUSE;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;288;-2613.536,-819.472;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;226;-2731.25,-602.3893;Float;False;193;DIFFUSE;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;225;-2332.295,-600.2171;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;289;-2104.181,-144.248;Float;False;Property;_ColorPass;ColorPass;11;2;[HideInInspector];[PerRendererData];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-2063.006,-876.6843;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Transition/BothTexturedCel;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;1;Sobel=True;False;0;0;False;-1;-1;0;False;4;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;230;0;228;0
WireConnection;231;0;229;0
WireConnection;233;0;232;0
WireConnection;233;1;229;0
WireConnection;234;0;232;0
WireConnection;234;1;231;0
WireConnection;236;5;234;0
WireConnection;235;5;233;0
WireConnection;237;0;236;0
WireConnection;237;1;235;0
WireConnection;241;0;237;0
WireConnection;283;0;230;0
WireConnection;199;0;200;0
WireConnection;242;0;283;0
WireConnection;242;1;239;1
WireConnection;202;0;199;0
WireConnection;202;1;201;0
WireConnection;276;0;213;0
WireConnection;274;0;179;0
WireConnection;284;0;242;0
WireConnection;281;0;284;0
WireConnection;278;0;274;3
WireConnection;278;1;272;1
WireConnection;279;0;276;3
WireConnection;279;1;273;1
WireConnection;223;0;202;0
WireConnection;223;1;224;0
WireConnection;223;2;224;0
WireConnection;275;0;274;1
WireConnection;275;1;274;2
WireConnection;275;2;278;0
WireConnection;203;1;223;0
WireConnection;270;0;282;0
WireConnection;252;0;281;0
WireConnection;277;0;276;1
WireConnection;277;1;276;2
WireConnection;277;2;279;0
WireConnection;215;0;277;0
WireConnection;215;1;275;0
WireConnection;215;2;280;0
WireConnection;271;0;270;0
WireConnection;271;1;218;0
WireConnection;285;0;203;0
WireConnection;285;1;286;0
WireConnection;222;0;220;0
WireConnection;222;1;271;0
WireConnection;193;0;215;0
WireConnection;288;0;285;0
WireConnection;225;0;288;0
WireConnection;225;1;226;0
WireConnection;225;2;222;0
WireConnection;0;13;225;0
ASEEND*/
//CHKSM=15218F1DCA9EDA394CA3F9B88AEB141FFA465B4E