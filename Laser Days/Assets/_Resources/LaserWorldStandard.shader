Shader "Crosshatch/Laser-Standard" {

    Properties {
        
        _MainTex("Material Map", 2D) = "white" {}
    
        [NoScaleOffset] _AccentMap("Accent Map", 2D) = "black" {}
        [NoScaleOffset] _EffectMap("Effect Map", 2D) = "white" {}
        
        [HideInInspector]_RealBase("Real Base Color", Color) = (0,0,0,0)  
        [HideInInspector]_RealAccent("Real Accent Color", Color) = (0,0,0,0)
        
        _LaserBase("Laser Base Color", Color) = (0,0,0,0)
        _LaserAccent("Laser Accent Color", Color) = (0,0,0,0)
        
        _Smoothness("Smoothness", Range(0,1)) = 0
        
        [PerRendererData]_TransitionState("Transition State", Range(0,1)) = 0
        
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5

        [HideInInspector] _SrcBlend ("_SrcBlend", Float) = 1
        [HideInInspector] _DstBlend ("_DstBlend", Float) = 0
        [HideInInspector] _ZWrite ("_ZWrite", Float) = 1
    }

    CGINCLUDE

    #define BINORMAL_PER_FRAGMENT

    ENDCG

    SubShader {

        Pass {
            Tags {
                "LightMode" = "ForwardBase"
            }
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM

            #pragma target 3.0

            #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
            #pragma shader_feature _METALLIC_MAP
            #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
            #pragma shader_feature _NORMAL_MAP
            #pragma shader_feature _OCCLUSION_MAP
            #pragma shader_feature _EMISSION_MAP
            #pragma shader_feature _DETAIL_MASK
            #pragma shader_feature _DETAIL_ALBEDO_MAP
            #pragma shader_feature _DETAIL_NORMAL_MAP
            

            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define FORWARD_BASE_PASS
            #define USECLIP

            #include "CrosshatchCommon.cginc"

            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "ForwardAdd"
            }

            Blend [_SrcBlend] One
            ZWrite Off

            CGPROGRAM

            #pragma target 3.0

            #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
            #pragma shader_feature _METALLIC_MAP
            #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
            #pragma shader_feature _NORMAL_MAP
            #pragma shader_feature _DETAIL_MASK
            #pragma shader_feature _DETAIL_ALBEDO_MAP
            #pragma shader_feature _DETAIL_NORMAL_MAP

            #pragma multi_compile_fwdadd_fullshadows
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            
            #define USECLIP
            #include "CrosshatchCommon.cginc"

            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "Deferred"
            }

            CGPROGRAM

            #pragma target 3.0
            #pragma exclude_renderers nomrt

            #pragma shader_feature _ _RENDERING_CUTOUT
            #pragma shader_feature _METALLIC_MAP
            #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
            #pragma shader_feature _NORMAL_MAP
            #pragma shader_feature _OCCLUSION_MAP
            #pragma shader_feature _EMISSION_MAP
            #pragma shader_feature _DETAIL_MASK
            #pragma shader_feature _DETAIL_ALBEDO_MAP
            #pragma shader_feature _DETAIL_NORMAL_MAP
            
            #pragma shader_feature _ACCENT_MAP
            
            #pragma multi_compile _ UNITY_HDR_ON

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define DEFERRED_PASS
            #define USECLIP
           

            #include "CrosshatchCommon.cginc"

            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
            #pragma shader_feature _SEMITRANSPARENT_SHADOWS
            #pragma shader_feature _SMOOTHNESS_ALBEDO

            #pragma multi_compile_shadowcaster

            #pragma vertex MyShadowVertexProgram
            #pragma fragment MyShadowFragmentProgram
            
            #define USECLIP
            //#define REAL

            #include "My Shadows.cginc"

            ENDCG
        }
    }

    //CustomEditor "MyLightingShaderGUI"
}