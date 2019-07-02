// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Crosshatch/CircleParticle"
{
	Properties
	{
		[Toggle]_Real("Real", Float) = 1
		[HDR]_Color("Color", Color) = (0,0,0,0)
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform float _Real;
		uniform float _TransitionState;
		uniform float _Cutoff = 0.2;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _Color.rgb;
			o.Alpha = 1;
			float2 uv_TexCoord2 = i.uv_texcoord + float2( 0.5,0.5 );
			clip( step( 0.55 , ( abs( ( _Real - _TransitionState ) ) * ( 1.0 - saturate( distance( uv_TexCoord2 , float2( 1,1 ) ) ) ) ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
485;44;1162;807;1877.182;638.0641;1.635537;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1342.385,-306.4514;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0.5,0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-1155.91,-32.91545;Float;False;Property;_Real;Real;0;1;[Toggle];Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;1;-1118.77,-307.7225;Float;True;2;0;FLOAT2;0,0;False;1;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1276.94,66.85235;Float;False;Property;_TransitionState;Transition State;2;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;4;-903.7249,-310.5927;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;-956.3754,12.87963;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;11;-743.7556,52.13249;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;5;-817.5862,-226.5449;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-621.0903,-104.879;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-531.1355,-351.8451;Float;False;Property;_Color;Color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;6;-450.1512,-164.3367;Float;True;2;0;FLOAT;0.55;False;1;FLOAT;0.66;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-178.2735,-371.2669;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Crosshatch/CircleParticle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.2;True;True;0;True;Opaque;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;0;2;0
WireConnection;4;0;1;0
WireConnection;10;0;9;0
WireConnection;10;1;8;0
WireConnection;11;0;10;0
WireConnection;5;0;4;0
WireConnection;12;0;11;0
WireConnection;12;1;5;0
WireConnection;6;1;12;0
WireConnection;0;2;7;0
WireConnection;0;10;6;0
ASEEND*/
//CHKSM=CAD30D52C08A1BF23D33729ED8FA8044DF85A07E