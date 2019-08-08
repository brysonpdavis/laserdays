Shader "Crosshatch/FlatWater" {
	
    Properties {
		
         _TriggerMap("Goop Map", 2D) = "white" {}
        
        _RealColor("Real Color", Color) = (0,0,0,0)
        _LaserColor("Laser Color", Color) = (0,0,0,0)
        
        _RealDepthColor("Real Depth Color", Color) = (0,0,0,0)
        _LaserDepthColor("Laser Depth Color", Color) = (0,0,0,0)
        
        
        _FadeLength("_FadeLength", Range(0,3)) = 0.1
                   
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
	    #pragma surface surf Unlit alpha:fade keepalpha noshadow
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
	    uniform float4 _RealColor, _RealDepthColor, _LaserColor, _LaserDepthColor;  
        uniform float  _AlphaCutoff; 
        uniform float _TransitionState, _isActive, _isCollide, _isLineRender, _FadeLength;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }
                
        
		void surf (Input i, inout SurfaceOutput o) {
        
            
            float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
            float surfZ = -mul(UNITY_MATRIX_V, float4(i.worldPos.xyz, 1)).z;
            float diff = sceneZ - surfZ;
            float intersect = 1 - saturate(diff / _FadeLength);
            
            float4 baseCol = lerp(_RealColor, _LaserColor, _TransitionState);
            float4 depthCol = lerp(_RealDepthColor, _LaserDepthColor, _TransitionState);
            
            o.Emission.rgb = lerp(baseCol, depthCol, intersect);

            o.Alpha = lerp(baseCol.a, depthCol.a, intersect);
               
            
		}
		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
}
