Shader "Crosshatch/TransparentBufferObject"
{
    Properties
    {
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

            struct v2f
            {
                float4 pos : SV_POSITION;

            };

            v2f vert (float3 v : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (float4(v,1));
                return o;

            }
            
            
            

            CBUFFER_START(UnityPerCamera2)
        
            CBUFFER_END
          
            half4 frag(v2f i) : COLOR1
            {
                
                half4 spec = float4(0, 0,0.5, 0.25);
                return spec;
                
                
            }
            ENDCG
        }       

    }

    Fallback Off
}
