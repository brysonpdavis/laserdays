Shader "Crosshatch/Glass-Single" {
    
    Properties {
        
        [Toggle]_Real("Is Real", Float) = 0
        
        _MainTex("Dirtiness Map", 2D) = "white" {}
        _AccentMap("Accent Map", 2D) = "black" {}
        _EffectMap("Effect Map", 2D) = "white" {}
        
        _Opacity("Opacity", Range(0,1)) = 0
        _Dirtiness("Dirtiness Contribution", Range(0,1)) = 0
        
        _Base("Base Color", Color) = (0,0,0,0)  
        _Accent("Accent Color", Color) = (0,0,0,0)

                                                        
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
        uniform float4 _Base, _Accent;   
        uniform float _TransitionState, _Opacity, _Dirtiness, _Real, _AlphaCutoff;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }

        float3 getColor(Input i)
        {
            float acc = glassAccent(i.uv_texcoord, _AccentMap, _AccentMap_ST, _TransitionState);
            return lerp(_Base, _Accent, acc * _Accent.a);
        }
        
        float getOpacity(Input i)
        {
            float2 dirtUV = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
            float2 dirtTex = tex2D(_MainTex, dirtUV).rg;
            float dirt = lerp(dirtTex.r, dirtTex.g, _TransitionState);
            dirt *= _Dirtiness;
            return saturate(_Opacity + dirt);
        }
        
        void surf (Input i, inout SurfaceOutput o) {
            
            float clipValue = glassSingleAlpha(i.uv_texcoord, _EffectMap, _EffectMap_ST, _TransitionState, _Real);
            clip(clipValue - _AlphaCutoff);
            
            o.Emission.rgb = getColor(i);
            //float cool = 0.5;
            o.Alpha = getOpacity(i);

        }
        ENDCG
    }
    Fallback "Legacy Shaders/VertexLit"
}
