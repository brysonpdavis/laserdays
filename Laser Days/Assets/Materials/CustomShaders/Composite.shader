
Shader "Custom/Composite"
{
	Properties
	{
		_MainTex ( "Screen", 2D ) = "black" {}
		//_OutlineTex("Outline Tex", 2D) = "white" {}
		_Color0("Color 0", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		ZTest Always
		Cull Off
		ZWrite Off

		
		Pass
		{ 
			CGPROGRAM 

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0   : TEXCOORD0;
                float2 uv1   : TEXCOORD1;
	
			};

            float2 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos ( v.vertex );
				o.uv0 = v.uv;
                o.uv1 = v.uv;

				#if UNITY_UV_STARTS_AT_TOP               
					if ( _MainTex_TexelSize.y < 0.0 )
						o.uv1.y = 1.0 - o.uv1.y;
				#endif
				return o;
			}

           

            sampler2D _MainTex;
            sampler2D _OutlineTex;
            sampler2D _TempTex;

            float4 _Line;

			half4 frag ( v2f i ) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv0);
                float4 col2 = tex2D(_OutlineTex, i.uv1);

                return col2;
                //return lerp(col, _Line, col2);
			} 
			ENDCG 
		}
	}
	
	
}
