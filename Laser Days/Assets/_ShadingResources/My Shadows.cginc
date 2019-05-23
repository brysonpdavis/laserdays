#if !defined(MY_SHADOWS_INCLUDED)
#define MY_SHADOWS_INCLUDED

#include "UnityCG.cginc"

#if defined(_RENDERING_FADE) || defined(_RENDERING_TRANSPARENT)
	#if defined(_SEMITRANSPARENT_SHADOWS)
		#define SHADOWS_SEMITRANSPARENT 1
	#else
		#define _RENDERING_CUTOUT
	#endif
#endif

#if SHADOWS_SEMITRANSPARENT || defined(_RENDERING_CUTOUT) || defined (USECLIP)
	#if !defined(_SMOOTHNESS_ALBEDO)
		#define SHADOWS_NEED_UV 1
	#endif
#endif



float4 _Tint;
sampler2D _EffectMap;
float4 _EffectMap_ST;
float _AlphaCutoff;
float _TransitionState;

sampler3D _DitherMaskLOD;

struct VertexData {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

struct InterpolatorsVertex {
	float4 position : SV_POSITION;
	
		float2 uv : TEXCOORD0;

	#if defined(SHADOWS_CUBE)
		float3 lightVec : TEXCOORD1;
	#endif
};

struct Interpolators {
	#if SHADOWS_SEMITRANSPARENT
		UNITY_VPOS_TYPE vpos : VPOS;
	#else
		float4 positions : SV_POSITION;
	#endif

	
		float2 uv : TEXCOORD0;

	#if defined(SHADOWS_CUBE)
		float3 lightVec : TEXCOORD1;
	#endif
};

float GetAlpha (Interpolators i) {
	float emv = tex2D(_EffectMap, i.uv.xy).r;
    
    #if defined(REAL)
        emv -= _TransitionState;
        emv = step(0,emv);
        return emv;
    #endif
    #if defined(LASER)
        emv += _TransitionState;
        emv = step(1,emv);
        return emv;
    #endif   
}

InterpolatorsVertex MyShadowVertexProgram (VertexData v) {
	InterpolatorsVertex i;
	#if defined(SHADOWS_CUBE)
		i.position = UnityObjectToClipPos(v.position);
		i.lightVec =
			mul(unity_ObjectToWorld, v.position).xyz - _LightPositionRange.xyz;
	#else
		i.position = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
		i.position = UnityApplyLinearShadowBias(i.position);
	#endif

	
		i.uv = TRANSFORM_TEX(v.uv, _EffectMap);

	return i;
}

float4 MyShadowFragmentProgram (Interpolators i) : SV_TARGET {
	#if defined(REAL) || defined(LASER)
        float alpha = GetAlpha(i);  
		clip(alpha - _AlphaCutoff);
        
	#endif

	#if SHADOWS_SEMITRANSPARENT
		float dither =
			tex3D(_DitherMaskLOD, float3(i.vpos.xy * 0.25, alpha * 0.9375)).a;
		clip(dither - 0.01);
	#endif
	
	#if defined(SHADOWS_CUBE)
		float depth = length(i.lightVec) + unity_LightShadowBias.x;
		depth *= _LightPositionRange.w;
		return UnityEncodeCubeShadowDepth(depth);
	#else
		return 0;
	#endif
}

#if defined(SHADOWS_CUBE)

#endif

#endif