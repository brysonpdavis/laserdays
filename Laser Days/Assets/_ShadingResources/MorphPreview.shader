Shader "Crosshatch/MorphPreview" {
    
    Properties {
        
        [Toggle]_Real("Is Real", Float) = 0        
        
        _Opacity("Opacity", Range(0,1)) = 0
        
        _Base("Base Color", Color) = (0,0,0,0) 
        
        _onHold("On Hold", float) = 0
                                                        
        _TransitionState("Transition State", Range( 0 , 1)) = 0      
                
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5
        
        [HideInInspector] _texcoord( "", 2D ) = "white" {}        
    }
    
    
    SubShader 
    {
        Tags{ "RenderType" = "TransparentCutout" "Queue" = "Transparent" "IsEmissive" = "True"}
        Cull Back
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
          
        uniform float4 _Base;   
        uniform float _TransitionState, _Opacity, _Real, _AlphaCutoff, _onHold;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }
        

        void surf (Input i, inout SurfaceOutput o) {
            clip(_onHold - _AlphaCutoff);
            half worldClip = lerp(_TransitionState, 1 - _TransitionState, _Real);
            clip(worldClip - _AlphaCutoff);
            
            o.Emission.rgb = _Base.rgb;
            o.Alpha = _Opacity;

        }
        ENDCG
    }
    Fallback "Legacy Shaders/VertexLit"
}
