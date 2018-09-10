// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/BothTexturedUpdate"
{
	Properties
	{
		_MultitexReal("MultitexReal", 2D) = "white" {}
		_MultitexLaser("MultitexLaser", 2D) = "white" {}
		_NormalReal("Normal Real", 2D) = "white" {}
		_NormalLaser("Normal Laser", 2D) = "white" {}
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_TransitionState("Transition State", Range( 0 , 1)) = 1
		_NormalScale("Normal Scale", Range( 0 , 10)) = 6.588235
		_MaterialColor("Material Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Off
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalScale;
		uniform float _TransitionState;
		uniform sampler2D _NormalReal;
		uniform float4 _NormalReal_ST;
		uniform sampler2D _NormalLaser;
		uniform float4 _NormalLaser_ST;
		uniform float4 _MaterialColor;
		uniform sampler2D _MultitexReal;
		uniform float4 _MultitexReal_ST;
		uniform sampler2D _MultitexLaser;
		uniform float4 _MultitexLaser_ST;
		uniform sampler2D _DissolveMap;
		uniform float4 _DissolveMap_ST;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float TansState168 = _TransitionState;
			float2 uv_NormalReal = i.uv_texcoord * _NormalReal_ST.xy + _NormalReal_ST.zw;
			float2 uv_NormalLaser = i.uv_texcoord * _NormalLaser_ST.xy + _NormalLaser_ST.zw;
			float3 norm172 = BlendNormals( UnpackScaleNormal( tex2D( _NormalReal, uv_NormalReal ), ( _NormalScale * ( 1.0 - TansState168 ) ) ) , UnpackScaleNormal( tex2D( _NormalLaser, uv_NormalLaser ), ( _NormalScale * TansState168 ) ) );
			o.Normal = norm172;
			float3 hsvTorgb180 = RGBToHSV( _MaterialColor.rgb );
			float2 uv_MultitexReal = i.uv_texcoord * _MultitexReal_ST.xy + _MultitexReal_ST.zw;
			float2 uv_MultitexLaser = i.uv_texcoord * _MultitexLaser_ST.xy + _MultitexLaser_ST.zw;
			float2 uv_DissolveMap = i.uv_texcoord * _DissolveMap_ST.xy + _DissolveMap_ST.zw;
			float Mask185 = saturate( ( (-1.0 + (TansState168 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + tex2D( _DissolveMap, uv_DissolveMap ).r ) );
			float4 lerpResult187 = lerp( tex2D( _MultitexReal, uv_MultitexReal ) , tex2D( _MultitexLaser, uv_MultitexLaser ) , Mask185);
			float4 Mult188 = lerpResult187;
			float4 break190 = Mult188;
			float3 hsvTorgb181 = HSVToRGB( float3(hsvTorgb180.x,( hsvTorgb180.y * break190.g ),break190.r) );
			float3 diffuse193 = hsvTorgb181;
			o.Albedo = diffuse193;
			float Smooth192 = break190.a;
			o.Smoothness = Smooth192;
			float Occo191 = break190.b;
			o.Occlusion = Occo191;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;61;1427;881;1862.362;286.9183;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-2721.885,248.5768;Float;False;Property;_TransitionState;Transition State;6;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;168;-2356.297,251.992;Float;False;TansState;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2354.249,336.1109;Float;True;Property;_DissolveMap;Dissolve Map;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;111;-2046.21,216.8187;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1814.877,226.1585;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;174;-1597.035,223.8237;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;158;-2367.216,-380.1691;Float;True;Property;_MultitexLaser;MultitexLaser;2;0;Create;True;0;0;False;0;None;02f835d35d4044115bc58edc6144a945;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;183;-2368.042,-590.145;Float;True;Property;_MultitexReal;MultitexReal;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;185;-1415.677,219.4398;Float;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;186;-2074.338,-378.0245;Float;False;185;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;187;-1914.592,-576.5715;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;169;-2737.552,824.5155;Float;False;168;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;179;-2278.871,-836.0488;Float;False;Property;_MaterialColor;Material Color;11;0;Create;True;0;0;False;0;0,0,0,0;0.4039215,0.3631999,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;188;-1747.968,-581.3399;Float;False;Mult;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;170;-2437.309,819.3169;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-2496.106,730.4395;Float;False;Property;_NormalScale;Normal Scale;10;0;Create;True;0;0;False;0;6.588235;0.6;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;180;-2011.015,-839.1292;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-2162.1,908.8987;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;190;-1536.326,-577.8985;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;177;-2162.1,787.6023;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-1974.653,748.4761;Float;True;Property;_NormalReal;Normal Real;3;0;Create;True;0;0;False;0;None;549187ca9dc0b42c5a75dcfe8abcf152;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;-1276.53,-724.4626;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;159;-1967.141,950.6927;Float;True;Property;_NormalLaser;Normal Laser;4;0;Create;True;0;0;False;0;e5895469fa6440e4a876bf0672599e59;b8f964f1ad90b4233a228c233972a4e6;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;171;-1658.492,872.9747;Float;True;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.HSVToRGBNode;181;-1002.828,-583.0939;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ToggleSwitchNode;165;-2651.459,-89.45348;Float;False;Property;_IsFlippable;IsFlippable;9;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;-1023.005,193.831;Float;False;193;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;-977.3623,338.0817;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-1016.192,286.9363;Float;False;191;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;166;-2650.45,14.57652;Float;False;Property;_ToggleSwitch0;IsSelected;8;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;-682.873,-500.2786;Float;False;diffuse;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-1243.241,-397.5817;Float;False;Smooth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;-1376.026,843.8535;Float;False;norm;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2722.391,154.4804;Float;False;Property;_EdgeThickness;Edge Thickness;7;0;Create;True;0;0;False;0;0.01;0.075;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-1243.241,-477.9533;Float;False;Occo;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-712.3373,206.2669;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Transition/BothTexturedUpdate;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;4;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;168;0;4;0
WireConnection;111;0;168;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;174;0;73;0
WireConnection;185;0;174;0
WireConnection;187;0;183;0
WireConnection;187;1;158;0
WireConnection;187;2;186;0
WireConnection;188;0;187;0
WireConnection;170;0;169;0
WireConnection;180;0;179;0
WireConnection;178;0;176;0
WireConnection;178;1;169;0
WireConnection;190;0;188;0
WireConnection;177;0;176;0
WireConnection;177;1;170;0
WireConnection;157;5;177;0
WireConnection;184;0;180;2
WireConnection;184;1;190;1
WireConnection;159;5;178;0
WireConnection;171;0;157;0
WireConnection;171;1;159;0
WireConnection;181;0;180;1
WireConnection;181;1;184;0
WireConnection;181;2;190;0
WireConnection;193;0;181;0
WireConnection;192;0;190;3
WireConnection;172;0;171;0
WireConnection;191;0;190;2
WireConnection;0;0;194;0
WireConnection;0;1;172;0
WireConnection;0;4;196;0
WireConnection;0;5;195;0
ASEEND*/
//CHKSM=A14D9FA4B19A5B159F03C78E238B38BC552BDA04