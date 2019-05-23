﻿#if !defined(CROSSHATCH_STANDARD_CG)
#define CROSSHATCH_STANDARD_CG

#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"
#include "CrosshatchBRDF.cginc"

float4 _RealBase, _RealAccent, _LaserBase, _LaserAccent, _RealGradient, _LaserGradient, _ShimmerColor, _InteractColor;
sampler2D _MainTex, _AccentMap, _EffectMap, _ShadingMap;
float4 _MainTex_ST, _AccentMap_ST, _EffectMap_ST, _ShadingMap_ST;

float _Smoothness, _GradientScale, _GradientOffset, _MainTexContribution;

int _LineA;

float _TransitionState, _TransitionStateB, _AlphaCutoff;
float _onHold, _onHover, _Flippable, _Elapsed;  


//Vertex Structure
struct VertexData {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;
};

//v2f Structure
struct Interpolators {
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
    float3 normal : TEXCOORD1;

    #if defined(BINORMAL_PER_FRAGMENT)
        float4 tangent : TEXCOORD2;
    #else
        float3 tangent : TEXCOORD2;
        float3 binormal : TEXCOORD3;
    #endif

    float3 worldPos : TEXCOORD4;

    SHADOW_COORDS(5)

    #if defined(VERTEXLIGHT_ON)
        float3 vertexLightColor : TEXCOORD6;
    #endif
};

//Remaps (-scale + offset) -> (scale + offset) to 0 -> 1 range.
//For building object space gradients.
inline half remap01 (float val, float scale, float off)
{
    float range = (scale + off) - (-scale + off);
    float pos = val - (-scale + off);\
    return 1 - saturate(pos/range);
}

//Remaps 0 -> 1 to newMin -> 1.
//For scaling multiplied intensity values.
inline half remapNewMin (float val, float newMin)
{
    newMin = saturate(newMin);
    val *= (1 - newMin);
    val += newMin;
    return val;
}

//Returns base color per fragment by blending MaterialMap with Real and Laser base colors.
float3 GetBaseColor (Interpolators i) 
{
    float3 tex = tex2D(_MainTex, i.uv.xy);
    float valMin = 1 - _MainTexContribution;
    
    #if defined(REAL)
        return _RealBase.rgb * remapNewMin(tex.r, valMin);
    #endif
    #if defined(LASER)
        return _LaserBase.rgb * remapNewMin(tex.g, valMin);
    #else
        float val = lerp(tex.r, tex.g, _TransitionState);
        val = remapNewMin(val, valMin);
        float3 col = lerp(_RealBase, _LaserBase, _TransitionState).rgb;
        return val * col;
    #endif  
}

//Returns 0 -> 1 gradient value at a fragment - based on fragment object position;
float GetGradient (Interpolators i)
{ 
    float3 objectPos = mul (unity_WorldToObject, float4(i.worldPos, 1)).xyz;
    
    #if defined(HEIGHT_GRADIENT)
        float a = remap01(objectPos.y, _GradientScale, _GradientOffset);
        return a;
        return floor(a*4)/4;
    #endif
    #if defined(RADIAL_GRADIENT)
        float a = remap01(length(objectPos), _GradientScale, _GradientOffset);
        return a;
        return floor(a*4)/4;
    #endif
}

//Blends gradient colors with input color based on GetGradient result.
float3 gradientBlend (float3 input, Interpolators i)
{   
    #if defined(REAL)
        return lerp(input, _RealGradient.rgb, GetGradient(i) *  _RealGradient.a); 
    #endif
    #if defined(LASER)
        return lerp(input, _LaserGradient.rgb, GetGradient(i) *  _LaserGradient.a); 
    #else 
        float4 blendCol = lerp(_RealGradient, _LaserGradient, _TransitionState);
        return lerp(input, blendCol.rgb, GetGradient(i) * blendCol.a); 
    #endif
    
    return input;  
}

//Returns a 0 || 1 accent mask value per fragment. 
float GetAccentMask (Interpolators i) {
    float3 accTex = tex2D(_AccentMap, i.uv.xy); 
    float accMask = 0;   
    
    #if defined(REAL)
        accMask = accTex.r;
        return step(0.1, accMask);
    #endif
    #if defined(LASER) 
        accMask = accTex.g;
        return step(0.1, accMask);
    #endif
    #if defined(SHARED)
        accMask = lerp(accTex.r, accTex.g, _TransitionState);
        return step(0.1, accMask); 
    #endif
}

//Blends accent colors with input color based on GetAccentMask result.
float3 accentBlend (float3 input, Interpolators i)
{
    #if defined(REAL)
        return lerp(input, _RealAccent.rgb, GetAccentMask(i) * _RealAccent.a);
    #endif
    #if defined(LASER)
        return lerp(input, _LaserAccent.rgb, GetAccentMask(i) * _LaserAccent.a);
    #else
        float4 blendCol = lerp(_RealAccent, _LaserAccent, _TransitionState);
        return lerp(input, blendCol.rgb, GetAccentMask(i) * blendCol.a);
    #endif
}

