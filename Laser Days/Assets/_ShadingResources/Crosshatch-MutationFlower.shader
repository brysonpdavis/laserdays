Shader "Crosshatch/MutationFlower" {

    Properties {
        
        _MainTex("Mutation Map", 2D) = "white" {}
        _MainTexContribution("Material Intensity", Range(0,1)) = 0.1 
        _Highlights("Shininess", Range(0 , 1)) = 0
    
        [NoScaleOffset] _AccentMap("Accent Map", 2D) = "black" {}
        [NoScaleOffset] _EffectMap("Effect Map", 2D) = "white" {}
        
        [HDR]_BeginColor("Staring Color", Color) = (1,1,1,1)
        [HDR]_BaseColor("Base Color", Color) = (1,1,1,1)
        [HDR]_DeathColor("Death Color", Color) = (1,1,1,1)
        [HDR]_AccentColor("Accent Color", Color) = (1,1,1,1)
        
        [PerRendererData] _TintColor("Tint Color", Color) = (1,1,1,1)
        
        _LineA ("Outline ID", Range(0,8)) = 0
        _Smoothness("Outline Smoothing Normals", Range(0,1)) = 0
        _Smoothness2("Outline Smoothing Depth", Range(0,1)) = 0
        
        [PerRendererData]_BeginToBase("Begginning to Base", Range(0,1)) = 0
        [PerRendererData]_BaseToDeath("_BaseToDeath", Range(0,1)) = 0
        
        [PerRendererData] _TransitionState("Transition State", Range(0,1)) = 0
        [PerRendererData] _TransitionStateB("Transition State B", Range( 0 , 1)) = 0
        
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5

        [HideInInspector] _Elapsed("Elapsed", Float) = 0

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
            #pragma multi_compile _ ACCENT_ON

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define FORWARD_BASE_PASS
            
            #include "CrosshatchMutationFlowerCG.cginc"

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
            #pragma multi_compile _ ACCENT_ON
          
            #pragma multi_compile_fwdadd_fullshadows
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            

            #include "CrosshatchMutationFlowerCG.cginc"

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
            #pragma multi_compile _ ACCENT_ON
            #pragma multi_compile _ UNITY_HDR_ON

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define DEFERRED_PASS

            #include "CrosshatchMutationFlowerCG.cginc"

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
            

            #include "My ShadowsAlpha.cginc"

            ENDCG
        }
    }
    
    
    CustomEditor "CrosshatchMutationFlowerGUI"
}