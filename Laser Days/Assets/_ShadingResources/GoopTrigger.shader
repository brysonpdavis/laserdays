Shader "Crosshatch/Goop-Trigger" {
	
    Properties {
		
        [Toggle]_Real("REAL", float) = 0
        [Toggle]_Animated("Use Vertex Animation", Float) = 0
         _TriggerMap("Goop Map", 2D) = "white" {}
        
        [HDR]_RestingColor("Resting Color", Color) = (0,0,0,0)
        [HDR]_ActiveColor("Active Color", Color) = (0,0,0,0)
        [HDR]_ShimmerColor("Shimmer Color", Color) = (0,0,0,0)
        
        _FadeLength("_FadeLength", Range(0,3)) = 0.1
                
        [HideInInspector]_Elapsed("Elapsed", Float) = 0
           
        _TransitionState("Transition State", Range( 0 , 1)) = 0
        
        [HideInInspector]_isActive("isActive", Range( 0 , 1)) = 0
        [HideInInspector]_isCollide("isCollide", Range( 0 , 1)) = 0
        [HideInInspector]_isLineRender("_isLineRender", Range ( 0 , 1)) = 0
        
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5

        [HideInInspector] _texcoord( "", 2D ) = "white" {}
        
	}
	
    
    SubShader 
    {
		Tags{ "RenderType" = "TransparentCutout" "Queue" = "Transparent" "IsEmissive" = "True"}
		Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

		CGPROGRAM
	    #pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		#pragma target 3.0
        
        #include "TransparentHelper.cginc"
		
        struct Input {
            float4 screenPos;
			float2 uv_texcoord;
            float3 worldPos;
		};
          
        sampler2D _TriggerMap;
        float4 _TriggerMap_ST;
  
        sampler2D _CameraDepthTexture;
        uniform float _Real;
	    uniform float4 _RestingColor, _ShimmerColor, _ActiveColor;  
        uniform float _Animated, _Elapsed, _AlphaCutoff; 
        uniform float _TransitionState, _isActive, _isCollide, _isLineRender, _FadeLength;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }
                
        void vertexDataFunc( inout appdata_full v, out Input o )
        {
            UNITY_INITIALIZE_OUTPUT( Input, o );
            float3 movement = goopVertexAnimation(v.vertex.xyz, v.normal.xyz, _Elapsed, _Animated);       
            v.vertex.xyz += movement; 
        }
        

		void surf (Input i, inout SurfaceOutput o) {
        
            float clipValue = goopAlpha(i.uv_texcoord, _TriggerMap, _TriggerMap_ST, _TransitionState, _Elapsed, _Real);
            clip(clipValue - _AlphaCutoff);
            
            float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
            float surfZ = -mul(UNITY_MATRIX_V, float4(i.worldPos.xyz, 1)).z;
            float diff = sceneZ - surfZ;
            float intersect = 1 - saturate(diff / _FadeLength);
            
            
            float shimmer = goopShimmer(i.uv_texcoord, _TriggerMap, _TriggerMap_ST, _TransitionState, _Elapsed, _Real);
            
            float activity = saturate((_isCollide * 0.3) + _isActive);
            
            float intensity = lerp(0.05, 1, activity);
        
            float3 dark = _RestingColor.rgb * 0.4;

            o.Emission.rgb = lerp(_RestingColor.rgb, dark, intersect);
            o.Emission = lerp(o.Emission.rgb, _ActiveColor, activity);
            
            shimmer = saturate(shimmer + _isLineRender);
            
            o.Emission += shimmer * _ShimmerColor * intensity;

            o.Alpha = _RestingColor.a;
            o.Alpha = lerp(_RestingColor.a, 1, 0.4 * intersect);
               
            
		}
		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
}
