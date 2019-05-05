Shader "Crosshatch/CompletionCrystal" 
{	
    Properties {
		
        
        [HDR]_BaseColor("Base Color", Color) = (0,0,0,0)
        _EffectMap("Effect Map", 2D) = "white" {}
        _OffsetMag("Expand Size", Float) = 1
      
        _TransitionState("Transition State", Range( 0 , 1)) = 0

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
          
        float4 _BaseColor;
        float _OffsetMag, _AlphaCutoff;
        float _TransitionState;
        sampler2D _EffectMap;
        float4 _EffectMap_ST;
            
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }
        
        
        void vertexDataFunc( inout appdata_full v, out Input o )
        {
            UNITY_INITIALIZE_OUTPUT( Input, o );
            float3 vert = v.vertex.xyz;
            float3 norm = v.normal.xyz;
            float mag = crystalVertexAnimation(v.vertex.xyz, _TransitionState, _OffsetMag);
            v.vertex.xyz += mag * norm; 
        }
        
		void surf (Input i, inout SurfaceOutput o) {
            
            float clipValue = crystalAlpha(i.uv_texcoord, _EffectMap, _EffectMap_ST, _TransitionState);    
            clip(clipValue - _AlphaCutoff);
            
            o.Emission.rgb = _BaseColor.rgb;
            o.Alpha = _BaseColor.a;
             
		}
		ENDCG
	}
	Fallback "Legacy Shaders/VertexLit"
}
