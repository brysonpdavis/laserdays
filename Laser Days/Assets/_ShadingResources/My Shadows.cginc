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
sampler2D _EffectMap, _MainTex;
float4 _EffectMap_ST, _MainTex_ST;
float _AlphaCutoff;
float _TransitionState;

float _AnimationMagnitude, _AnimationSpeed, _AnimationWaveSize, _AnimationTimeStep;
float4 _AnimationSway;

sampler3D _DitherMaskLOD;

struct VertexData {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
    
    #if defined(FOLIAGE)
        float4 color : COLOR;
    #endif
    
};

struct InterpolatorsVertex {
	float4 position : SV_POSITION;
	float3 worldPosition : TEXCOORD2;
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
    #endif
    #if defined(LASER)
        emv += _TransitionState;
        emv = step(1,emv);
    #endif 
    
      return emv;  
}

float3 GetVertexMovement(VertexData v, float3 wPos)
{
    float offset = _AnimationWaveSize * (wPos.x + wPos.z);
    offset += _Time.y * _AnimationSpeed;
    
    #if defined (W_POS_SWAYING)
        float magnitude = _AnimationMagnitude * sin(offset) * v.color.r;
        return magnitude * _AnimationSway.xyz;
    #endif
    #if defined (W_POS_SWELLING)
        float magnitude = _AnimationMagnitude * ((sin(offset) + 1) * 0.5) * v.color.r;
        return magnitude * v.normal.xyz;
    #endif
    #if defined (V_COLOR_SWAYING)
        float altOffset = _AnimationWaveSize * (v.color.g + v.color.b);
        altOffset += _Time.y * _AnimationSpeed;
        float magnitude = _AnimationMagnitude * (sin(altOffset)) * v.color.r;
        return magnitude * _AnimationSway.xyz;
    #endif
    #if defined (V_COLOR_GLITCHY)
        float altOffset = _AnimationWaveSize * (v.color.g + v.color.b);
        altOffset += ((floor(_Time.y * _AnimationTimeStep)) / _AnimationTimeStep) * _AnimationSpeed;
        float magnitude = _AnimationMagnitude * ((sin(altOffset) + 1) * 0.5) * v.color.r;
        return magnitude * _AnimationSway.xyz;
    #endif
    #if defined (V_COLOR_CIRCLE)
        float offA = _AnimationWaveSize * (v.color.g) + _AnimationSpeed * _Time.y;
        float offB = _AnimationWaveSize * (v.color.b) + _AnimationSpeed * _Time.y;
        float magA = _AnimationMagnitude * sin(offA) * v.color.r;
        float magB = _AnimationMagnitude * cos(offB) * v.color.r;
        float3 newDirection = _AnimationMagnitude * float3(magA, magB, 0);
        return newDirection;
    #endif
    
    return float3(0,0,0);
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
    
    i.uv = TRANSFORM_TEX(v.uv, _MainTex);
    
    #if defined(TERRAIN)
        i.uv = (v.position.xz);
    #endif

	return i;
}

InterpolatorsVertex MyShadowVertexProgramFoliage (VertexData v) {
    InterpolatorsVertex i;
    
    float3 vWorldPos = mul(unity_ObjectToWorld, v.position);
    float3 newVertPos = v.position;
    
    #if defined(FOLIAGE) && !defined(NO_ANIMATION)
        newVertPos += GetVertexMovement(v, vWorldPos);
    #endif
    
    #if defined(SHADOWS_CUBE)
        i.position = UnityObjectToClipPos(newVertPos);
        i.lightVec = mul(unity_ObjectToWorld, newVertPos).xyz - _LightPositionRange.xyz;
    #else
        i.position = UnityClipSpaceShadowCasterPos(newVertPos, v.normal);
        i.position = UnityApplyLinearShadowBias(i.position);
    #endif
    
    i.uv = TRANSFORM_TEX(v.uv, _MainTex);
    
    #if defined(TERRAIN)
        i.uv = (v.position.xz);
    #endif

    return i;
}

float4 MyShadowFragmentProgram (Interpolators i) : SV_TARGET {
        
        float alpha = GetAlpha(i);  
		clip(alpha - _AlphaCutoff);
   

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