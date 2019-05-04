Shader "Crosshatch/OutlineBuffer/Goop"
{
    Properties
    {
        [Toggle]_Real("REAL", float) = 0
        
        [HDR]_RestingColor("Resting Color", Color) = (0,0,0,0)
        [HDR]_ShimmerColor("Shimmer Color", Color) = (0,0,0,0)
        [HDR]_ActiveColor("Active Color", Color) = (0,0,0,0)
        
        
        [Toggle]_Animated("Use Vertex Animation", Float) = 0
        [HideInInspector]_Elapsed("Elapsed", Float) = 0
        
        _TriggerMap("Trigger Map", 2D) = "white" {}
        _EffectMap("Effect Map", 2D) = "white" {}
        
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
        
            sampler2D _CameraDepthTexture;
        
            uniform float _Real;
         
            uniform float4 _RestingColor, _ShimmerColor, _ActiveColor;
        
            uniform float _Animated, _Elapsed, _AlphaCutoff;
        
            uniform float _TransitionState, _isActive, isCollide;
        

            Interpolators vert (VertexData v)
            {
                Interpolators i;
                float4 temp = v.vertex;
                float mag = goopVertexAnimation(v.vertex.xyz, _Elapsed, _Animated);       
                temp.xyz += mag * v.normal;
                
                i.pos = UnityObjectToClipPos(temp);
                
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
