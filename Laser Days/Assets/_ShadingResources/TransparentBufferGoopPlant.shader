Shader "Crosshatch/OutlineBuffer/Goop-PlantVariant"
{
    Properties
    {
        [Toggle]_Real("REAL", float) = 0
        
        [HideInInspector]_Elapsed("Elapsed", Float) = 0
        
        _AccentMap("Accent Map", 2D) = "black" {}
        _EffectMap("Effect Map", 2D) = "white" {}
        
        _TransitionState("Transition State", Range( 0 , 1)) = 0
        
        [Toggle]_Animated("Use Vertex Animation", Float) = 0
        
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
                float3 worldPos : TEXCOORD2;
                float4 uv : TEXCOORD0; 
            };
            
            sampler2D _AccentMap, _EffectMap;
            float4 _AccentMap_ST, _EffectMap_ST;
        
            uniform float _Real;
        
            uniform float _Elapsed, _AlphaCutoff, _Animated;
        
            uniform float _TransitionState;
        

            Interpolators vert (VertexData v)
            {
                Interpolators i;
                float4 temp = v.vertex;
                float mag = goopPlantVertexAnimation(v.vertex.xyz, _Elapsed, _Animated);       
                temp.xyz += mag * v.normal;
                
                i.worldPos = temp.xyz;
                i.pos = UnityObjectToClipPos(temp);
                
                i.uv.xy = v.uv;
                
                return i;
                
            }
            

            CBUFFER_START(UnityPerCamera2)
        
            CBUFFER_END
          
            half4 frag(Interpolators i) : COLOR1
            {
                
                float clipFactor = glassSingleAlpha(i.uv.xy, _EffectMap, _EffectMap_ST, _TransitionState, _Real);
                clip (clipFactor - _AlphaCutoff);
                
                float4 spec = float4(0, 0,0.5, 0.25);
                
                float shimmer = tex2D(_AccentMap, i.uv.xy *_AccentMap_ST.xy + _AccentMap_ST.zw);
                float intensity = goopShimmer(i.uv.xy, _AccentMap, _AccentMap_ST, _TransitionState, _Elapsed, _Real);
                shimmer = saturate(shimmer - intensity);
                shimmer = 1 - step(0.1, shimmer);
                
                float ving = float4(shimmer * 0.25,0,0,0);
                spec += ving;
                
                return spec;
                
                
            }
            ENDCG
        }       

    }

    Fallback Off
}
