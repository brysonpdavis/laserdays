Shader "Crosshatch/Goop-PlantVariant" {
    
    Properties {
        
        [Toggle]_Real("Is Real", float) = 0
        [Toggle]_Animated("Use Vertex Animation", Float) = 0
         _AccentMap("Accent Map", 2D) = "black" {}
         _EffectMap("Effect Map", 2D) = "white" {}
        
        [HDR]_RestingColor("Resting Color", Color) = (0,0,0,0)
        [HDR]_ShimmerColor("Shimmer Color", Color) = (0,0,0,0)
        [HDR]_ThirdColor("Third Color", Color) = (0,0,0,0)
        
        _FadeLength("_FadeLength", Range(0,3)) = 0.1
                
        [HideInInspector]_Elapsed("Elapsed", Float) = 0
           
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
        #pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
        #pragma target 3.0
        
        #include "TransparentHelper.cginc"
        
        struct Input {
            float4 screenPos;
            float2 uv_texcoord;
            float3 worldPos;
        };
          
        sampler2D _AccentMap, _EffectMap;
        float4 _AccentMap_ST, _EffectMap_ST;
  
        sampler2D _CameraDepthTexture;
        uniform float _Real;
        uniform float4 _RestingColor, _ShimmerColor, _ThirdColor;
        uniform float _Animated, _Elapsed, _AlphaCutoff, _FadeLength; 
        uniform float _TransitionState;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }
        
        
        void vertexDataFunc( inout appdata_full v, out Input o )
        {
            UNITY_INITIALIZE_OUTPUT( Input, o );
            float3 vert = v.vertex.xyz;
            float3 norm = v.normal.xyz;
            float mag = goopPlantVertexAnimation(v.vertex.xyz, _Elapsed, _Animated);       
            v.vertex.xyz += mag * norm; 
        }
        
        void surf (Input i, inout SurfaceOutput o) {
        
            float clipValue = glassSingleAlpha(i.uv_texcoord, _EffectMap, _EffectMap_ST, _TransitionState, _Real);
            clip(clipValue - _AlphaCutoff);
            
            float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
            float surfZ = -mul(UNITY_MATRIX_V, float4(i.worldPos.xyz, 1)).z;
            float diff = sceneZ - surfZ;
            float intersect = 1 - saturate(diff / _FadeLength);
            
            float intensity = goopShimmer(i.uv_texcoord, _AccentMap, _AccentMap_ST, _TransitionState, _Elapsed, _Real);
            float acc = tex2D(_AccentMap, i.uv_texcoord * _AccentMap_ST.xy + _AccentMap_ST.zw);
            
            acc = saturate(acc - intensity);
            acc = 1 - step(0.1,acc);
            
            
            
            float3 dark = _RestingColor.rgb * 0.4;

            o.Emission.rgb = lerp(_RestingColor.rgb, _ThirdColor.rgb, intersect);

            
            
            o.Emission += intensity * _ShimmerColor;

            o.Alpha = _RestingColor.a;
            o.Alpha = lerp(_RestingColor.a, _ThirdColor.a, intersect);
               
            
        }
        ENDCG
    }
    Fallback "Legacy Shaders/VertexLit"
}
