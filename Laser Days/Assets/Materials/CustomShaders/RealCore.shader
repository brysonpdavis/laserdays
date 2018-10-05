// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/RealCore"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EffectMap("Effect Map", 2D) = "gray" {}
		[HDR]_ShimmerColor("Shimmer Color", Color) = (8,8,8,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_Health("Health", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _ShimmerColor;
		uniform float _Health;
		uniform float _TransitionState;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = ( _ShimmerColor * _Health ).rgb;
			o.Alpha = 1;
			float TRANS192 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float DISSOLVE199 = tex2D( _EffectMap, uv_EffectMap ).r;
			clip( step( 0.0 , ( -TRANS192 + DISSOLVE199 ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;64;1557;933;2193.734;298.1377;1.240015;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2411.724,231.9449;Float;False;Property;_TransitionState;Transition State;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2109.928,225.2245;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2409.552,341.5645;Float;True;Property;_EffectMap;Effect Map;1;0;Create;True;0;0;False;0;None;4213667ef65704f73b4663308f4e1b31;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;197;-1623.104,257.8127;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2114.869,350.865;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-1607.033,363.845;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;236;-1425.945,258.1794;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;209;-1594.988,-192.8836;Float;False;Property;_ShimmerColor;Shimmer Color;2;1;[HDR];Create;True;0;0;False;0;8,8,8,0;4,1.47451,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;237;-1654.454,-15.27313;Float;False;Property;_Health;Health;4;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1251.237,319.997;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-1137.467,127.3439;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;143;-1014.841,324.3226;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-675.3628,108.561;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/RealCore;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;4;0
WireConnection;199;0;2;1
WireConnection;236;0;197;0
WireConnection;73;0;236;0
WireConnection;73;1;203;0
WireConnection;238;0;209;0
WireConnection;238;1;237;0
WireConnection;143;1;73;0
WireConnection;0;2;238;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=E7AD2A095F1998FE79AAFD57B2C3B43AD169C503