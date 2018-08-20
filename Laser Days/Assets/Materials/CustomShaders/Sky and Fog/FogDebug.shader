Shader "Hidden/5ColorFog"
{
    HLSLINCLUDE

        #include "PostProcessing/Shaders/StdLib.hlsl"
       
        #define SKYBOX_THREASHOLD_VALUE 0.999
             
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

        float _FogDensity;

        float4 _Color1;
        float _Cutoff1;
        float _Sharp1;

        float4 _Color2;
        float _Cutoff2;
        float _Sharp2;

        float4 _Color3;
        float _Cutoff3;
        float _Sharp3;

        float4 _Color4;
        float _Cutoff4;
        float _Sharp4;

        float4 _Color5;

        struct VS_Input{
            float4 vertPos : POSITION;
            float4 vertUV : TEXCOORD0;
        };

        struct VS_Output{
            float4 fragPos : SV_POSITION;
            float2 fragUV : TEXCOORD0;
            float2 fragUVDepth : TEXCOORD1;
        };


        VS_Output Vert(VS_Input v){
            VS_Output o;
            o.fragPos = float4(v.vertPos.xy, 0.0, 1.0);
            o.fragUV = TransformTriangleVertexToUV(v.vertPos.xy);
            o.fragUVDepth = TransformTriangleVertexToUV(v.vertPos.xy);

            #if UNITY_UV_STARTS_AT_TOP
                o.fragUV = o.fragUV * float2(1.0, -1.0) + float2(0.0, 1.0);
                o.fragUVDepth = o.fragUVDepth * float2(1.0, -1.0) + float2(0.0, 1.0);
            #endif

            return o;
        }


        float ComputeDistance(float depth)
        {
            float dist = 0.0;
            dist = depth * _ProjectionParams.z;
            dist -= _ProjectionParams.y;
            return dist;
        }

        float ComputeFog(float coord)
        {
            float fog = 0.0;
            fog = _FogDensity * coord;
            fog = exp2(-fog);
            saturate(fog);
            return fog; 
        }


        float4 Frag(VS_Output i) : SV_Target
        {

           float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.fragUV);

           float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.fragUVDepth);

           depth = Linear01Depth(depth);

           float4 fogColor;
           float r;

           float p1 = _Cutoff1;
           float p2 = p1 + _Cutoff2;
           float p3 = p2 + _Cutoff3;
           float p4 = p3 + _Cutoff4;


           if(depth < p1)
           {
                r = depth/_Cutoff1;
                r = max(r, 0.0);
                r = pow(r, _Sharp1);
                fogColor = lerp(_Color1, _Color2, r);
                fogColor.a = _Color1.a;
                //fogColor = _Color1;

           }

           if (depth >= p1 && depth < p2)
           {
              r = (depth - p1)/_Cutoff2;
              r = max(r,0.0);
              r = pow(r, _Sharp2);
              fogColor = lerp(_Color2, _Color3, r);
              fogColor.a = _Color2.a;
              //fogColor = _Color2;
           }

           if (depth >= p2 && depth < p3)
           {
              r = (depth - p2)/_Cutoff3;
              r = max(r,0.0);
              r = pow(r, _Sharp3);
              fogColor = lerp(_Color3, _Color4, r);
              fogColor.a = _Color3.a;
              //fogColor = _Color3;
           }

           if (depth >= p3 && depth < p4)
           {
              r = (depth - p3)/_Cutoff4;
              r = max(r,0.0);
              r = pow(r, _Sharp4);
              fogColor = lerp(_Color4, _Color5, r);
              fogColor.a = _Color4.a;
              //fogColor = _Color4;

           }

           if (depth >= p4)
           {
                fogColor = _Color5;
                fogColor.a = _Color5.a;
           }
                


           float dist = ComputeDistance(depth);
         
           float fog = ComputeFog(dist);
           fog = 1 - fog;

           float sky = 1;
           if (depth >= SKYBOX_THREASHOLD_VALUE){
           sky = 0;
           }



           return lerp(sceneColor, fogColor, (fog*sky));

           

        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag

            ENDHLSL
        }
    }
}










