Shader "Crosshatch/OutlineBuffer/Crystal"
{
    Properties
    {
        
        _EffectMap("Effect Map", 2D) = "white" {}
        _OffsetMag("Expand Size", Float) = 1
        
        _TransitionState("Transition State", Range( 0 , 1)) = 0
        
        
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
            

            uniform float  _AlphaCutoff, _TransitionState, _OffsetMag;
            sampler2D _EffectMap;
            float4 _EffectMap_ST;
        

            Interpolators vert (VertexData v)
            {
                Interpolators i;
                float4 temp = v.vertex;
                float mag = crystalVertexAnimation(v.vertex.xyz, _TransitionState, _OffsetMag);
                temp.xyz += mag * v.normal;
                
                i.pos = UnityObjectToClipPos(temp);
                
                i.uv.xy = v.uv;
                
                return i;
                
            }
            

            CBUFFER_START(UnityPerCamera2)
        
            CBUFFER_END
          
            half4 frag(Interpolators i) : COLOR1
            {
                
                float clipFactor = crystalAlpha(i.uv.xy, _EffectMap, _EffectMap_ST, _TransitionState);
                clip (clipFactor - _AlphaCutoff);
                
                half4 spec = float4(0, 0,0.5, 0.25);
                
                
                return spec;
                
                
            }
            ENDCG
        }       

    }

    Fallback Off
}
