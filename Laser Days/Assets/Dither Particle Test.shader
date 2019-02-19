// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/DitherParticle"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		_AlphaMap("Alpha Map", 2D) = "white" {}
		[HDR]_ParticleColor("Particle Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		AlphaToMask On
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _ParticleColor;
		uniform sampler2D _AlphaMap;
		uniform float4 _AlphaMap_ST;
		uniform float _Cutoff = 0.2;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_14_0 = _ParticleColor;
			o.Emission = temp_output_14_0.rgb;
			o.Alpha = 1;
			float2 uv_AlphaMap = i.uv_texcoord * _AlphaMap_ST.xy + _AlphaMap_ST.zw;
			float4 tex2DNode25 = tex2D( _AlphaMap, uv_AlphaMap );
			clip( tex2DNode25.a - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
64;71;1179;828;794.4431;571.6023;1.626235;True;False
Node;AmplifyShaderEditor.ColorNode;26;-221.0673,112.1549;Float;False;Property;_LineColor;Line Color;2;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-156.0988,-269.502;Float;False;Property;_ParticleColor;Particle Color;3;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.76,0.4771111,0.3968889,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;43;-143.9488,-513.0579;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;-346.7419,296.598;Float;True;Property;_AlphaMap;Alpha Map;1;0;Create;True;0;0;False;0;None;a2c928ca393434b84a1a34a0847cae77;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;39;615.0067,174.6783;Float;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;132.9986,-75.98585;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;145.1388,95.0165;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;322.7202,33.19625;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;42;865.8796,39.98016;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/DitherParticle;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.2;True;False;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;14;0
WireConnection;27;1;25;2
WireConnection;28;0;26;0
WireConnection;28;1;25;1
WireConnection;29;0;27;0
WireConnection;29;1;28;0
WireConnection;42;2;14;0
WireConnection;42;10;25;4
ASEEND*/
//CHKSM=159775E162D7FC161B7109A2BA2AEF6D217CD7CE