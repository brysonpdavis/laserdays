#if !defined(CROSSHATCH_STANDARD_CG)
#define CROSSHATCH_STANDARD_CG

#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"
#include "CrosshatchBRDF.cginc"

float4 _BeginColor, _BaseColor, _DeathColor, _AccentColor, _TintColor;
sampler2D _MainTex, _AccentMap, _EffectMap;
float4 _MainTex_ST, _AccentMap_ST, _EffectMap_ST;

float _Smoothness, _Smoothness2, _Highlights, _MainTexContribution;

int _LineA;

float _TransitionState, _TransitionStateB, _AlphaCutoff, _Elapsed, _BeginToBase, _BaseToDeath;  

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

float4 GetDefaultUV (Interpolators i)
{
    return i.uv;   
}

//Gets which value should control color transition
//For shared objects, _TransitionState controls color fade
//For interactables the _TransitionStateB controls color
float TransitionValue ()
{
    return _TransitionState;
}

float4 GetTextureValue (sampler2D tex, Interpolators i)
{
    return = tex2D(tex, i.uv.xy);
}

//Returns base color per fragment by blending MaterialMap with Real and Laser base colors.
float3 GetBaseColor (Interpolators i) 
{  
    float3 a = lerp(_BeginColor, _BaseColor, _BeginToBase).rgb;
    float3 b = lerp(a, _DeathColor, _BaseToDeath).rgb; 
    
    return b * _TintColor.rgb;
}

//Returns a 0 || 1 accent mask value per fragment. 
float GetAccentMask (Interpolators i) {
    
    float3 accTex = GetTextureValue(_AccentMap, i).rgb; 
   
    #if defined(REAL)
        return step(0.1, accTex.r);
    #endif
    #if defined(LASER)
        return step(0.1, accTex.g);
    #else
        float accMask = lerp(accTex.r, accTex.g, TransitionValue());
        return step(0.1, accMask); 
    #endif
}

//Blends accent colors with input color based on GetAccentMask result.
float3 accentBlend (float3 input, Interpolators i)
{
    #if defined(REAL)
        return lerp(input, _RealAccent.rgb, GetAccentMask(i) * _AccentColor.a);
    #endif
    #if defined(LASER)
        return lerp(input, _LaserAccent.rgb, GetAccentMask(i) * _AccentColor.a);
    #else
        float4 blendCol = lerp(_RealAccent, _LaserAccent, TransitionValue());
        return lerp(input, blendCol.rgb, GetAccentMask(i) * blendCol.a);
    #endif
}

//Keeping this to be called, but just returning one.
float GetAlpha (Interpolators i) 
{
    float4 mutationMap = GetTextureValue(_MainTex, i);
    float vitality = saturate(_BeginToBase - _BaseToDeath);
    
    return step(0.5, mutationMap.r * (mutationMap.b + vitality));
}

//Returns alpha at fragment to determine whether it gets clipped. 
float GetAlphaSingle (Interpolators i)
{
    float emv = GetTextureValue(_EffectMap, i).r;
    float trans;
    
    #if defined(REAL)
        emv -= trans;
        emv = step(0,emv);
        return emv;
    #endif
    #if defined(LASER)
        emv += trans;
        emv = step(1,emv);
        return emv;
    #endif  
    
    return 1;    
}

// Return simple tangent normal
float3 GetTangentSpaceNormal (Interpolators i) 
{
    float3 normal = float3(0, 0, 1);
    return normal;
}

//Return constant low value for metallic. Part of keeping shading how we want. 
float GetMetallic (Interpolators i) {
   return 0.05;
}

//Currently using smoothness to control outline smoothing 
float GetSmoothness (Interpolators i) 
{
    return _Smoothness;    
}

float GetShininess (Interpolators i )
{
    return 1 - _Highlights;
}


//Currently not using occlusion. But could use unused chanels of MaterialMap.
float GetOcclusion (Interpolators i) 
{
   return 1;
}

float3 BaseColorWrapper (Interpolators i)
{
    float3 albedo = GetBaseColor(i);
    
    #if defined(ACCENT_ON)
        albedo = accentBlend(albedo, i);
    #endif
       
    return albedo;
}

//Returns outline data to use in gBuffer1 -> communicates key information to the outline post-processor. 
float3 GetOutlineData (Interpolators i)
{
    //AccentMask is 0 || 1, multuplying by 0.25 leaves room for transparent object outlines
    float r = GetAccentMask(i) * 0.25;
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
        float alpha = GetAlpha(i) * GetAlphaSingle(i);
    #else
        float alpha = GetAlpha(i);
    #endif
    
    clip(GetAlphaSingle(i) - _AlphaCutoff);
   
    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

    float3 specularTint;
    float oneMinusReflectivity;
    
    float3 albedo = BaseColorWrapper(i);
    
    specularTint = float4(0.1, 0.1, 0.1, 0);
    oneMinusReflectivity = 1;
    
    #if defined(_RENDERING_TRANSPARENT)
        albedo *= alpha;
        alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
    #endif

    float4 color = BRDF_Unity_Toon(
        albedo, float4(0,0,0,0),
        1.0, 0,
        i.normal, viewDir,
        CreateLight(i), CreateIndirectLight(i, viewDir)
    );
    

      
    #if defined(_RENDERING_FADE) || defined(_RENDERING_TRANSPARENT)
        color.a = alpha;
    #endif

    FragmentOutput output;
    #if defined(DEFERRED_PASS)
        #if !defined(UNITY_HDR_ON)
            color.rgb = exp2(-color.rgb);
        #endif
        output.gBuffer0.rgb = albedo;
        output.gBuffer0.a = GetShininess(i);
        output.gBuffer1.rgb = GetOutlineData(i);
        output.gBuffer1.a = GetSmoothness(i);
        output.gBuffer2 = float4(i.normal * 0.5 + 0.5, GetOcclusion(i));
        output.gBuffer3 = color;
    #else
        output.color = color;
    #endif
    return output;
}

#endif