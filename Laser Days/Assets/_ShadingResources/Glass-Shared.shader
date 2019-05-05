Shader "Crosshatch/Glass-Shared" {
    
    Properties {
        
        _MainTex("Dirtiness Map", 2D) = "white" {}
        _AccentMap("Accent Map", 2D) = "black" {}
        
        _Opacity("Opacity", Range(0,1)) = 0
        _Dirtiness("Dirtiness Contribution", Range(0,1)) = 0
        
        _RealBase("Real Base Color", Color) = (0,0,0,0)  
        _RealAccent("Real Accent Color", Color) = (0,0,0,0)
        
        _LaserBase("Laser Base Color", Color) = (0,0,0,0)
        _LaserAccent("Laser Accent Color", Color) = (0,0,0,0)
                                                        
        _TransitionState("Transition State", Range( 0 , 1)) = 0      
                
        [HideInInspector] _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5
        
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
          
        sampler2D _MainTex, _AccentMap;
        float4 _MainTex_ST, _AccentMap_ST;
        uniform float4 _RealBase, _RealAccent, _LaserBase, _LaserAccent;   
        uniform float _TransitionState, _Opacity, _Dirtiness;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }

        float3 getColor(Input i)
        {
            float3 baseCol = lerp(_RealBase, _LaserBase, _TransitionState).rgb;
            float4 accCol = lerp(_RealAccent, _LaserAccent, _TransitionState);
            float acc = glassAccent(i.uv_texcoord, _AccentMap, _AccentMap_ST, _TransitionState);
            return lerp(baseCol, accCol, acc * accCol.a);
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
            
            o.Emission.rgb = getColor(i);
            //float cool = 0.5;
            o.Alpha = getOpacity(i);

        }
        ENDCG
    }
    Fallback "Legacy Shaders/VertexLit"
}
