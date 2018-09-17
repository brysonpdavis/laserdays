// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/Selected"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_HoldColor("Hold Color", Color) = (0,0,0,0)
		_HoverColor("Hover Color", Color) = (0,0,0,0)
		[HideInInspector]_onHold("onHold", Range( 0 , 1)) = 0
		[HideInInspector]_onHover("onHover", Range( 0 , 1)) = 0
		_SelectedEffectMap("Selected Effect Map", 2D) = "white" {}
		[HDR]_SelectedEffectColor("Selected Effect Color", Color) = (7.999999,7.999999,7.999999,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow nofog 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _HoverColor;
		uniform half _onHover;
		uniform float4 _HoldColor;
		uniform half _onHold;
		uniform sampler2D _DissolveMap;
		uniform float4 _SelectedEffectColor;
		uniform sampler2D _SelectedEffectMap;
		uniform float4 _SelectedEffectMap_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 lerpResult165 = lerp( tex2D( _Albedo, uv_Albedo ) , _HoverColor , ( _onHover * _HoverColor.a ));
			float4 lerpResult184 = lerp( lerpResult165 , _HoldColor , ( _HoldColor.a * _onHold ));
			float4 Albedo187 = lerpResult184;
			o.Albedo = Albedo187.rgb;
			float2 panner192 = ( 1.0 * _Time.y * float2( 0,0.2 ) + i.uv_texcoord);
			float2 uv_SelectedEffectMap = i.uv_texcoord * _SelectedEffectMap_ST.xy + _SelectedEffectMap_ST.zw;
			o.Emission = ( saturate( pow( tex2D( _DissolveMap, panner192 ) , 2.0 ) ) * _SelectedEffectColor * tex2D( _SelectedEffectMap, uv_SelectedEffectMap ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;44;1625;884;760.7747;-26.69434;1.109998;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;209;-1856.779,72.32484;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;159;-2142.219,-514.7857;Float;False;Property;_HoverColor;Hover Color;4;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;192;-1595.68,71.18036;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-2210.458,-341.4102;Half;False;Property;_onHover;onHover;7;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;181;-1414.929,-386.9798;Float;False;Property;_HoldColor;Hold Color;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-1679.608,-372.7224;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1402.057,43.18899;Float;True;Property;_DissolveMap;Dissolve Map;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-2260.015,-749.894;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;1c6c94e6f0b33b84794b532ebc14e8a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;183;-1507.209,-206.6797;Half;False;Property;_onHold;onHold;6;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;196;-1088.149,50.21935;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;2;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;165;-1415.963,-518.5706;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-1201.682,-267.2216;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;190;-944.2786,364.736;Float;False;Property;_SelectedEffectColor;Selected Effect Color;9;1;[HDR];Create;True;0;0;False;0;7.999999,7.999999,7.999999,0;21.36125,6.71212,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;202;-1404.191,267.3907;Float;True;Property;_SelectedEffectMap;Selected Effect Map;8;0;Create;True;0;0;False;0;None;881c304491028ea48b5027ac6c62cf73;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;206;-843.2041,53.2559;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;184;-912.5145,-482.8979;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;166.6858,74.52575;Float;False;187;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2895.633,163.3618;Float;False;Property;_TransitionState;Transition State;0;0;Create;True;0;0;False;0;0.7388377;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;-396.655,223.0929;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-589.491,-485.1602;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432.4345,117.531;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Transition/Selected;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.6;True;False;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;False;0;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;5;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;209;0
WireConnection;168;0;167;0
WireConnection;168;1;159;4
WireConnection;2;1;192;0
WireConnection;196;0;2;0
WireConnection;165;0;78;0
WireConnection;165;1;159;0
WireConnection;165;2;168;0
WireConnection;186;0;181;4
WireConnection;186;1;183;0
WireConnection;206;0;196;0
WireConnection;184;0;165;0
WireConnection;184;1;181;0
WireConnection;184;2;186;0
WireConnection;200;0;206;0
WireConnection;200;1;190;0
WireConnection;200;2;202;0
WireConnection;187;0;184;0
WireConnection;0;0;189;0
WireConnection;0;2;200;0
ASEEND*/
//CHKSM=66EB921C01CF6BB3285A106F3F72851E46669BEE