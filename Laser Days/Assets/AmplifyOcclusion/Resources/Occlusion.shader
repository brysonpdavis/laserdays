// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Occlusion/Occlusion"
{
	CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#pragma exclude_renderers gles d3d11_9x n3ds
		#include "GTAO.cginc"

		UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_Source );

		v2f_out vert( appdata v )
		{
			v2f_out o;
			UNITY_SETUP_INSTANCE_ID( v );
			UNITY_TRANSFER_INSTANCE_ID( v, o );
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

			float4 vertex = v.vertex * float4( 2.0, 2.0, 1.0, 1.0 ) + float4( -1.0, -1.0, 0.0, 0.0 );

			#ifdef UNITY_HALF_TEXEL_OFFSET
			vertex.xy += ( 1.0 / _AO_Target_TexelSize.zw ) * float2( -1.0, 1.0 );
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


		half4 GTAO( const v2f_in ifrag, const int directionCount, const int sampleCount, const int normalSource )
		{
			UNITY_SETUP_INSTANCE_ID( ifrag );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( ifrag );

			half outDepth;
			half4 outRGBA;

			GetGTAO( ifrag, directionCount, sampleCount / 2, normalSource, outDepth, outRGBA );

			return half4( outRGBA.a, outDepth, 0, 0 );
		}

		half4 CombineDownsampledOcclusionDepth( const v2f_in ifrag )
		{
			UNITY_SETUP_INSTANCE_ID( ifrag );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( ifrag );

			const half2 screenPos = ifrag.uv.xy;

			const half depthSample = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, UnityStereoTransformScreenSpaceTex( screenPos ) );

			//UNITY_BRANCH
			#if defined(UNITY_REVERSED_Z)
			if( depthSample <= 1e-7 ) // 0b0000000000000010
			#else
			if( depthSample >= 0.999 ) // 0b0011101111111110
			#endif
			{
				return half4( 1.0, HALF_MAX, 0, 0 );
			}

			const half referenceDepth = LinearEyeDepth( depthSample );
			half outOcclusion;

			const half2 sPosAdjusted = floor( screenPos * _AO_Source_TexelSize.zw + half2( 0.5, 0.5 ) ) * _AO_Source_TexelSize.xy;
			half2 odLU = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( sPosAdjusted + half2( -0.5, -0.5 ) * _AO_Source_TexelSize.xy ) ).rg;
			half2 odRU = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( sPosAdjusted + half2( +0.5, -0.5 ) * _AO_Source_TexelSize.xy ) ).rg;
			half2 odLD = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( sPosAdjusted + half2( -0.5, +0.5 ) * _AO_Source_TexelSize.xy ) ).rg;
			half2 odRD = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( sPosAdjusted + half2( +0.5, +0.5 ) * _AO_Source_TexelSize.xy ) ).rg;

			const half4 o0123 = half4( odLU.x, odRU.x, odLD.x, odRD.x );
			const half4 d0123 = half4( odLU.y, odRU.y, odLD.y, odRD.y );

			half4 depthWeight0123 = saturate( 1.0 / ( abs( LinearEyeToSampledDepth( d0123 ) - ( depthSample ).xxxx ) * 16384 + 1.0 ) );
			half weightOcclusion = dot( o0123, depthWeight0123 );

			outOcclusion = saturate( weightOcclusion / dot( ( 1 ).xxxx, depthWeight0123 ) );

			return half4( outOcclusion, referenceDepth, 0, 0 );
		}
	ENDCG

	SubShader
	{
		ZTest Always
		Cull Off
		ZWrite Off

		// 0-3 => FULL OCCLUSION - LOW QUALITY                    directionCount / sampleCount
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 4, NORMALS_NONE ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 4, NORMALS_CAMERA ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 4, NORMALS_GBUFFER ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 4, NORMALS_GBUFFER_OCTA_ENCODED ); } ENDCG }

		// 04-07 => FULL OCCLUSION / MEDIUM QUALITY
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 6, NORMALS_NONE ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 6, NORMALS_CAMERA ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 6, NORMALS_GBUFFER ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 2, 6, NORMALS_GBUFFER_OCTA_ENCODED ); } ENDCG }

		// 08-11 => FULL OCCLUSION - HIGH QUALITY
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 3, 8, NORMALS_NONE ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 3, 8, NORMALS_CAMERA ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 3, 8, NORMALS_GBUFFER ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 3, 8, NORMALS_GBUFFER_OCTA_ENCODED ); } ENDCG }

		// 12-15 => FULL OCCLUSION / VERYHIGH QUALITY
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 4, 10, NORMALS_NONE ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 4, 10, NORMALS_CAMERA ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 4, 10, NORMALS_GBUFFER ); } ENDCG }
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return GTAO( IN, 4, 10, NORMALS_GBUFFER_OCTA_ENCODED ); } ENDCG }

		// 16 => CombineDownsampledOcclusionDepth
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return CombineDownsampledOcclusionDepth( IN );	} ENDCG	}
	}
}

