// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/BothUnlit"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_EffectMap("Effect Map", 2D) = "gray" {}
		_TransitionStateB("Transition State B", Range( 0 , 1)) = 1
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		[HDR]_ColorLaser("Color Laser", Color) = (0,0,0,0)
		[HDR]_ColorReal("Color Real", Color) = (0,0,0,0)
		[Toggle]_SteppedTransition("Stepped Transition", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half4 _ColorReal;
		uniform half4 _ColorLaser;
		uniform float _TransitionStateB;
		uniform float _SteppedTransition;
		uniform float _TransitionState;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 lerpResult203 = lerp( _ColorReal , _ColorLaser , _TransitionStateB);
			half4 DIFFUSE204 = lerpResult203;
			o.Emission = DIFFUSE204.rgb;
			o.Alpha = 1;
			float TRANS168 = _TransitionState;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode2 = tex2D( _EffectMap, uv_EffectMap );
			float MASK185 = lerp(saturate( ( (-1.0 + (TRANS168 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + tex2DNode2.r ) ),step( 1.0 , ( TRANS168 + tex2DNode2.r ) ),_SteppedTransition);
			clip( MASK185 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
124;45;1556;945;3292.571;1168.32;1.394226;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2828.331,-332.6716;Float;False;Property;_TransitionState;Transition State;3;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2565.849,-41.89796;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2664.82,50.21706;Float;True;Property;_EffectMap;Effect Map;1;0;Create;True;0;0;False;0;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2354.403,-240.992;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-2173.975,-18.3296;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;212;-2167.959,-254.5089;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;210;-1953.762,-22.26138;Float;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-2780.891,-545.1008;Float;False;Property;_TransitionStateB;Transition State B;2;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;213;-1926.34,-221.9734;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;179;-2488.784,-835.8434;Half;False;Property;_ColorReal;Color Real;6;1;[HDR];Create;True;0;0;False;0;0,0,0,0;2.996078,2.996078,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;197;-2482.902,-654.9715;Half;False;Property;_ColorLaser;Color Laser;5;1;[HDR];Create;True;0;0;False;0;0,0,0,0;2.996078,0,2.996078,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;211;-1645.552,-72.12125;Float;False;Property;_SteppedTransition;Stepped Transition;7;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;203;-1743.115,-739.4244;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;185;-1326.758,3.78045;Float;False;MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;215;-1788.202,-598.0812;Float;False;myVarName3;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2931.943,-140.6217;Float;False;Property;_EdgeThickness;Edge Thickness;4;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;214;-2651.446,289.3625;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;-950.912,177.9324;Float;False;204;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;204;-1527.72,-721.6079;Half;False;DIFFUSE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-710.8378,206.2669;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Transition/BothUnlit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;168;0;4;0
WireConnection;111;0;168;0
WireConnection;73;0;168;0
WireConnection;73;1;2;1
WireConnection;212;0;111;0
WireConnection;212;1;2;1
WireConnection;210;1;73;0
WireConnection;213;0;212;0
WireConnection;211;0;213;0
WireConnection;211;1;210;0
WireConnection;203;0;179;0
WireConnection;203;1;197;0
WireConnection;203;2;216;0
WireConnection;185;0;211;0
WireConnection;204;0;203;0
WireConnection;0;2;194;0
WireConnection;0;10;185;0
ASEEND*/
//CHKSM=D1D1FCC714AF411D313C98BD85D625DF792DD029