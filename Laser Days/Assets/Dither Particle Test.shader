// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/MainParticleReal"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		_AlphaMap("Alpha Map", 2D) = "white" {}
		[HDR]_ParticleColor("Particle Color", Color) = (0,0,0,0)
		_TransitionState("TransitionState", Range( 0 , 1)) = 0
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
		uniform float _TransitionState;
		uniform float _Cutoff = 0.2;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _ParticleColor.rgb;
			o.Alpha = 1;
			float2 uv_AlphaMap = i.uv_texcoord * _AlphaMap_ST.xy + _AlphaMap_ST.zw;
			float4 tex2DNode25 = tex2D( _AlphaMap, uv_AlphaMap );
			clip( ( tex2DNode25.r * step( 0.0 , ( tex2DNode25.g + -_TransitionState ) ) ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
156;92;1179;828;870.4826;571.6023;1.626235;True;False
Node;AmplifyShaderEditor.RangedFloatNode;44;-407.399,415.5226;Float;False;Property;_TransitionState;TransitionState;3;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;50;-102.8997,386.2502;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;-304.4599,114.4597;Float;True;Property;_AlphaMap;Alpha Map;1;0;Create;True;0;0;False;0;None;da740d721002142d081609be82092efb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;49;54.84512,304.9386;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;51;360.5773,330.9581;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;335.0242,-32.07168;Float;False;Property;_ParticleColor;Particle Color;2;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.76,0.4771111,0.3968889,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;598.0079,176.4658;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;42;865.8796,39.98016;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/MainParticleReal;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.2;True;False;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;50;0;44;0
WireConnection;49;0;25;2
WireConnection;49;1;50;0
WireConnection;51;1;49;0
WireConnection;48;0;25;1
WireConnection;48;1;51;0
WireConnection;42;2;14;0
WireConnection;42;10;48;0
ASEEND*/
//CHKSM=2B16018139373584CEEDDA88B68A0F5D1DAD61AA