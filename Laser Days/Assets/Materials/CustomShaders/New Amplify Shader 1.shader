// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Patch"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_M("M", 2D) = "white" {}
		[PerRendererData]_TransitionState("Transition State", Range( 0 , 1)) = 0
		_LaserColor("Laser Color", Color) = (0,0,0,0)
		_RealColor("Real Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _RealColor;
		uniform float4 _LaserColor;
		uniform float _TransitionState;
		uniform sampler2D _M;
		uniform float4 _M_ST;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ase_vertexNormal * 0.0001 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 lerpResult9 = lerp( _RealColor , _LaserColor , _TransitionState);
			o.Albedo = lerpResult9.rgb;
			o.Alpha = 1;
			float2 uv_M = i.uv_texcoord * _M_ST.xy + _M_ST.zw;
			float4 tex2DNode1 = tex2D( _M, uv_M );
			float lerpResult5 = lerp( tex2DNode1.r , tex2DNode1.g , _TransitionState);
			clip( step( 0.2 , lerpResult5 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;169;1179;828;1569.45;747.9276;2.113786;True;False
Node;AmplifyShaderEditor.RangedFloatNode;2;-831.7389,1.262768;Float;False;Property;_TransitionState;Transition State;2;1;[PerRendererData];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-857.1042,-256.6191;Float;True;Property;_M;M;1;0;Create;True;0;0;False;0;None;d776cbeec058b429d9a01227a0ede61c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;10;-288.4956,421.9062;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-717.5943,428.2472;Float;False;Property;_LaserColor;Laser Color;3;0;Create;True;0;0;False;0;0,0,0,0;0.4251252,0.4203897,0.4433962,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-721.822,254.917;Float;False;Property;_RealColor;Real Color;4;0;Create;True;0;0;False;0;0,0,0,0;0.7315708,1,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;5;-489.3055,-237.5951;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-237.7647,605.8057;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;0.0001;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-396.299,147.1139;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;30.68601,303.5342;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;7;-189.148,-239.7089;Float;False;2;0;FLOAT;0.2;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;329.7506,-190.2407;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/Patch;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;1;1
WireConnection;5;1;1;2
WireConnection;5;2;2;0
WireConnection;9;0;6;0
WireConnection;9;1;8;0
WireConnection;9;2;2;0
WireConnection;12;0;10;0
WireConnection;12;1;11;0
WireConnection;7;1;5;0
WireConnection;0;0;9;0
WireConnection;0;10;7;0
WireConnection;0;11;12;0
ASEEND*/
//CHKSM=97FADD8165E440BF6F6556027D717FE9C97AFC6B