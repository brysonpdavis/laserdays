Shader "Custom/PortalCard" {
	Properties {
		
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _DepthFade("Depth Fade Amount", Range(0, 100)) = 1
        _CamFade("Camera Fade Amount", Range(0,100)) = 1
        _CamFadeOffset("Offset", Range(0,100)) = 1

	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector" = "True" "IsEmissive" = "true" }
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
        #pragma target 3.0
        

		struct Input {
			float4 screenPos;
            float3 worldPos;
            float eyeDepth;
		};

		fixed4 _Color;
        sampler2D _CameraDepthTexture;
        float _DepthFade, _CamFade, _CamFadeOffset;
        
        void vertexDataFunc( inout appdata_full v, out Input o )
        {
            UNITY_INITIALIZE_OUTPUT( Input, o );
            o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
        }
        
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }

		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input i, inout SurfaceOutput o) {
        
            float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
            float surfZ = -mul(UNITY_MATRIX_V, float4(i.worldPos.xyz, 1)).z;
            float diff = sceneZ - surfZ;
            float intersect = saturate(diff / _DepthFade);
            
            float camDiff = saturate((i.eyeDepth - _ProjectionParams.y - _CamFadeOffset) / _CamFade); 
            
            o.Alpha = _Color.a * intersect * camDiff;
            o.Emission.rgb = _Color.rgb;

		}
		ENDCG
	}
    Fallback "Legacy Shaders/VertexLit"
}
