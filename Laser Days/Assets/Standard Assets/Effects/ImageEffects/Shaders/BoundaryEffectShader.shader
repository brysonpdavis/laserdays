Shader "Hidden/BoundaryEffect" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
        _VignetteColor("Vignette Color", Color) = (0,0,0,0)
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv2 : TEXCOORD1;
	};
	
	sampler2D _MainTex;
	
	half _Intensity;
    
    float4 _VignetteColor;

	float4 _MainTex_TexelSize;
	half4  _MainTex_ST;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv2 = v.texcoord.xy;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			 o.uv2.y = 1.0 - o.uv2.y;
		#endif

		return o;
	} 
	
	half4 frag(v2f i) : SV_Target {
		half2 coords = i.uv;
		half2 uv = i.uv;

		half4 color = tex2D (_MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));

		half mask = _VignetteColor.a * _Intensity; 
		
		return lerp(color, _VignetteColor, mask);
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}

Fallback off	
} 
