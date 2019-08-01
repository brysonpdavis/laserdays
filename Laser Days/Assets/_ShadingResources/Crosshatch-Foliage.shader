Shader "Crosshatch/Standard-Foliage" {

    Properties {
        
        _MainTex("Material Map", 2D) = "white" {}
        _MainTexContribution("Material Intensity", Range(0,1)) = 0.1 
        _Highlights("Shininess", Range(0 , 1)) = 0
    
        [NoScaleOffset] _AccentMap("Accent Map", 2D) = "black" {}
        [NoScaleOffset] _EffectMap("Effect Map", 2D) = "white" {}
        [NoScaleOffset] _ShadingMap("Hatching Map", 2D) = "white" {}
         
         _BlendOffset("Triplanar Blend Offset", Range(0,1)) = 0
         _TerrainScale("Triplanar Tex Scale", Float) = 1
        
        _RealBase("Real Base Color", Color) = (1,1,1,1)  
        _RealAccent("Real Accent Color", Color) = (0,0,0,0)
        _RealGradient("Real Gradient Color", Color) = (0,0,0,0)
        [HDR]_RealEmission("Real Emission Color", Color) = (0,0,0,0)
        
        _LaserBase("Laser Base Color", Color) = (1,1,1,1)
        _LaserAccent("Laser Accent Color", Color) = (0,0,0,0)
        _LaserGradient("Laser Gradient Color", Color) = (0,0,0,0)
        [HDR]_LaserEmission("Laser Emission Color", Color) = (0,0,0,0)
        
        _InteractColor("Interact Color", Color) = (0,0,0,0)
        [HDR]_ShimmerColor("Shimmer Color", Color) = (0,0,0,0)
        
        _LineA ("Outline ID", Range(0,8)) = 0
        _Smoothness("Outline Smoothing Normals", Range(0,1)) = 0
        _Smoothness2("Outline Smoothing Depth", Range(0,1)) = 0
        
        _GradientScale("Gradient Scale", Range(0, 3)) = 1
        _GradientOffset("Gradient Offset", Range(-3,3)) = 0
        
        _AnimationMagnitude("Animation Magnitude", Float) = 0
        _AnimationSpeed("Animation Speed", Float) = 1
        _AnimationWaveSize("Animation Waze Size", Float) = 1
        _AnimationSway("Animation Sway direction", Vector) = (0,1,0,0)
        _AnimationTimeStep("Animation Time Step", Float) = 1
        
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
            #pragma shader_feature NO_GRADIENT HEIGHT_GRADIENT RADIAL_GRADIENT
            #pragma shader_feature _ WORLD_POS_GRADIENT
            #pragma shader_feature _ ACCENT_ON
            #pragma shader_feature _ EMISSIVE
            #pragma shader_feature STATIC INTERACTABLE TERRAIN INVERSE_INTERACTABLE
            #pragma shader_feature NO_ANIMATION W_POS_SWELLING W_POS_SWAYING V_COLOR_SWAYING V_COLOR_GLITCHY V_COLOR_CIRCLE

            #pragma vertex MyVertexProgramFoliage
            #pragma fragment MyFragmentProgram
            
            #define FOLIAGE
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
            #pragma shader_feature NO_GRADIENT HEIGHT_GRADIENT RADIAL_GRADIENT
            #pragma shader_feature _ WORLD_POS_GRADIENT
            #pragma shader_feature _ ACCENT_ON
            #pragma shader_feature _ EMISSIVE
            #pragma shader_feature STATIC INTERACTABLE TERRAIN INVERSE_INTERACTABLE
            #pragma shader_feature NO_ANIMATION W_POS_SWELLING W_POS_SWAYING V_COLOR_SWAYING V_COLOR_GLITCHY V_COLOR_CIRCLE
          
            #pragma multi_compile_fwdadd_fullshadows
            
            #pragma vertex MyVertexProgramFoliage
            #pragma fragment MyFragmentProgram
            
            #define FOLIAGE
            
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
            #pragma shader_feature NO_GRADIENT HEIGHT_GRADIENT RADIAL_GRADIENT
            #pragma shader_feature _ WORLD_POS_GRADIENT
            #pragma shader_feature _ ACCENT_ON
            #pragma shader_feature _ EMISSIVE
            #pragma shader_feature STATIC INTERACTABLE TERRAIN INVERSE_INTERACTABLE
            #pragma shader_feature NO_ANIMATION W_POS_SWELLING W_POS_SWAYING V_COLOR_SWAYING V_COLOR_GLITCHY V_COLOR_CIRCLE
            #pragma multi_compile _ UNITY_HDR_ON

            #pragma vertex MyVertexProgramFoliage
            #pragma fragment MyFragmentProgram

            #define DEFERRED_PASS
            #define FOLIAGE

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
            #pragma shader_feature NO_ANIMATION W_POS_SWELLING W_POS_SWAYING V_COLOR_SWAYING V_COLOR_GLITCHY V_COLOR_CIRCLE
            #define FOLIAGE
            
            #pragma vertex MyShadowVertexProgramFoliage
            #pragma fragment MyShadowFragmentProgram
            

            #include "My Shadows.cginc"

            ENDCG
        }
    }
    
    
    CustomEditor "CrosshatchStandardFoliageGUI"
}