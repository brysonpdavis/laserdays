// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFY_AO_TEMPORALFILTER
#define AMPLIFY_AO_TEMPORALFILTER

#include "Common.cginc"

UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_CurrOcclusionDepth );
UNITY_DECLARE_SCREENSPACE_TEXTURE( _AO_TemporalAccumm );

float4	_AO_TemporalAccumm_TexelSize;

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
UNITY_DECLARE_SCREENSPACE_TEXTURE( _CameraMotionVectorsTexture );
#else
sampler2D_half _CameraMotionVectorsTexture;
#endif

float4x4	_AO_InvViewProjMatrixLeft;
float4x4	_AO_PrevViewProjMatrixLeft;
float4x4	_AO_PrevInvViewProjMatrixLeft;

float4x4	_AO_InvViewProjMatrixRight;
float4x4	_AO_PrevViewProjMatrixRight;
float4x4	_AO_PrevInvViewProjMatrixRight;

float		_AO_TemporalCurveAdj;
float		_AO_TemporalMotionSensibility;

half		_AO_TemporalDirections;
half		_AO_HalfProjScale;

inline half CalcDisocclusion( const half aDepth, const half aPrevDepth )
{
	return saturate( abs( 1.0 - ( aDepth / aPrevDepth ) ) * 2000.0 * _AO_TemporalMotionSensibility - ( 2.0 - aDepth * 2.0 ) * 2.0 );
}


inline void FetchPrevAO_Depth_N( const float2 aUV, out half outAO, out half outDepth, out half outN )
{
	float2 uv = aUV;
#if UNITY_UV_STARTS_AT_TOP
	uv.y = ( _ProjectionParams.x > 0 ) ? 1 - aUV.y : aUV.y;
#endif
	const half4 temporalAccummEnc = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_TemporalAccumm, UnityStereoTransformScreenSpaceTex( uv ) );

	outAO = DecAO( temporalAccummEnc.x );			// AO i-1
	outDepth = DecDepth( temporalAccummEnc.yz );	// Depth i-1
	outN = temporalAccummEnc.w;						// N i-1
}


inline half4 FetchTempAcc( const half2 aUV )
{
	return UNITY_SAMPLE_SCREENSPACE_TEXTURE( _AO_TemporalAccumm, UnityStereoTransformScreenSpaceTex( aUV ) );
}


inline half FetchNeighborAO_N( const half2 aUV, const half aCurDepth )
{
	const half2 xy = half2( 1.0, 1.0 );

	const half4 xLyC = FetchTempAcc( half2( -xy.x,     0 ) * _AO_TemporalAccumm_TexelSize.xy + aUV );
	const half4 xRyC = FetchTempAcc( half2( +xy.x,     0 ) * _AO_TemporalAccumm_TexelSize.xy + aUV );
	const half4 xCyU = FetchTempAcc( half2(     0, +xy.y ) * _AO_TemporalAccumm_TexelSize.xy + aUV );
	const half4 xCyD = FetchTempAcc( half2(     0, -xy.y ) * _AO_TemporalAccumm_TexelSize.xy + aUV );

	const half4 n = half4( xLyC.w, xRyC.w, xCyU.w, xCyD.w );
	const half minN = min( min( min( n.x, n.y ), n.z ), n.w );

	return minN;
}


void GetTemporalFilter( const float2 aScreenPos,
						const half aDepth,
						const bool aUseMotionVectors,
						const bool aTemporalDilation,
						out half outPrev_ao,
						out half outAccLerp,
						out half outNewN,
						out float2 outReprojUV )
{

	float2 reproj_screenPos;

	// P i = UnprojectToWorld( uv i , z i )

#if defined( SHADER_API_OPENGL ) || defined( SHADER_API_GLES ) || defined( SHADER_API_GLES3 ) || defined( SHADER_API_GLCORE )
	const float4 vpos = float4( float3( aScreenPos, aDepth ) * 2.0 - 1.0, 1.0 );
#else
	const float4 vpos = float4( ( aScreenPos * 2.0 - 1.0 ), ( 1.0 - aDepth ), 1.0 );
#endif

	#ifdef UNITY_SINGLE_PASS_STEREO
	float4x4 invViewProjMatrix;

	if ( unity_StereoEyeIndex == 0 )
	{
		invViewProjMatrix = _AO_InvViewProjMatrixLeft;
	}
	else
	{
		invViewProjMatrix = _AO_InvViewProjMatrixRight;
	}

	float4 wpos = mul( invViewProjMatrix, vpos );

	#else

	float4 wpos = mul( _AO_InvViewProjMatrixLeft, vpos );

	#endif

	wpos = wpos / wpos.w;

	float2 mv;

	if( aUseMotionVectors == false )
	{
		// uv i-1 = ProjectToUV( View i-1 , P i )

		#ifdef UNITY_SINGLE_PASS_STEREO
		float4x4 prevViewProjMatrix;

		if ( unity_StereoEyeIndex == 0 )
		{
			prevViewProjMatrix = _AO_PrevViewProjMatrixLeft;
		}
		else
		{
			prevViewProjMatrix = _AO_PrevViewProjMatrixRight;
		}

		const float4 reproj_vpos = mul( prevViewProjMatrix, wpos );

		#else
		const float4 reproj_vpos = mul( _AO_PrevViewProjMatrixLeft, wpos );
		#endif

		reproj_screenPos = ( reproj_vpos.xy / reproj_vpos.w ) * 0.5 + 0.5;

		mv = aScreenPos - reproj_screenPos;
	}
	else
	{
		mv = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _CameraMotionVectorsTexture, UnityStereoTransformScreenSpaceTex( aScreenPos) ).rg;

		reproj_screenPos = aScreenPos - mv;
	}

	// z i-1 = Fetch( ZBuffer i-1 , uv i-1 )
	half prev_ao, prev_depth, prev_N;
	FetchPrevAO_Depth_N( reproj_screenPos, prev_ao, prev_depth, prev_N );

	const half disocclusion = CalcDisocclusion( aDepth, prev_depth );

	half neighborN = prev_N;

	if( aTemporalDilation )
	{
		neighborN = FetchNeighborAO_N( reproj_screenPos, aDepth );
		prev_N = min( prev_N, neighborN );
	}

	const half outOfScreen = ( ( ( reproj_screenPos.x < 0.0 ) ||
								 ( reproj_screenPos.y < 0.0 ) ||
								 ( reproj_screenPos.x > 1.0 ) ||
								 ( reproj_screenPos.y > 1.0 ) ) == true )? 1.0 : 0.0;

	const half2 screenPosCenter = ( aScreenPos - 0.5 );
	const half screenPosInvSqrLen = 1.0 - dot( screenPosCenter, screenPosCenter );

	half att =	disocclusion + outOfScreen +
				dot( mv, mv ) * _AO_TemporalAccumm_TexelSize.w * _AO_TemporalMotionSensibility * screenPosInvSqrLen;

	att = 1.0 - saturate( att );

	const half newN = min( prev_N + ( 1.0 / 3.0 ), 1.0 ) * att;

	const half n = (aTemporalDilation == true)?min( newN, prev_N ):newN;

	outPrev_ao = prev_ao;
	outAccLerp = n;
	outNewN = newN;
	outReprojUV = reproj_screenPos;
}

#endif
