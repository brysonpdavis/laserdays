// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Occlusion/Blur"
{
	Properties { }
	CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#pragma exclude_renderers gles d3d11_9x n3ds

		#include "Common.cginc"

		UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_Source );

		float4		_AO_Source_TexelSize;
		float4		_AO_Target_TexelSize;
		float		_AO_BlurSharpness;


		inline half ComputeSharpness( half linearEyeDepth )
		{
			return _AO_BlurSharpness * ( saturate( 1 - linearEyeDepth ) + 0.01 );
		}

		inline half ComputeFalloff( const int radius )
		{
			return 2.0 / ( radius * radius );
		}

		inline half2 CrossBilateralWeight( const half2 r, half2 d, half d0, const half sharpness, const half falloff )
		{
			half2 diff = ( d0 - d ) * sharpness;
			return exp2( -( r * r ) * falloff - diff * diff );
		}

		inline half4 CrossBilateralWeight( const half4 r, half4 d, half d0, const half sharpness, const half falloff )
		{
			half4 diff = ( d0 - d ) * sharpness;
			return exp2( -( r * r ) * falloff - diff * diff );
		}

		inline half2 FetchOcclusionDepth( half2 aUV )
		{
			return UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( aUV ) ).rg;
		}

		half4 blur1D_1x( v2f_in IN, half2 deltaUV )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half2 occlusionDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( IN.uv ) ).rg;

			const half occlusion = occlusionDepth.x;
			const half depth = occlusionDepth.y;

			const half2 offset1 = 1.0 * deltaUV;

			half4 s1;
			s1.xy = FetchOcclusionDepth( IN.uv + offset1 ).xy;
			s1.zw = FetchOcclusionDepth( IN.uv - offset1 ).xy;

			const half sharpness = ComputeSharpness( depth );
			const half falloff = ComputeFalloff( 1 );

			const half2 w1 = CrossBilateralWeight( ( 1 ).xx, s1.yw, depth, sharpness, falloff );

			half ao = occlusion + dot( s1.xz, w1 );
			ao /= 1.0 + dot( ( 1.0 ).xx, w1 );

			return half4( ao, depth, 0.0, 0.0 );
		}

		half4 blur1D_2x( v2f_in IN, half2 deltaUV )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half2 occlusionDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( IN.uv ) ).rg;

			const half occlusion = occlusionDepth.x;
			const half depth = occlusionDepth.y;

			const half2 offset1 = 1.0 * deltaUV;
			const half2 offset2 = 2.0 * deltaUV;

			half4 s1, s2;
			s2.zw = FetchOcclusionDepth( IN.uv - offset2 ).xy;
			s1.zw = FetchOcclusionDepth( IN.uv - offset1 ).xy;
			s1.xy = FetchOcclusionDepth( IN.uv + offset1 ).xy;
			s2.xy = FetchOcclusionDepth( IN.uv + offset2 ).xy;

			const half sharpness = ComputeSharpness( depth );
			const half falloff = ComputeFalloff( 2 );

			const half4 w12 = CrossBilateralWeight( half4( 1, 1, 2, 2 ), half4( s1.yw, s2.yw ), depth, sharpness, falloff );

			half ao = occlusion + dot( half4( s1.xz, s2.xz ), w12 );
			ao /= 1.0 + dot( ( 1.0 ).xxxx, w12 );

			return half4( ao, depth, 0.0, 0.0 );
		}


		half4 blur1D_3x( v2f_in IN, half2 deltaUV )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half2 occlusionDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( IN.uv ) ).rg;

			const half occlusion = occlusionDepth.x;
			const half depth = occlusionDepth.y;

			const half2 offset1 = 1.0 * deltaUV;
			const half2 offset2 = 2.0 * deltaUV;
			const half2 offset3 = 3.0 * deltaUV;

			half4 s1, s2, s3;
			s3.zw = FetchOcclusionDepth( IN.uv - offset3 ).xy;
			s2.zw = FetchOcclusionDepth( IN.uv - offset2 ).xy;
			s1.zw = FetchOcclusionDepth( IN.uv - offset1 ).xy;
			s1.xy = FetchOcclusionDepth( IN.uv + offset1 ).xy;
			s2.xy = FetchOcclusionDepth( IN.uv + offset2 ).xy;
			s3.xy = FetchOcclusionDepth( IN.uv + offset3 ).xy;

			const half sharpness = ComputeSharpness( depth );
			const half falloff = ComputeFalloff( 3 );

			const half4 w12 = CrossBilateralWeight( half4( 1, 1, 2, 2 ), half4( s1.yw, s2.yw ), depth, sharpness, falloff );
			const half2 w3 = CrossBilateralWeight( ( 3 ).xx, s3.yw, depth, sharpness, falloff );

			half ao = occlusion + dot( half4( s1.xz, s2.xz ), w12 ) + dot( s3.xz, w3 );
			ao /= 1.0 + dot( ( 1.0 ).xxxx, w12 ) + dot( ( 1 ).xx, w3 );

			return half4( ao, depth, 0.0, 0.0 );
		}


		half4 blur1D_4x( v2f_in IN, half2 deltaUV )
		{
			UNITY_SETUP_INSTANCE_ID( IN );
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

			const half2 occlusionDepth = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_Source, UnityStereoTransformScreenSpaceTex( IN.uv ) ).rg;

			const half occlusion = occlusionDepth.x;
			const half depth = occlusionDepth.y;

			const half2 offset1 = 1.0 * deltaUV;
			const half2 offset2 = 2.0 * deltaUV;
			const half2 offset3 = 3.0 * deltaUV;
			const half2 offset4 = 4.0 * deltaUV;

			half4 s1, s2, s3, s4;
			s4.zw = FetchOcclusionDepth( IN.uv - offset4 ).xy;
			s3.zw = FetchOcclusionDepth( IN.uv - offset3 ).xy;
			s2.zw = FetchOcclusionDepth( IN.uv - offset2 ).xy;
			s1.zw = FetchOcclusionDepth( IN.uv - offset1 ).xy;
			s1.xy = FetchOcclusionDepth( IN.uv + offset1 ).xy;
			s2.xy = FetchOcclusionDepth( IN.uv + offset2 ).xy;
			s3.xy = FetchOcclusionDepth( IN.uv + offset3 ).xy;
			s4.xy = FetchOcclusionDepth( IN.uv + offset4 ).xy;

			const half sharpness = ComputeSharpness( depth );
			const half falloff = ComputeFalloff( 4 );

			const half4 w12 = CrossBilateralWeight( half4( 1, 1, 2, 2 ), half4( s1.yw, s2.yw ), depth, sharpness, falloff );
			const half4 w34 = CrossBilateralWeight( half4( 3, 3, 4, 4 ), half4( s3.yw, s4.yw ), depth, sharpness, falloff );

			half ao = occlusion + dot( half4( s1.xz, s2.xz ), w12 ) + dot( half4( s3.xz, s4.xz ), w34 );
			ao /= 1.0 + dot( ( 1.0 ).xxxx, w12 ) + dot( ( 1 ).xxxx, w34 );

			return half4( ao, depth, 0.0, 0.0 );
		}


		v2f_out vert( appdata v )
		{
			v2f_out o;
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

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
	ENDCG


	SubShader
	{
		ZTest Always Cull Off ZWrite Off

		// 0 => BLUR HORIZONTAL R:1
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_1x( IN, half2( _AO_Source_TexelSize.x, 0 ) ); } ENDCG }

		// 1 => BLUR VERTICAL R:1
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_1x( IN, half2( 0, _AO_Source_TexelSize.y ) ); } ENDCG }

		// 2 => BLUR HORIZONTAL R:2
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_2x( IN, half2( _AO_Source_TexelSize.x, 0 ) ); } ENDCG }

		// 3 => BLUR VERTICAL R:2
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_2x( IN, half2( 0, _AO_Source_TexelSize.y ) ); } ENDCG }

		// 4 => BLUR HORIZONTAL R:3
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_3x( IN, half2( _AO_Source_TexelSize.x, 0 ) ); } ENDCG }

		// 5 => BLUR VERTICAL R:3
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_3x( IN, half2( 0, _AO_Source_TexelSize.y ) ); } ENDCG }

		// 6 => BLUR HORIZONTAL R:4
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_4x( IN, half2( _AO_Source_TexelSize.x, 0 ) ); } ENDCG }

		// 7 => BLUR VERTICAL R:4
		Pass { CGPROGRAM half4 frag( v2f_in IN ) : SV_Target { return blur1D_4x( IN, half2( 0, _AO_Source_TexelSize.y ) ); } ENDCG }
	}

	Fallback Off
}
