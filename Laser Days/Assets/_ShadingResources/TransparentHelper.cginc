#if !defined(CROSSHATCH_TRANSPARENT_HELPER_INCLUDED)
#define CROSSHATCH_TRANSPARENT_HELPER_INCLUDED

// Returns two panning values packed into one float4
inline float4 crossPan (float time)
{
    float4 panned;
    panned.xy = time * float2(0.01, 0.01).xy;
    panned.zw = time * float2(0.02, -0.03).xy;
    return panned;
}

//Goop vertex animation function
inline float3 goopVertexAnimation (float3 pos, float3 norm, float time, float animate)
{
    float offset = pos.x * pos.z + time;
    float magnitude = 0.04 * sin(offset * 8);
    float3 movement = magnitude * norm.xyz;
    movement *= animate;
    return movement; 
}

//Goop vertex animation function - multiply result by vertex normal
inline float goopPlantVertexAnimation (float3 pos, float time, float animate)
{
    float m = pos.x * pos.z + 5 * pos.y + time;
    m = sin(m * 0.8) + 1;
    m *= 0.07;
    return m; 
}

//Crystal vertex animation function - multiply result by vertex normal
inline float crystalVertexAnimation (float3 pos, float trans, float mag)
{
    return mag * trans;
}

//Goop trigger transition mask 
inline float goopAlpha (float2 uv, sampler2D tex, float4 ST, float trans, float time, float real)
{
    float2 map_uv = uv * ST.xy + ST.zw;
    float4 pan = crossPan(time);
    
    // First crossPan value to sample red channel
    // Second crossPan value to sample green channel
    
    float tex_a = tex2D(tex, map_uv + pan.xy).r; 
    float tex_b = tex2D(tex, map_uv + pan.zw).g;
    
    float result = (tex_a - tex_b + 1) * 0.5;
    result = step(0, result - trans);
    float oneMinusReuslt = 1 - result;
    
    return saturate(result * real + oneMinusReuslt * (1 - real));
}

float crystalAlpha (float2 uv, sampler2D tex, float4 ST, float trans)
{
    float w = tex2D(tex, (uv * ST.xy + ST.zw )).r;
    w -= trans;
    w = step(0, w);
    return w;
}

inline float goopShimmer (float2 uv, sampler2D tex, float4 ST, float trans, float time, float real)
{
    float2 map_uv = uv * ST.xy + ST.zw;
    float4 pan = crossPan(time);
    
    // First crossPan value to sample red channel
    // Second crossPan value to sample green channel
    
    float tex_a = tex2D(tex, map_uv + pan.xy).r; 
    float tex_b = tex2D(tex, map_uv + pan.zw).g;
            
    float result = tex_b - tex_a;
    result = step(0, result);
    return result;                   
}

//Accent map for both singe and double world glass shaders
inline float glassAccent (float2 uv, sampler2D tex, float4 ST, float trans)
{
    float2 map_uv = uv * ST.xy + ST.zw;
    float2 tex_a = tex2D(tex, map_uv).rg;
    float acc = lerp(tex_a.r, tex_a.g, trans);
    acc = step(0.1, acc);
    return acc;                   
}

inline float blockerAccent (float2 uv, sampler2D tex, float4 ST, float trans, float time)
{
    
    float2 map_uv = uv * ST.xy + ST.zw;
    float2 tex_a = tex2D(tex, map_uv).rg;
    float tex_b = tex2D(tex, map_uv + time).b;
    float acc = lerp(tex_a.r, tex_a.g, trans);
    acc = step(0.1, acc * tex_b);
    return acc;                   
}

inline float blockerPlayerHole (float2 uv, sampler2D tex, float4 ST, float3 camPos, float3 wPos)
{
    float2 map_uv = uv * ST.xy + ST.zw;
    float tex_a = tex2D(tex, map_uv).b;
    float3 diff = camPos - wPos;
    float len = length(diff);
    len = 1 - saturate(len);
    return step(0.1, len * saturate(tex_a + 0.5));    
}

//Clip value for single world glass
//TODO refactor all alpha functions - to use another world like clip
inline float glassSingleAlpha (float2 uv, sampler2D tex, float4 ST, float trans, float real)
{
    float tex_a = tex2D(tex, (uv * ST.xy + ST.zw )).r;
    tex_a -= trans;
    tex_a = step(0, tex_a);
    float tex_b = 1 - tex_a;
    return saturate(tex_a * real + tex_b * (1 - real));
}

inline float goopPlantIntensity(float3 pos, float time)
{
    float m = pos.x * pos.z + 2*pos.y + time;
    m = sin(m);
    
    m = (m + 1) * 0.4;
    m *= m * m;
    return m;
}




#endif