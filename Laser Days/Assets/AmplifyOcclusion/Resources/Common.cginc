// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFY_AO_COMMON_INCLUDED
#define AMPLIFY_AO_COMMON_INCLUDED

#pragma multi_compile_instancing

#include "UnityCG.cginc"

#define HALF_MAX        65504.0 // (2 - 2^-10) * 2^15
#define HALF_MAX_MINUS1 65472.0 // (2 - 2^-9) * 2^15

inline half DecAO( const half ao )
{
	return sqrt( ao );
}

inline half EncAO( const half ao )
{
	return saturate( ao * ao );
}

inline half4 DecAO( const half4 ao )
{
	return sqrt( ao );
}

inline half4 EncAO( const half4 ao )
{
	return saturate( ao * ao );
}

float2		_AO_FadeParams;
float4		_AO_FadeValues;

inline half ComputeDistanceFade( const half distance )
{
	return saturate( max( 0.0, distance - _AO_FadeParams.x ) * _AO_FadeParams.y );
}

inline half DecDepth( const float2 aEncodedDepth )
{
	return DecodeFloatRG( aEncodedDepth );
}

inline half2 EncDepth( const half aDepth )
{
	return EncodeFloatRG( aDepth );
}

inline float LinearEyeToSampledDepth( float linearEyeDepth )
{
	return ( 1.0 - linearEyeDepth * _ZBufferParams.w ) / ( linearEyeDepth * _ZBufferParams.z );
}

inline float2 LinearEyeToSampledDepth( float2 linearEyeDepth )
{
	return ( (1.0).xx - linearEyeDepth * ( _ZBufferParams.w ).xx ) / ( linearEyeDepth * ( _ZBufferParams.z ).xx );
}

inline float3 LinearEyeToSampledDepth( float3 linearEyeDepth )
{
	return ( (1.0).xxx - linearEyeDepth * ( _ZBufferParams.w ).xxx ) / ( linearEyeDepth * ( _ZBufferParams.z ).xxx );
}

inline float4 LinearEyeToSampledDepth( float4 linearEyeDepth )
{
	return ( (1.0).xxxx - linearEyeDepth * ( _ZBufferParams.w ).xxxx ) / ( linearEyeDepth * ( _ZBufferParams.z ).xxxx );
}

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};


struct v2f_out
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};


struct v2f_in
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};


#if !defined( UNITY_DECLARE_DEPTH_TEXTURE )
#define UNITY_DECLARE_DEPTH_TEXTURE(tex) sampler2D_float tex
#endif

#if !defined( UNITY_DECLARE_SCREENSPACE_TEXTURE )
#define UNITY_DECLARE_SCREENSPACE_TEXTURE(tex) sampler2D tex;
#endif

#if !defined( UNITY_SAMPLE_SCREENSPACE_TEXTURE )
#define UNITY_SAMPLE_SCREENSPACE_TEXTURE(tex, uv) tex2D(tex, uv)
#endif

#if !defined( UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX )
#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(x)
#endif

#endif
