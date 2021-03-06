// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Transition/TriggerReal"
{
	Properties
	{
		_EffectMap("Effect Map", 2D) = "white" {}
		[Toggle]_isActive1("isActive1", Float) = 0
		[Toggle]_isActive0("isActive0", Float) = 0
		[HDR]_PassiveColor("PassiveColor", Color) = (0,0,0,0)
		[HDR]_ActiveColor("ActiveColor", Color) = (0,0,0,0)
		[Toggle]_isCollide("isCollide", Float) = 0
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _isActive1;
		uniform float4 _PassiveColor;
		uniform float4 _ActiveColor;
		uniform float _isActive0;
		uniform float _isCollide;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform float _TransitionState;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = lerp(_PassiveColor,_ActiveColor,_isActive1).rgb;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode1 = tex2D( _EffectMap, uv_EffectMap );
			o.Alpha = ( lerp(lerp(0.0,tex2DNode1.g,_isCollide),tex2DNode1.b,_isActive0) * ( 1.0 - _TransitionState ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
146;98;1001;768;965.171;471.4501;1.40747;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-1076.808,-33.0923;Float;True;Property;_EffectMap;Effect Map;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-719.6238,146.5167;Float;False;Property;_TransitionState;Transition State;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;3;-721.4379,-208.7452;Float;False;Property;_isCollide;isCollide;6;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-1096.302,-520.8001;Float;False;Property;_ActiveColor;ActiveColor;5;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.02352942,0.1403964,0.4705882,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;10;-383.4196,116.1807;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;4;-444.1858,-149.4133;Float;True;Property;_isActive0;isActive0;3;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-1096.834,-685.136;Float;False;Property;_PassiveColor;PassiveColor;4;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.8679245,0.6662333,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;16;-1049.202,-198.1473;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-135.4438,-19.09958;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;13;-1051.202,-325.1473;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;7;-746.3409,-618.7535;Float;False;Property;_isActive1;isActive1;2;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1321.202,-368.1473;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;15;-1293.202,-185.1473;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;134.5362,-288.3973;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Transition/TriggerReal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;1;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;False;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;1;1;2
WireConnection;10;0;8;0
WireConnection;4;0;3;0
WireConnection;4;1;1;3
WireConnection;16;0;14;0
WireConnection;16;1;15;0
WireConnection;17;0;4;0
WireConnection;17;1;10;0
WireConnection;13;0;14;0
WireConnection;13;1;15;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;2;2;7;0
WireConnection;2;9;17;0
ASEEND*/
//CHKSM=BC3D83F2556BC91F980E95DEB71B7DBFD61AB8AB