//Keeping this to be called, but just returning one.
float GetAlpha (Interpolators i) {
    return 1.0;
}

//Returns alpha at fragment to determine whether it gets clipped. 
float GetAlphaSingle (Interpolators i){
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

// Return simple tangent normal
float3 GetTangentSpaceNormal (Interpolators i) {
    float3 normal = float3(0, 0, 1);
    return normal;
}

//Return constant low value for metallic. Part of keeping shading how we want. 
float GetMetallic (Interpolators i) {
   return 0.05;
}

//Currently using smoothness to control outline smoothing 
// TODO actual smoothness in the lighting model
float GetSmoothness (Interpolators i) 
{
    return _Smoothness;
}

//Currently not using occlusion. But could use unused chanels of MaterialMap.
float GetOcclusion (Interpolators i) 
{
   float o;
   float3 s = tex2D(_ShadingMap, i.uv.zw);
   s = step(0.3, s); 
   o = s.b;
   return o;
}


float GetGlowMask (Interpolators i) 
{
    #if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
        #if defined (INTERACTABLE)
            float tex = tex2D(_EffectMap, i.uv.xy).b;
            float texOff = tex2D(_EffectMap, i.uv.xy + (0.1 * _Elapsed)).g;
            float glowMask = tex * texOff;
            glowMask *= _onHold * _Flippable;
            return step(0.3, glowMask);     
        #endif
    #endif
    return 0;
}

//Returns 0 -> 1 value for interaction amount
float3 GetInteraction (Interpolators i)
{
   return saturate((_onHover * 0.33) + _onHold) * _InteractColor.a;   
}

//Returns outline data to use in gBuffer1 -> communicates key information to the outline post-processor. 
float3 GetOutlineData (Interpolators i)
{
    //AccentMask is 0 || 1, multuplying by 0.25 leaves room for transparent object outlines
    float r = GetAccentMask(i) * 0.25;
    #if defined(INTERACTABLE)
        r += GetGlowMask(i) * 0.25;
    #endif
    //Allows 8 different values for getting outlines between materials with similar depth/normals 
    float g = 0.125 * floor(_LineA);
    return float3(r,g,0);
}


//As it was.
void ComputeVertexLightColor (inout Interpolators i) {
    #if defined(VERTEXLIGHT_ON)
        i.vertexLightColor = Shade4PointLights(
            unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
            unity_LightColor[0].rgb, unity_LightColor[1].rgb,
            unity_LightColor[2].rgb, unity_LightColor[3].rgb,
            unity_4LightAtten0, i.worldPos, i.normal
        );
    #endif
}

//As it was.
float3 CreateBinormal (float3 normal, float3 tangent, float binormalSign) {
    return cross(normal, tangent.xyz) *
        (binormalSign * unity_WorldTransformParams.w);
}

Interpolators MyVertexProgram (VertexData v) {
    Interpolators i;
    i.pos = UnityObjectToClipPos(v.vertex);
    i.worldPos = mul(unity_ObjectToWorld, v.vertex);
    i.normal = UnityObjectToWorldNormal(v.normal);

    #if defined(BINORMAL_PER_FRAGMENT)
        i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
    #else
        i.tangent = UnityObjectToWorldDir(v.tangent.xyz);
        i.binormal = CreateBinormal(i.normal, i.tangent, v.tangent.w);
    #endif

    i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
    i.uv.zw = TRANSFORM_TEX(v.uv, _ShadingMap);
    //i.uv.zw = TRANSFORM_TEX(v.uv, _EffectMap);

    TRANSFER_SHADOW(i);

    ComputeVertexLightColor(i);
    return i;
}

UnityLight CreateLight (Interpolators i) {
    UnityLight light;

    #if defined(DEFERRED_PASS)
        light.dir = float3(0, 1, 0);
        light.color = 0;
    #else
        #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
            light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
        #else
            light.dir = _WorldSpaceLightPos0.xyz;
        #endif

        UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
        
        light.color = _LightColor0.rgb * attenuation;
    #endif
    return light;
}

float3 BoxProjection (
    float3 direction, float3 position,
    float4 cubemapPosition, float3 boxMin, float3 boxMax
) {
    #if UNITY_SPECCUBE_BOX_PROJECTION
        UNITY_BRANCH
        if (cubemapPosition.w > 0) {
            float3 factors =
                ((direction > 0 ? boxMax : boxMin) - position) / direction;
            float scalar = min(min(factors.x, factors.y), factors.z);
            direction = direction * scalar + (position - cubemapPosition);
        }
    #endif
    return direction;
}

UnityIndirect CreateIndirectLight (Interpolators i, float3 viewDir) {
    UnityIndirect indirectLight;
    indirectLight.diffuse = 0;
    indirectLight.specular = 0;

    #if defined(VERTEXLIGHT_ON)
        indirectLight.diffuse = i.vertexLightColor;
    #endif

    #if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
        indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
        float3 reflectionDir = reflect(-viewDir, i.normal);
        Unity_GlossyEnvironmentData envData;
        envData.roughness = 1 - GetSmoothness(i);
        envData.reflUVW = BoxProjection(
            reflectionDir, i.worldPos,
            unity_SpecCube0_ProbePosition,
            unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax
        );
        float3 probe0 = Unity_GlossyEnvironment(
            UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData
        );
        envData.reflUVW = BoxProjection(
            reflectionDir, i.worldPos,
            unity_SpecCube1_ProbePosition,
            unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax
        );
        #if UNITY_SPECCUBE_BLENDING
            float interpolator = unity_SpecCube0_BoxMin.w;
            UNITY_BRANCH
            if (interpolator < 0.99999) {
                float3 probe1 = Unity_GlossyEnvironment(
                    UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0),
                    unity_SpecCube0_HDR, envData
                );
                indirectLight.specular = lerp(probe1, probe0, interpolator);
            }
            else {
                indirectLight.specular = probe0;
            }
        #else
            indirectLight.specular = probe0;
        #endif
        
        indirectLight.specular = 0.0;

        float occlusion = 1;
        indirectLight.diffuse *= occlusion;
        indirectLight.specular *= occlusion;

        #if defined(DEFERRED_PASS) && UNITY_ENABLE_REFLECTION_BUFFERS
            indirectLight.specular = 0;
        #endif
    #endif

    return indirectLight;
}

