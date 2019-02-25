Shader "Crosshatch/Patch"
{
	Properties
	{
		_MainTex ("Patch Map", 2D) = "white" {}
        _RealColor("Real Color", Color) = (0,0,0,0)
        _LaserColor("Laser Color", Color) = (0,0,0,0)
        _TransitionState("Transition State", Range(0,1)) = 0
	}
	SubShader
	{
		Pass
		{
			Fog { Mode Off } // no fog in g-buffers pass
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers nomrt
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float4 screenUV : TEXCOORD1;
				float3 ray : TEXCOORD2;
				half3 orientation : TEXCOORD3;
			};

			v2f vert (float3 v : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (float4(v,1));
				o.uv = v.xz+0.5;
				o.screenUV = ComputeScreenPos (o.pos);
				o.ray = mul (UNITY_MATRIX_MV, float4(v,1)).xyz * float3(-1,-1,1);
				o.orientation = mul ((float3x3)unity_ObjectToWorld, float3(0,1,0));
				return o;
			}
            
            
            

			CBUFFER_START(UnityPerCamera2)
		
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;
			sampler2D _NormalsCopy;
            float4 _RealColor;
            float4 _LaserColor;
            float _TransitionState;
            
            
            float getMask(v2f i)
            {
                float3 tex = tex2D(_MainTex, i.uv);
                float mask = lerp(tex.r, tex.g, _TransitionState);
                return step(0.1, mask); 
            }
            
            float4 getCol(v2f i)
            {
                return lerp (_RealColor, _LaserColor, _TransitionState);
            }
            

			void frag(v2f i, out half4 outDiffuse : COLOR0, out half4 outExtra : COLOR1)
			{
				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				float2 uv = i.screenUV.xy / i.screenUV.w;
				// read depth and reconstruct world position
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth (depth);
				float4 vpos = float4(i.ray * depth,1);
				float3 wpos = mul (unity_CameraToWorld, vpos).xyz;
				float3 opos = mul (unity_WorldToObject, float4(wpos,1)).xyz;

				clip (float3(0.5,0.5,0.5) - abs(opos.xyz));


				i.uv = opos.xz+0.5;

				half3 normal = tex2D(_NormalsCopy, uv).rgb;
                half alph = getMask(i);
				fixed3 wnormal = normal.rgb * 2.0 - 1.0;
				clip (dot(wnormal, i.orientation) - 0.3);
			    outDiffuse = half4(getCol(i).rgb, alph);
                
                float4 spec = float4(1, 0,0, getMask(i));
                outExtra = spec;
			}
			ENDCG
		}		

	}

	Fallback Off
}
