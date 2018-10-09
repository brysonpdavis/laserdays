// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/LaserGate"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EffectMap("Effect Map", 2D) = "gray" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_Open("Open", Range( 0 , 1)) = 0
		_TransitionStateB("Transition State B", Range( 0 , 1)) = 0
		_ColorLaser("Color Laser", Color) = (0,0,0,0)
		_ColorReal("Color Real", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _ColorReal;
		uniform float4 _ColorLaser;
		uniform float _TransitionStateB;
		uniform float _TransitionState;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float _Open;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 lerpResult253 = lerp( _ColorReal , _ColorLaser , _TransitionStateB);
			o.Albedo = ( lerpResult253 * float4( 1,1,1,0 ) ).rgb;
			o.Alpha = 1;
			float TRANS192 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode2 = tex2D( _EffectMap, uv_EffectMap );
			float DISSOLVE199 = tex2DNode2.r;
			float OPEN285 = tex2DNode2.a;
			clip( ( ( 1.0 - step( 0.0 , ( -TRANS192 + DISSOLVE199 ) ) ) * step( 0.0 , ( -_Open + OPEN285 ) ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
146;98;1273;781;3190.414;219.5115;1.856658;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2401.581,163.34;Float;False;Property;_TransitionState;Transition State;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-2125.255,569.5934;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-1987.956,122.6325;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2404.037,352.1225;Float;True;Property;_EffectMap;Effect Map;1;0;Create;True;0;0;False;0;None;48a641c42967c418ea65286466ecb5f4;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;236;-1942.948,566.6599;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2109.354,361.423;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;284;-2225.304,841.508;Float;False;Property;_Open;Open;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-2124.035,672.3254;Float;False;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1768.24,628.4775;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;288;-1868.269,855.4522;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;285;-2104.823,449.0067;Float;False;OPEN;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;289;-2049.357,961.1179;Float;False;285;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;143;-1531.844,632.803;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;250;-2402.608,-96.96741;Float;False;Property;_ColorLaser;Color Laser;5;0;Create;True;0;0;False;0;0,0,0,0;0.5,0.5,0.5,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;249;-2402.444,-264.0293;Float;False;Property;_ColorReal;Color Real;6;0;Create;True;0;0;False;0;0,0,0,0;0.6509434,0.5949731,0.365388,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;252;-2403.383,73.18729;Float;False;Property;_TransitionStateB;Transition State B;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;290;-1693.561,917.2698;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;294;-1291.053,686.5375;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;253;-2022.274,-224.7093;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;291;-1457.165,921.5954;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;292;-1110.913,737.7472;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;-1191.057,-128.2215;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-795.8089,-104.9187;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/LaserGate;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;4;0
WireConnection;236;0;197;0
WireConnection;199;0;2;1
WireConnection;73;0;236;0
WireConnection;73;1;203;0
WireConnection;288;0;284;0
WireConnection;285;0;2;4
WireConnection;143;1;73;0
WireConnection;290;0;288;0
WireConnection;290;1;289;0
WireConnection;294;0;143;0
WireConnection;253;0;249;0
WireConnection;253;1;250;0
WireConnection;253;2;252;0
WireConnection;291;1;290;0
WireConnection;292;0;294;0
WireConnection;292;1;291;0
WireConnection;293;0;253;0
WireConnection;0;0;293;0
WireConnection;0;10;292;0
ASEEND*/
//CHKSM=CD8FCEFD43199B327862BEB97079340AD6FF0378