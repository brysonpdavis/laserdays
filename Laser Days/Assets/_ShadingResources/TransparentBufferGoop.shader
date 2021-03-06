﻿Shader "Crosshatch/OutlineBuffer/Goop-Trigger"
{
    Properties
    {
        [Toggle]_Real("REAL", float) = 0
        [Toggle]_Animated("Use Vertex Animation", Float) = 0
        
        [HideInInspector]_Elapsed("Elapsed", Float) = 0
        
        _TriggerMap("Goop Map", 2D) = "white" {}
        
        _TransitionState("Transition State", Range( 0 , 1)) = 0
        
        [HideInInspector]_isActive("isActive", Range( 0 , 1)) = 0
        [HideInInspector]_isCollide("isCollide", Range( 0 , 1)) = 0
        
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5

        [HideInInspector] _texcoord( "", 2D ) = "white" {}
    }
    
    
    SubShader
    {
        Pass
        {
            Fog { Mode Off } // no fog in g-buffers pass
            ZWrite Off
            Cull Back
            //Blend One One , SrcAlpha OneMinusSrcAlpha
            Blend One One
            

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers nomrt
            
            #include "UnityCG.cginc"
            #include "TransparentHelper.cginc"
            
            struct VertexData 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Interpolators
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0; 
            };
            
            sampler2D _TriggerMap;
            float4 _TriggerMap_ST;
        
            uniform float _Real;
  
            uniform float _Animated, _Elapsed, _AlphaCutoff;
        
            uniform float _TransitionState;
           

            Interpolators vert (VertexData v)
            {
                Interpolators i;
                float3 newPos = v.vertex.xyz;
                float3 movement = goopVertexAnimation(v.vertex.xyz, v.normal.xyz, _Elapsed, _Animated);       
                newPos += movement;
                
                i.pos = UnityObjectToClipPos(newPos);
                i.uv.xy = v.uv;
                
                return i;
            }
            

            CBUFFER_START(UnityPerCamera2)
        
            CBUFFER_END
          
            half4 frag(Interpolators i) : COLOR1
            {
                
                float clipFactor = goopAlpha(i.uv.xy, _TriggerMap, _TriggerMap_ST, _TransitionState, _Elapsed, _Real);
                clip (clipFactor - _AlphaCutoff);
                
                half4 spec = float4(0, 0,0.5, 0.25);
                half shimmer = goopShimmer(i.uv.xy, _TriggerMap, _TriggerMap_ST, _TransitionState, _Elapsed, _Real);
                half4 ving = float4(shimmer * 0.25,0,0,0);
                spec += ving;
                
                return spec;
                
                
            }
            ENDCG
        }       

    }

    Fallback Off
}
