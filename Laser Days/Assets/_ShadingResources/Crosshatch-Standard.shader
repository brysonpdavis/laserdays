Shader "Crosshatch/Standard" {

    Properties {
        
        _MainTex("Material Map", 2D) = "white" {}
        _MainTexContribution("Material Intensity", Range(0,1)) = 0.1 
        _Highlights("Shininess", Range(0 , 1)) = 0
    
        [NoScaleOffset] _AccentMap("Accent Map", 2D) = "black" {}
        [NoScaleOffset] _EffectMap("Effect Map", 2D) = "white" {}
        
         _ShadingMap("Hatching Map", 2D) = "white" {}
        
        _RealBase("Real Base Color", Color) = (1,1,1,1)  
        _RealAccent("Real Accent Color", Color) = (0,0,0,0)
        _RealGradient("Real Gradient Color", Color) = (0,0,0,0)
        
        _LaserBase("Laser Base Color", Color) = (1,1,1,1)
        _LaserAccent("Laser Accent Color", Color) = (0,0,0,0)
        _LaserGradient("Laser Gradient Color", Color) = (0,0,0,0)
        
        _InteractColor("Interact Color", Color) = (0,0,0,0)
        [HDR]_ShimmerColor("Shimmer Color", Color) = (0,0,0,0)
        
        _LineA ("Outline ID", Range(0,8)) = 0
        _Smoothness("Outline Smoothing", Range(0,1)) = 0
        
        _GradientScale("Gradient Scale", Range(0, 3)) = 1
        _GradientOffset("Gradient Offset", Range(-3,3)) = 0
        
        [PerRendererData] _TransitionState("Transition State", Range(0,1)) = 0
        [PerRendererData] _TransitionStateB("Transition State B", Range( 0 , 1)) = 0
        
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        
        [HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
        [HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
        [HideInInspector]_Flippable("Flippable", Int) = 1
        [HideInInspector]_Elapsed("Elapsed", Float) = 0

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

            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON
            
            #pragma multi_compile SHARED REAL LASER
            #pragma multi_compile NO_GRADIENT HEIGHT_GRADIENT RADIAL_GRADIENT
            #pragma multi_compile _ ACCENT_ON
            #pragma multi_compile STATIC INTERACTABLE

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define FORWARD_BASE_PASS


            #include "CrosshatchStandardCG.cginc"

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
            
            #pragma multi_compile SHARED REAL LASER
            #pragma multi_compile NO_GRADIENT HEIGHT_GRADIENT RADIAL_GRADIENT
            #pragma multi_compile _ ACCENT_ON
            #pragma multi_compile STATIC INTERACTABLE
          
            #pragma multi_compile_fwdadd_fullshadows
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            

            #include "CrosshatchStandardCG.cginc"

            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "Deferred"
            }

            CGPROGRAM

            #pragma target 3.0
            #pragma exclude_renderers nomrt
            
            #pragma multi_compile SHARED REAL LASER
            #pragma multi_compile NO_GRADIENT HEIGHT_GRADIENT RADIAL_GRADIENT
            #pragma multi_compile _ ACCENT_ON
            #pragma multi_compile STATIC INTERACTABLE
            #pragma multi_compile _ UNITY_HDR_ON

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define DEFERRED_PASS
            #define SHARED

            #include "CrosshatchStandardCG.cginc"

            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM

            #pragma target 3.0



            #pragma multi_compile_shadowcaster
            #pragma multi_compile SHARED REAL LASER

            #pragma vertex MyShadowVertexProgram
            #pragma fragment MyShadowFragmentProgram
            

            #include "My Shadows.cginc"

            ENDCG
        }
    }
    
    
    CustomEditor "CrosshatchStandardGUI"
}