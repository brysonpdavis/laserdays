// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/LaserOnly"
{
	Properties
	{
		_MaterialMap("Material Map", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_EffectMap("Effect Map", 2D) = "gray" {}
		_HoverColor("Hover Color", Color) = (0.5,0.5,0.5,0.2)
		_ColorLaser("Color Laser", Color) = (0.5,0.5,0.5,0.2)
		_ColorReal("Color Real", Color) = (0.5,0.5,0.5,0.2)
		_HoldColor("Hold Color", Color) = (0.25,0.25,0.25,0.4)
		[HDR]_EdgeColor("Edge Color", Color) = (0,0,0,0)
		_EdgeThickness("Edge Thickness", Range( 0 , 0.5)) = 0.3560508
		[HDR]_SecondaryEffectColor("Secondary Effect Color", Color) = (8,8,8,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		_TransitionStateB("Transition State B", Range( 0 , 1)) = 1
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_Clip("Clip", Float) = 1
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
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

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _MaterialMap;
		uniform float4 _MaterialMap_ST;
		uniform float4 _ColorReal;
		uniform float4 _ColorLaser;
		uniform float _TransitionStateB;
		uniform float4 _HoldColor;
		uniform half _onHold;
		uniform float _TransitionState;
		uniform float _EdgeThickness;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float4 _EdgeColor;
		uniform float4 _SecondaryEffectColor;
		uniform half _onHover;
		uniform float4 _HoverColor;
		uniform float _Clip;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORM182 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = NORM182;
			float2 uv_MaterialMap = i.uv_texcoord * _MaterialMap_ST.xy + _MaterialMap_ST.zw;
			float4 lerpResult171 = lerp( _ColorReal , _ColorLaser , _TransitionStateB);
			float4 lerpResult172 = lerp( ( tex2D( _MaterialMap, uv_MaterialMap ).r * lerpResult171 ) , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 DIFFUSE173 = lerpResult172;
			o.Albedo = DIFFUSE173.rgb;
			float TRANS175 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float DISSOLVE178 = tex2D( _EffectMap, uv_EffectMap ).r;
			float4 EDGE181 = ( ( step( ( ( TRANS175 * ( 1.0 + _EdgeThickness ) ) + -_EdgeThickness + DISSOLVE178 ) , 1.0 ) * _EdgeColor ) + ( _SecondaryEffectColor * float4( 0,0,0,0 ) ) );
			float4 HOVER209 = ( _onHover * _HoverColor * _HoverColor.a );
			o.Emission = ( EDGE181 + HOVER209 ).rgb;
			o.Alpha = 1;
			clip( step( 1.0 , ( TRANS175 + DISSOLVE178 ) ) - _Clip );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
82;45;1636;934;2405.808;884.5551;2.093382;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2235.168,-459.7347;Float;False;Property;_TransitionState;Transition State;10;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2410.029,576.6994;Float;False;Property;_EdgeThickness;Edge Thickness;8;0;Create;True;0;0;False;0;0.3560508;0.5;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;-2213.222,442.2285;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2254.27,-351.1039;Float;True;Property;_EffectMap;Effect Map;2;0;Create;True;0;0;False;0;None;4bb4b24093c3c41e4875dbafc18115e5;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;204;-2040.238,510.7202;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;175;-1926.118,-458.5519;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;198;-1847.921,544.8185;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;203;-1834.629,449.0787;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.28;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;178;-1928.723,-336.8405;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;180;-1905.841,632.6866;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;150;-1674.731,454.1983;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;207;-2234.643,-559.0808;Float;False;Property;_TransitionStateB;Transition State B;11;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;201;-1398.428,452.3585;Float;True;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;206;-2243.846,-741.2952;Float;False;Property;_ColorLaser;Color Laser;4;0;Create;True;0;0;False;0;0.5,0.5,0.5,0.2;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;152;-1415.469,679.9291;Float;False;Property;_EdgeColor;Edge Color;7;1;[HDR];Create;True;0;0;False;0;0,0,0,0;2,0,1.537255,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;187;-1145.886,715.3124;Float;False;Property;_SecondaryEffectColor;Secondary Effect Color;9;1;[HDR];Create;True;0;0;False;0;8,8,8,0;95.87451,47.18431,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;205;-2247.527,-908.7855;Float;False;Property;_ColorReal;Color Real;5;0;Create;True;0;0;False;0;0.5,0.5,0.5,0.2;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-2322.807,-1111.873;Float;True;Property;_MaterialMap;Material Map;0;0;Create;True;0;0;False;0;None;3628da1a659394f18ac44ddd08febdb6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;168;-1595.179,-827.9581;Float;False;Property;_HoldColor;Hold Color;6;0;Create;True;0;0;False;0;0.25,0.25,0.25,0.4;0,0,0,0.8039216;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-1151.258,479.9462;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;171;-1828.122,-814.1455;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;164;-1486.861,-318.0666;Float;False;Property;_HoverColor;Hover Color;3;0;Create;True;0;0;False;0;0.5,0.5,0.5,0.2;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;214;-875.5458,681.2943;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1590.101,-641.7913;Half;False;Property;_onHold;onHold;12;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-1518.037,-397.2334;Half;False;Property;_onHover;onHover;13;1;[HideInInspector];Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1278.861,-704.5185;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-689.2777,526.6241;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-1605.174,-930.872;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;-1177.715,80.20304;Float;False;175;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1046.455,-415.6061;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-1176.598,157.9982;Float;False;178;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;212;-714.3561,-82.78989;Float;False;209;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-907.6382,80.88966;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;211;-710.1693,-147.6847;Float;False;181;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;209;-839.959,-394.7037;Float;False;HOVER;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;157;-2266.315,-115.0031;Float;True;Property;_Normal;Normal;1;0;Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;172;-1092.765,-831.8486;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;-538.2872,520.3663;Float;False;EDGE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-865.45,-808.3433;Float;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;143;-659.3576,78.65103;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;182;-1951.478,-117.7006;Float;False;NORM;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;174;-468.0909,-285.1394;Float;False;173;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;213;-465.2437,-118.3773;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-2232.251,261.4507;Float;False;Constant;_Clip;Clip;13;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-464.8398,-203.6259;Float;False;182;0;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-212.0929,-140.4752;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/LaserOnly;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.27;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;2;0.7608291,0.9622642,0.1770203,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;192;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;204;1;156;0
WireConnection;175;0;4;0
WireConnection;198;0;156;0
WireConnection;203;0;176;0
WireConnection;203;1;204;0
WireConnection;178;0;2;1
WireConnection;150;0;203;0
WireConnection;150;1;198;0
WireConnection;150;2;180;0
WireConnection;201;0;150;0
WireConnection;151;0;201;0
WireConnection;151;1;152;0
WireConnection;171;0;205;0
WireConnection;171;1;206;0
WireConnection;171;2;207;0
WireConnection;214;0;187;0
WireConnection;170;0;168;4
WireConnection;170;1;167;0
WireConnection;190;0;151;0
WireConnection;190;1;214;0
WireConnection;208;0;78;1
WireConnection;208;1;171;0
WireConnection;169;0;165;0
WireConnection;169;1;164;0
WireConnection;169;2;164;4
WireConnection;73;0;177;0
WireConnection;73;1;179;0
WireConnection;209;0;169;0
WireConnection;172;0;208;0
WireConnection;172;1;168;0
WireConnection;172;2;170;0
WireConnection;181;0;190;0
WireConnection;173;0;172;0
WireConnection;143;1;73;0
WireConnection;182;0;157;0
WireConnection;213;0;211;0
WireConnection;213;1;212;0
WireConnection;0;0;174;0
WireConnection;0;1;183;0
WireConnection;0;2;213;0
WireConnection;0;10;143;0
ASEEND*/
//CHKSM=B550B3452FA61C7571E86E1BE41338E286C6EFC9