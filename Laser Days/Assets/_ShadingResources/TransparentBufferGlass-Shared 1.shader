Shader "Crosshatch/OutlineBuffer/Glass-Blocker"
{
    Properties
    {

        _AccentMap("Accent Map", 2D) = "black" {}
                                            
        _TransitionState("Transition State", Range( 0 , 1)) = 0      
        
         _Elapsed("Elapsed", float) = 0
                
        [HideInInspector] _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5
        
        [HideInInspector] _texcoord( "", 2D ) = "white" {}      
    }
    
    
    SubShader
    {
        Pass
        {
            Fog { Mode Off } // no fog in g-buffers pass
            ZWrite Off
            Cull Off
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
                float4 worldPos : TEXCOORD1;
            };
            
            sampler2D _AccentMap;
            float4 _AccentMap_ST;
        
        
            uniform float _TransitionState, _Elapsed, _AlphaCutoff;
        

            Interpolators vert (VertexData v)
            {
                Interpolators i;
                float4 temp = v.vertex;
                i.pos = UnityObjectToClipPos(temp);
                i.worldPos = mul(unity_ObjectToWorld, v.vertex);
                i.uv.xy = v.uv;
                return i;
                
            }
            

            CBUFFER_START(UnityPerCamera2)
        
            CBUFFER_END
          
            half4 frag(Interpolators i) : COLOR1
            {
                clip(_AlphaCutoff -  blockerPlayerHole(i.uv, _AccentMap, _AccentMap_ST, _WorldSpaceCameraPos, i.worldPos.xyz));
                
                half acc = blockerAccent(i.uv, _AccentMap, _AccentMap_ST, _TransitionState, _Elapsed);
                
                return half4(acc * 0.25, 0, 0.5, 0.25);
                
                
            }
            ENDCG
        }       

    }

    Fallback Off
}
