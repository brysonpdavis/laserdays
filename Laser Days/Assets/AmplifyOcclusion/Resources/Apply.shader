// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Occlusion/Apply"
{
	CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#pragma exclude_renderers gles d3d11_9x n3ds
		#pragma multi_compile_instancing

		#include "Common.cginc"
		#include "TemporalFilter.cginc"

		float4		_AO_Target_TexelSize;

		UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_OcclusionTexture );
		UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_ColorTexture );
		UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_GBufferAlbedo );
		UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_GBufferEmission );
		UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_ApplyOcclusionTexture );

		half4		_AO_Levels;
		half4		_AO_FadeToTint;
		float4		_AO_UVToView;
		half		_AO_PowExponent;

		struct DeferredOutput
		{
			half4 albedo : SV_Target0;
			half4 emission : SV_Target1;
		};

		struct DeferredOutputTemporal
		{
			half4 albedo : SV_Target0;
			half4 emission : SV_Target1;
			half4 temporalAcc : SV_Target2;
		};

		struct PostEffectOutputTemporal
		{
			half4 occlusionColor : SV_Target0;
			half4 temporalAcc : SV_Target1;
		};


		v2f_out vert( appdata v )
		{
			v2f_out o;
			UNITY_SETUP_INSTANCE_ID( v );
			UNITY_INITIALIZE_OUTPUT( v2f_out, o );
			UNITY_TRANSFER_INSTANCE_ID( v, o );
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

			float4 vertex = float4( v.vertex.xy * 2.0 - 1.0, 0.0, 1.0 );

		#ifdef UNITY_HALF_TEXEL_OFFSET
			vertex.xy += ( 1.0 / _AO_Target_TexelSize.zw ) * float2( -1, 1 );
		#endif

			o.pos = vertex;

		#ifdef UNITY_SINGLE_PASS_STEREO
			#if UNITY_UV_STARTS_AT_TOP
				o.uv = float2( v.uv.x, 1.0 - v.uv.y );
			#else
				o.uv = v.uv;
			#endif
		#else
			o.uv = ComputeScreenPos( vertex ).xy;
		#endif

			return o;
		}


		inline void FetchOcclusion_LinearDepth( const half2 aScreenPos, out half outOcclusion, out half outLinearDepth )
		{
			const half2 occlusion_depth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_OcclusionTexture, UnityStereoTransformScreenSpaceTex( aScreenPos ) ).rg;

			outOcclusion = occlusion_depth.x;
			outLinearDepth = occlusion_depth.y;
		}


		inline half4 CalcOcclusion( const half aOcclusion, const half aLinearDepth )
		{
			const half distanceFade = ComputeDistanceFade( aLinearDepth );

			const half exponent = lerp( _AO_PowExponent, _AO_FadeValues.z, distanceFade );

			const half occlusion = pow( aOcclusion, exponent );

			half3 tintedOcclusion = lerp( _AO_Levels.rgb, _AO_FadeToTint.rgb, distanceFade );

			tintedOcclusion = lerp( tintedOcclusion, ( 1 ).xxx, occlusion.xxx );

			const half intensity = lerp( _AO_Levels.a, _AO_FadeValues.x, distanceFade );

			return lerp( ( 1 ).xxxx, half4( tintedOcclusion.rgb, occlusion ), intensity );
		}


		half4 TemporalFilter( const half2 aScreenPos, const half2 aOcclusionDepth, const bool aUseMotionVectors, const bool aTemporalDilation, out half outOcclusion )
		{
			const half ao = aOcclusionDepth.x;
			half depth01 = LinearEyeToSampledDepth( aOcclusionDepth.y );

			#if defined(UNITY_REVERSED_Z)
			depth01 = 1.0 - depth01;
			#endif

			half prev_ao, accLerp, newN;
			float2 reprojUV;

			GetTemporalFilter(	aScreenPos,
								depth01,
								aUseMotionVectors,
								aTemporalDilation,
								prev_ao,
								accLerp,
								newN,
								reprojUV );

			half depthFactor;
			depthFactor = saturate( 1.0 - depth01 * depth01 );

			half aoDiff = ( ao - prev_ao ) * depthFactor;
			aoDiff = saturate( ( saturate( -aoDiff ) + saturate( aoDiff * 16.0 * _AO_TemporalMotionSensibility ) ) - 0.005 );

			newN = saturate( newN - aoDiff );

			aoDiff = saturate( 1.0 - aoDiff * 2.0 );

			const half lerpAcc = min( aoDiff, accLerp ) * _AO_TemporalCurveAdj;

			const half new_ao = lerp( ao, prev_ao, lerpAcc );

			outOcclusion = new_ao;

			return half4( EncAO( new_ao ), EncDepth( depth01 ), newN );
		}


		half4 ApplyDebug( const v2f_in IN )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half2 screenPos = IN.uv.xy;

			half occlusion, linearDepth;
			FetchOcclusion_LinearDepth( screenPos, occlusion, linearDepth );

			const half4 occlusionRGBA = CalcOcclusion( occlusion, linearDepth );

			return half4( occlusionRGBA.rgb, 1 );
		}


		PostEffectOutputTemporal ApplyDebugTemporal( const v2f_in IN, const bool aUseMotionVectors, const bool aTemporalDilation )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const float2 screenPos = IN.uv.xy;

			const half2 occlusionDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_CurrOcclusionDepth, UnityStereoTransformScreenSpaceTex( screenPos ) ).rg;

			PostEffectOutputTemporal OUT;

			if( occlusionDepth.y < HALF_MAX )
			{
				half occlusion;
				const half4 temporalAcc = TemporalFilter( screenPos, occlusionDepth, aUseMotionVectors, aTemporalDilation, occlusion );

				const half4 occlusionRGBA = CalcOcclusion( occlusion, occlusionDepth.y );

				OUT.occlusionColor = occlusionRGBA;
				OUT.temporalAcc = temporalAcc;
			}
			else
			{
				OUT.occlusionColor = half4( (1).xxxx );
				OUT.temporalAcc = half4( (1).xxxx );
			}

			return OUT;
		}


		DeferredOutput ApplyDeferred( v2f_in IN, const bool log )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half2 screenPos = IN.uv.xy;

			half occlusion, linearDepth;
			FetchOcclusion_LinearDepth( screenPos, occlusion, linearDepth );

			const half4 occlusionRGBA = CalcOcclusion( occlusion, linearDepth );

			half4 emission, albedo;

			if ( log )
			{
				emission = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_GBufferEmission, UnityStereoTransformScreenSpaceTex( screenPos ) );
				albedo = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_GBufferAlbedo, UnityStereoTransformScreenSpaceTex( screenPos ) );

				emission.rgb = -log2( emission.rgb );
				emission.rgb *= occlusionRGBA.rgb;

				albedo.a *= occlusionRGBA.a;

				emission.rgb = exp2( -emission.rgb );
			}
			else
			{
				albedo = half4( 1, 1, 1, occlusionRGBA.a );
				emission = half4( occlusionRGBA.rgb, 1 );
			}

			DeferredOutput OUT;
			OUT.albedo = albedo;
			OUT.emission = emission;
			return OUT;
		}


		DeferredOutputTemporal ApplyDeferredTemporal( v2f_in IN, const bool log, const bool aUseMotionVectors, const bool aTemporalDilation )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const float2 screenPos = IN.uv.xy;

			const half2 occlusionDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_CurrOcclusionDepth, UnityStereoTransformScreenSpaceTex( screenPos ) ).rg;

			DeferredOutputTemporal OUT;

			if( occlusionDepth.y < HALF_MAX )
			{
				half occlusion;
				const half4 temporalAcc = TemporalFilter( screenPos, occlusionDepth, aUseMotionVectors, aTemporalDilation, occlusion );

				const half4 occlusionRGBA = CalcOcclusion( occlusion, occlusionDepth.y );

				half4 emission, albedo;

				if ( log )
				{
					emission = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_GBufferEmission, UnityStereoTransformScreenSpaceTex( screenPos ) );
					albedo = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_GBufferAlbedo, UnityStereoTransformScreenSpaceTex( screenPos ) );

					emission.rgb = -log2( emission.rgb );
					emission.rgb *= occlusionRGBA.rgb;

					albedo.a *= occlusionRGBA.a;

					emission.rgb = exp2( -emission.rgb );
				}
				else
				{
					albedo = half4( 1, 1, 1, occlusionRGBA.a );
					emission = half4( occlusionRGBA.rgb, 1 );
				}

				OUT.albedo = albedo;
				OUT.emission = emission;
				OUT.temporalAcc = temporalAcc;
			}
			else
			{
				half4 emission, albedo;

				if ( log )
				{
					emission = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_GBufferEmission, UnityStereoTransformScreenSpaceTex( screenPos ) );
					albedo = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_GBufferAlbedo, UnityStereoTransformScreenSpaceTex( screenPos ) );
				}
				else
				{
					albedo = half4( (1).xxxx );
					emission = half4( (1).xxxx );
				}

				OUT.albedo = albedo;
				OUT.emission = emission;
				OUT.temporalAcc = half4( (1).xxxx );
			}

			return OUT;
		}


		PostEffectOutputTemporal ApplyPostEffectTemporal( v2f_in IN, const bool aUseMotionVectors, const bool aTemporalDilation )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const float2 screenPos = IN.uv.xy;

			const half2 occlusionDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_CurrOcclusionDepth, UnityStereoTransformScreenSpaceTex( screenPos ) ).rg;

			PostEffectOutputTemporal OUT;

			if( occlusionDepth.y < HALF_MAX )
			{
				half occlusion;
				const half4 temporalAcc = TemporalFilter( screenPos, occlusionDepth, aUseMotionVectors, aTemporalDilation, occlusion );

				const half4 occlusionRGBA = CalcOcclusion( occlusion, occlusionDepth.y );

				OUT.occlusionColor = occlusionRGBA;
				OUT.temporalAcc = temporalAcc;
			}
			else
			{
				OUT.occlusionColor = half4( (1).xxxx );
				OUT.temporalAcc = half4( (1).xxxx );
			}

			return OUT;
		}

		half4 ApplyPostEffectTemporalMultiply( v2f_in IN )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			return UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_ApplyOcclusionTexture, UnityStereoTransformScreenSpaceTex( IN.uv.xy ) );
		}

		DeferredOutput ApplyDeferredTemporalMultiply( v2f_in IN )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half4 occlusionRGBA = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_ApplyOcclusionTexture, UnityStereoTransformScreenSpaceTex( IN.uv.xy ) );

			DeferredOutput OUT;
			OUT.albedo = half4( (1.0).xxx, occlusionRGBA.a );
			OUT.emission = half4( occlusionRGBA.rgb, 1.0 );

			return OUT;
		}

		half4 ApplyPostEffect( v2f_in IN )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half2 screenPos = IN.uv.xy;

			half occlusion, linearDepth;
			FetchOcclusion_LinearDepth( screenPos, occlusion, linearDepth );

			const half4 occlusionRGBA = CalcOcclusion( occlusion, linearDepth );

			return half4( occlusionRGBA.rgb, 1 );
		}
	ENDCG

	///////////////////////////////////////////////////////////////////////////////////////
	// MRT BLENDING PATH
	///////////////////////////////////////////////////////////////////////////////////////

	SubShader
	{
		Tags { "MRTBlending" = "True" }
		ZTest Always Cull Off ZWrite Off

		// -- APPLICATION METHODS --------------------------------------------------------------
		// 0 => APPLY DEBUG
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return ApplyDebug( IN ); } ENDCG }
		// 1 => APPLY DEBUG Temporal
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, false, false ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, false, true ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, true, false ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, true, true ); } ENDCG }

		// 5 => APPLY DEFERRED
		Pass
		{
			Blend DstColor Zero, DstAlpha Zero
			CGPROGRAM DeferredOutput frag( v2f_in IN ) { return ApplyDeferred( IN, false ); } ENDCG
		}
		// 6 => APPLY DEFERRED Temporal
		Pass
		{
			Blend 0 DstColor Zero, DstAlpha Zero
			Blend 1 DstColor Zero, DstAlpha Zero
			Blend 2 Off
			CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, false, false ); } ENDCG
		}
		Pass
		{
			Blend 0 DstColor Zero, DstAlpha Zero
			Blend 1 DstColor Zero, DstAlpha Zero
			Blend 2 Off
			CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, false, true ); } ENDCG
		}
		Pass
		{
			Blend 0 DstColor Zero, DstAlpha Zero
			Blend 1 DstColor Zero, DstAlpha Zero
			Blend 2 Off
			CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, true, false ); } ENDCG
		}
		Pass
		{
			Blend 0 DstColor Zero, DstAlpha Zero
			Blend 1 DstColor Zero, DstAlpha Zero
			Blend 2 Off
			CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, true, true ); } ENDCG
		}

		// 10 => APPLY DEFERRED (LOG)
		Pass { CGPROGRAM DeferredOutput frag( v2f_in IN ) { return ApplyDeferred( IN, true ); } ENDCG }
		// 11 => APPLY DEFERRED (LOG) Temporal
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, false, false ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, false, true ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, true, false ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, true, true ); } ENDCG }


		// 15 => APPLY POST-EFFECT
		Pass
		{
			Blend DstColor Zero
			CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return ApplyPostEffect( IN ); } ENDCG
		}
		// 16 => APPLY POST-EFFECT Temporal
		Pass
		{
			Blend 0 DstColor Zero
			Blend 1 Off
			CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, false, false ); } ENDCG
		}
		Pass
		{
			Blend 0 DstColor Zero
			Blend 1 Off
			CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, false, true ); } ENDCG
		}
		Pass
		{
			Blend 0 DstColor Zero
			Blend 1 Off
			CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, true, false ); } ENDCG
		}
		Pass
		{
			Blend 0 DstColor Zero
			Blend 1 Off
			CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, true, true ); } ENDCG
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////
	// NO MRT BLENDING FALLBACK
	///////////////////////////////////////////////////////////////////////////////////////

	SubShader
	{
		Tags { "MRTBlending" = "False" }
		ZTest Always Cull Off ZWrite Off

		// -- APPLICATION METHODS --------------------------------------------------------------
		// 0 => APPLY DEBUG
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return ApplyDebug( IN ); } ENDCG }
		// 1 => APPLY DEBUG Temporal
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, false, false ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, false, true ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, true, false ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyDebugTemporal( IN, true, true ); } ENDCG }

		// 5 => APPLY DEFERRED
		Pass
		{
			Blend DstColor Zero, DstAlpha Zero
			CGPROGRAM DeferredOutput frag( v2f_in IN ) { return ApplyDeferred( IN, false ); } ENDCG
		}
		// 6 => APPLY DEFERRED Temporal
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, false, false ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, false, true ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, true, false ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, false, true, true ); } ENDCG }

		// 10 => APPLY DEFERRED (LOG)
		Pass { CGPROGRAM DeferredOutput frag( v2f_in IN ) { return ApplyDeferred( IN, true ); } ENDCG }
		// 11 => APPLY DEFERRED (LOG) Temporal
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, false, false ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, false, true ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, true, false ); } ENDCG }
		Pass { CGPROGRAM DeferredOutputTemporal frag( v2f_in IN ) { return ApplyDeferredTemporal( IN, true, true, true ); } ENDCG }


		// 15 => APPLY POST-EFFECT
		Pass { Blend DstColor Zero CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return ApplyPostEffect( IN ); } ENDCG }
		// 16 => APPLY POST-EFFECT Temporal
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, false, false ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, false, true ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, true, false ); } ENDCG }
		Pass { CGPROGRAM PostEffectOutputTemporal frag( v2f_in IN ) { return ApplyPostEffectTemporal( IN, true, true ); } ENDCG }

		// 20 => APPLY POST-EFFECT Temporal Multiply
		Pass
		{
			Blend DstColor Zero
			CGPROGRAM half4 frag( v2f_in IN ) : SV_Target0 { return ApplyPostEffectTemporalMultiply( IN ); } ENDCG
		}

		// 21 => APPLY DEFERRED Temporal Multiply
		Pass
		{
			Blend DstColor Zero, DstAlpha Zero
			CGPROGRAM DeferredOutput frag( v2f_in IN ) { return ApplyDeferredTemporalMultiply( IN ); } ENDCG
		}
	}

	Fallback Off
}
