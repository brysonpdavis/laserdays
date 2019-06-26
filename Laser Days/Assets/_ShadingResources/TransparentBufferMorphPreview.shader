Shader "Crosshatch/OutlineBuffer/MorphPreview"
{
    Properties
    {
        [Toggle]_Real("Is Real", float) = 1
        _onHold("On Hold", float) = 0                                          
        _TransitionState("Transition State", Range( 0 , 1)) = 0      
                
        [HideInInspector] _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5
        
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
                 
        
            uniform float _TransitionState, _Real, _AlphaCutoff, _onHold;
        
            Interpolators vert (VertexData v)
            {
                Interpolators i;
                float4 temp = v.vertex;
                i.pos = UnityObjectToClipPos(temp);
                i.uv.xy = v.uv;
                return i;
                
            }
            

            CBUFFER_START(UnityPerCamera2)
        
            CBUFFER_END
          
            half4 frag(Interpolators i) : COLOR1
            {
                clip(_onHold - _AlphaCutoff);
                half worldClip = lerp(_TransitionState, 1 - _TransitionState, _Real);
                clip(worldClip - _AlphaCutoff);
                
                return half4(0, 0, 0.5, 0.1);
                
                
            }
            ENDCG
        }       

    }

    Fallback Off
}
