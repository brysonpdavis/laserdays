
Shader "Hidden/EdgeDetect" { 
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};
	
	struct v2fd {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	half4 _MainTex_ST;

	sampler2D _CameraDepthNormalsTexture;
	half4 _CameraDepthNormalsTexture_ST;
    
    sampler2D _CameraGBufferTexture1;
    half4 _CameraGBufferTexture1_ST;

	sampler2D_float _CameraDepthTexture;
	half4 _CameraDepthTexture_ST;

	uniform half4 _Sensitivity; 
	uniform half4 _BgColor;
	uniform half _BgFade;
	uniform half _SampleDistance;
	uniform float _Exponent;

	uniform float _Threshold;
    
    uniform float _PauseMenu;

	struct v2flum {
		float4 pos : SV_POSITION;
		float2 uv[3] : TEXCOORD0;
	};

	v2flum vertLum (appdata_img v)
	{
		v2flum o;
		o.pos = UnityObjectToClipPos(v.vertex);
		float2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
		o.uv[0] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);
		o.uv[1] = UnityStereoScreenSpaceUVAdjust(uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance, _MainTex_ST);
		o.uv[2] = UnityStereoScreenSpaceUVAdjust(uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance, _MainTex_ST);
		return o;
	}


	fixed4 fragLum (v2flum i) : SV_Target
	{
		fixed4 original = tex2D(_MainTex, i.uv[0]);

		// a very simple cross gradient filter

		half3 p1 = original.rgb;
		half3 p2 = tex2D(_MainTex, i.uv[1]).rgb;
		half3 p3 = tex2D(_MainTex, i.uv[2]).rgb;
		
		half3 diff = p1 * 2 - p2 - p3;
		half len = dot(diff, diff);
		len = step(len, _Threshold);
		//if(len >= _Threshold)
		//	original.rgb = 0;

		return len * lerp(original, _BgColor, _BgFade);			
	}	
	
    inline half CheckSameExtra (float4 centerExtra, half4 theSampleA, half4 theSampleB)
    {
        float3 diff = centerExtra.rgb * 2 - theSampleA.rgb - theSampleB.rgb; 
        float len = dot(diff,diff);
        len = step(len, 0.05);
        return len;
        
  
        
    }   
    
	inline half CheckSame (half2 centerNormal, float centerDepth, half4 theSample)
	{
		// difference in normals
		// do not bother decoding normals - there's no need here
		half2 diff = abs(centerNormal - theSample.xy) * _Sensitivity.y;
		int isSameNormal = (diff.x + diff.y) * _Sensitivity.y < 0.1;
		// difference in depth
		float sampleDepth = DecodeFloatRG (theSample.zw);
		float zdiff = abs(centerDepth-sampleDepth);
		// scale the required threshold by the distance
		int isSameDepth = zdiff * _Sensitivity.x < 0.09 * centerDepth;
	
		// return:
		// 1 - if normals and depth are similar enough
		// 0 - otherwise
		
		return isSameNormal * isSameDepth ? 1.0 : 0.0;
	}	
		
	v2f vertRobert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
				
		// calc coord for the X pattern
		// maybe nicer TODO for the future: 'rotated triangles'
		
		o.uv[1] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(1,1) * _SampleDistance, _MainTex_ST);
		o.uv[2] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(-1,-1) * _SampleDistance, _MainTex_ST);
		o.uv[3] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(-1,1) * _SampleDistance, _MainTex_ST);
		o.uv[4] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(1,-1) * _SampleDistance, _MainTex_ST);
				 
		return o;
	} 
	
	v2f vertThin( appdata_img v )
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
		
		o.uv[1] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);
		o.uv[4] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);
				
		// offsets for two additional samples
		o.uv[2] = UnityStereoScreenSpaceUVAdjust(uv + float2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance, _MainTex_ST);
		o.uv[3] = UnityStereoScreenSpaceUVAdjust(uv + float2(+_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _SampleDistance, _MainTex_ST);
		
		return o;
	}	  
	 
	v2fd vertD( appdata_img v )
	{
		v2fd o;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = uv;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
		
		o.uv[1] = uv;
		
		return o;
	}

	float4 fragDCheap(v2fd i) : SV_Target 
	{	
		// inspired by borderlands implementation of popular "sobel filter"

		float centerDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[1]));
		float4 depthsDiag;
		float4 depthsAxis;

		float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;

		depthsDiag.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist, _CameraDepthTexture_ST))); // TR
		depthsDiag.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(-1,1), _CameraDepthTexture_ST))); // TL
		depthsDiag.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(-1,1), _CameraDepthTexture_ST))); // BR
		depthsDiag.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist, _CameraDepthTexture_ST))); // BL

		depthsAxis.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(0,1), _CameraDepthTexture_ST))); // T
		depthsAxis.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(1,0), _CameraDepthTexture_ST))); // L
		depthsAxis.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(1,0), _CameraDepthTexture_ST))); // R
		depthsAxis.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(0,1), _CameraDepthTexture_ST))); // B

		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		Sobel = 1.0-pow(saturate(Sobel), _Exponent);
		return Sobel * lerp(tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv[0].xy, _MainTex_ST)), _BgColor, _BgFade);
	}

	// pretty much also just a sobel filter, except for that edges "outside" the silhouette get discarded
	//  which makes it compatible with other depth based post fx

	float4 fragD(v2fd i) : SV_Target 
	{	
		// inspired by borderlands implementation of popular "sobel filter"

		float centerDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1], _CameraDepthTexture_ST)));
		float4 depthsDiag;
		float4 depthsAxis;

		float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;

		depthsDiag.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist, _CameraDepthTexture_ST))); // TR
		depthsDiag.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(-1,1), _CameraDepthTexture_ST))); // TL
		depthsDiag.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(-1,1), _CameraDepthTexture_ST))); // BR
		depthsDiag.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist, _CameraDepthTexture_ST))); // BL

		depthsAxis.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(0,1), _CameraDepthTexture_ST))); // T
		depthsAxis.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(1,0), _CameraDepthTexture_ST))); // L
		depthsAxis.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]+uvDist*float2(1,0), _CameraDepthTexture_ST))); // R
		depthsAxis.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(i.uv[1]-uvDist*float2(0,1), _CameraDepthTexture_ST))); // B

		// make it work nicely with depth based image effects such as depth of field:
		depthsDiag = (depthsDiag > centerDepth.xxxx) ? depthsDiag : centerDepth.xxxx;
		depthsAxis = (depthsAxis > centerDepth.xxxx) ? depthsAxis : centerDepth.xxxx;

		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		Sobel = 1.0-pow(saturate(Sobel), _Exponent);
		return Sobel * lerp(tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv[0].xy, _MainTex_ST)), _BgColor, _BgFade);
	}

	half4 fragRobert(v2f i) : SV_Target {				
		half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1].xy);
		half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2].xy);
		half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3].xy);
		half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4].xy);

		half edge = 1.0;
		
		edge *= CheckSame(sample1.xy, DecodeFloatRG(sample1.zw), sample2);
		edge *= CheckSame(sample3.xy, DecodeFloatRG(sample3.zw), sample4);

		return edge * lerp(tex2D(_MainTex, i.uv[0]), _BgColor, _BgFade);
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
            fog = 0.03 * coord;
            fog = exp2(-fog);
            saturate(fog);
            return fog; 
        }

	half4 fragThin (v2f i) : SV_Target
	{
		half4 original = tex2D(_MainTex, i.uv[0]);
		
		half4 center = tex2D (_CameraDepthNormalsTexture, i.uv[1]);
		half4 sample1 = tex2D (_CameraDepthNormalsTexture, i.uv[2]);
		half4 sample2 = tex2D (_CameraDepthNormalsTexture, i.uv[3]);
        
        half4 centerExtra = tex2D (_CameraGBufferTexture1, i.uv[1]);
        half4 sample1A = tex2D (_CameraGBufferTexture1, i.uv[2]);
        half4 sample2B = tex2D (_CameraGBufferTexture1, i.uv[3]);
		
		// encoded normal
		half2 centerNormal = center.xy;
		// decoded depth
		float centerDepth = DecodeFloatRG (center.zw);
        
        float CEa = centerExtra.r;
        
		
		half edge = 1.0;
        half4 white = (1,1,1,1);
        half4 black  = (0,0,0,0);
		
		edge *= CheckSame(centerNormal, centerDepth, sample1);
		edge *= CheckSame(centerNormal, centerDepth, sample2);
        edge *= CheckSameExtra(centerExtra, sample1A, sample2B);
        
         //return edge;
        edge = 1-edge;

        float f = ComputeDistance(centerDepth);
        f = ComputeFog(f);
        //step()
        // f = 1 - f;
        if(centerDepth>0.999){
        f = 1;
        }
			
       
        _PauseMenu = saturate(_PauseMenu);
        float4 blu = float4(0,0.64,0.85,1); 
        float4 pm = lerp(0,1,(edge*f));   
		float4 sc = lerp(original, _BgColor, (edge*f*_BgColor.a));
        return lerp(sc, pm, _PauseMenu);
        //return lerp(original, _BgColor, (edge));
	}
	
	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vertThin
      #pragma fragment fragThin
      ENDCG
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vertRobert
      #pragma fragment fragRobert
      ENDCG
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertD
      #pragma fragment fragDCheap
      ENDCG
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertD
      #pragma fragment fragD
      ENDCG
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertLum
      #pragma fragment fragLum
      ENDCG
  }
}

Fallback off
	
} // shader
