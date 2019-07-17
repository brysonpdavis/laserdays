Shader "Crosshatch/Glass-Mouth" {
    
    Properties {
    
        [Toggle] _Real("Real", Float) = 0
        
        [HideInInspector] _MainTex("Dirtiness Map", 2D) = "white" {}
        _AccentMap("Accent Map", 2D) = "black" {}
        _EffectMap("Effect Map", 2D) = "black" {}
        
        [HideInInspector] _Opacity("Opacity", Range(0,1)) = 0
        [HideInInspector]_Dirtiness("Dirtiness Contribution", Range(0,1)) = 0
        
        _RealBase("Base Color", Color) = (0,0,0,0)  
        _RealAccent("Accent Color", Color) = (0,0,0,0)
        
       [HideInInspector]  _LaserBase("Laser Base Color", Color) = (0,0,0,0)
       [HideInInspector]  _LaserAccent("Laser Accent Color", Color) = (0,0,0,0)
        
       [HideInInspector] _Elapsed("Elapsed", float) = 0
        
        _TransitionState("Transition State", Range( 0 , 1)) = 0  
            
                
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5
        
        [HideInInspector] _texcoord( "", 2D ) = "white" {}        
    }
    
    
    SubShader 
    {
        Tags{ "RenderType" = "TransparentCutout" "Queue" = "Transparent" "IsEmissive" = "True"}
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Unlit alpha:fade keepalpha noshadow 
        #pragma target 3.0
        
        #include "TransparentHelper.cginc"
        
        struct Input {
            float4 screenPos;
            float2 uv_texcoord;
            float3 worldPos;
        };
          
        sampler2D _MainTex, _AccentMap, _EffectMap;
        float4 _MainTex_ST, _AccentMap_ST, _EffectMap_ST;
        uniform float4 _RealBase, _RealAccent, _LaserBase, _LaserAccent, _BlockColor;   
        float4 _Hit;
        uniform float _TransitionState, _Opacity, _Dirtiness, _Blocked, _Elapsed, _AlphaCutoff, _Real;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }
        
        
        float getCollisionPoint(Input i)
        {
            float xs = i.uv_texcoord.x - _Hit.x;
            xs *= xs;
            float xy = i.uv_texcoord.y - _Hit.y;
            xy *= xy;
            float dist = sqrt(xs + xy);
            
            return step(_Hit.z, dist);
        }
        
      
        float4 getColor(Input i)
        {
            //float3 baseCol = lerp(_RealBase, _LaserBase, _TransitionState).rgb;
            //float4 accCol = lerp(_RealAccent, _LaserAccent, _TransitionState);
            float acc = blockerAccent(i.uv_texcoord, _AccentMap, _AccentMap_ST, _TransitionState, _Elapsed);   
            float3 color = lerp(_RealBase.rgb, _RealAccent.rgb, acc * _RealAccent.a);
            color = lerp(color, _BlockColor.rgb, _Blocked);
            float alph = lerp(_RealBase.a, _BlockColor.a, _Blocked);
            return float4(color, alph);
        }
       
        
       //blockerPlayerHole (float2 uv, sampler2D tex, float4 ST, float3 camPos, float3 wPos)
        
        void surf (Input i, inout SurfaceOutput o) {
            
            float clipValue = glassSingleAlpha(i.uv_texcoord, _EffectMap, _EffectMap_ST, _TransitionState, _Real);
            clip(clipValue - _AlphaCutoff);
            
            float4 oo = getColor(i);
            o.Emission.rgb = oo.rgb;
            //float cool = 0.5;
            o.Alpha = oo.a;

        }
        ENDCG
    }
    Fallback "Legacy Shaders/VertexLit"
}
