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


//Goop vertex animation function - multiply result by vertex normal
inline float goopVertexAnimation (float3 pos, float time, float animate)
{
    float m = pos.x * pos.z + time;
    m = 0.04 * sin(m * 8);
    m *= animate;
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



#endif