void InitializeFragmentNormal(inout Interpolators i) {
    float3 tangentSpaceNormal = GetTangentSpaceNormal(i);
    #if defined(BINORMAL_PER_FRAGMENT)
        float3 binormal = CreateBinormal(i.normal, i.tangent.xyz, i.tangent.w);
    #else
        float3 binormal = i.binormal;
    #endif
    
    i.normal = normalize(
        tangentSpaceNormal.x * i.tangent +
        tangentSpaceNormal.y * binormal +
        tangentSpaceNormal.z * i.normal
    );
}

struct FragmentOutput {
    #if defined(DEFERRED_PASS)
        float4 gBuffer0 : SV_Target0;
        float4 gBuffer1 : SV_Target1;
        float4 gBuffer2 : SV_Target2;
        float4 gBuffer3 : SV_Target3;
    #else
        float4 color : SV_Target;
    #endif
};

FragmentOutput MyFragmentProgram (Interpolators i) {
    
    #if defined(REAL) || defined(LASER)
        float alpha = GetAlpha(i);
        clip(GetAlphaSingle(i) - _AlphaCutoff);
    #endif

    InitializeFragmentNormal(i);

    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

    float3 specularTint;
    float oneMinusReflectivity;
    
    float3 albedo = GetBaseColor(i);
    
    #if defined(INTERACTABLE)
        albedo = lerp(albedo, _InteractColor, GetInteraction(i));
    #endif
    
    
    #if defined(ACCENT_ON)
        albedo = accentBlend(albedo, i);
    #endif
    
    #if defined(HEIGHT_GRADIENT) || defined(RADIAL_GRADIENT)
        albedo = gradientBlend(albedo, i);
    #endif
    
  
    specularTint = float4(0.1, 0.1, 0.1, 0);
    oneMinusReflectivity = 1;
    
    #if defined(_RENDERING_TRANSPARENT)
        albedo *= alpha;
        alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
    #endif

    float4 color = BRDF_Unity_Toon(
        albedo, float4(0,0,0,0),
        1.0, GetSmoothness(i),
        i.normal, viewDir,
        CreateLight(i), CreateIndirectLight(i, viewDir)
    );
    
    #if defined(INTERACTABLE)
        color.rgb += _InteractColor.rgb * GetInteraction(i);
        color.rgb += _ShimmerColor.rgb * GetGlowMask(i);
    #endif
    
    
    #if defined(_RENDERING_FADE) || defined(_RENDERING_TRANSPARENT)
        color.a = alpha;
    #endif

    FragmentOutput output;
    #if defined(DEFERRED_PASS)
        #if !defined(UNITY_HDR_ON)
            color.rgb = exp2(-color.rgb);
        #endif
        output.gBuffer0.rgb = albedo;
        output.gBuffer0.a = GetOcclusion(i);
        output.gBuffer1.rgb = GetOutlineData(i);
        output.gBuffer1.a = GetSmoothness(i);
        output.gBuffer2 = float4(i.normal * 0.5 + 0.5, 1);
        output.gBuffer3 = color;
    #else
        output.color = color;
    #endif
    return output;
}

#endif