Shader "ColorReplacement" {

    Properties {
        ReplacementColor ("ReplacementColor", Color) = (0,0,0,1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
    }
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			float4 ReplacementColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return ReplacementColor;
			}
			ENDCG
		}
	}
}