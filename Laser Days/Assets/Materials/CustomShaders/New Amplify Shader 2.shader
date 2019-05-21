// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/VerticalGradient"
{
	Properties
	{
		_S("S", Range( 0 , 1)) = 0
		_B("B", Color) = (0.6226415,0.04992881,0.04992881,0)
		_A("A", Color) = (0.9433962,0.6185475,0.6185475,0)
		_YScale("Y Scale", Range( 0 , 200)) = 0
		_off("off", Range( -3 , 3)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _A;
		uniform float4 _B;
		uniform float _YScale;
		uniform float _off;
		uniform float _S;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 worldToObj3 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			float4 lerpResult10 = lerp( _A , _B , saturate( (0.0 + (worldToObj3.y - ( -_YScale + _off )) * (1.0 - 0.0) / (( _YScale + _off ) - ( -_YScale + _off ))) ));
			o.Albedo = lerpResult10.rgb;
			o.Smoothness = _S;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
225;96;1162;807;2432.405;852.7135;1.677016;True;False
Node;AmplifyShaderEditor.RangedFloatNode;12;-1661.919,-233.4647;Float;False;Property;_YScale;Y Scale;3;0;Create;True;0;0;False;0;0;0.7;0;200;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-1926.286,-469.1004;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NegateNode;14;-1313.197,-358.1686;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;3;-1698.574,-438.7236;Float;False;World;Object;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;16;-1541.91,-151.7209;Float;False;Property;_off;off;4;0;Create;True;0;0;False;0;0;0;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-1104.209,-272.4661;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;5;-1052.473,-483.8763;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-1179.675,-369.7328;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;13;-774.7017,-460.3378;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;15;-539.7339,-294.665;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-416.377,42.65284;Float;False;Property;_B;B;1;0;Create;True;0;0;False;0;0.6226415,0.04992881,0.04992881,0;0.764151,0.6524047,0.5803222,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;7;-428.8256,-131.624;Float;False;Property;_A;A;2;0;Create;True;0;0;False;0;0.9433962,0.6185475,0.6185475,0;0.7058823,0.2770043,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-412.6747,-498.1108;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-408,213.5;Float;False;Property;_S;S;0;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;-184.207,-300.2158;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-247.187,-489.9903;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/VerticalGradient;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;12;0
WireConnection;3;0;2;0
WireConnection;18;0;12;0
WireConnection;18;1;16;0
WireConnection;5;0;3;0
WireConnection;17;0;14;0
WireConnection;17;1;16;0
WireConnection;13;0;5;1
WireConnection;13;1;17;0
WireConnection;13;2;18;0
WireConnection;15;0;13;0
WireConnection;10;0;7;0
WireConnection;10;1;8;0
WireConnection;10;2;15;0
WireConnection;9;0;6;0
WireConnection;0;0;10;0
WireConnection;0;4;1;0
ASEEND*/
//CHKSM=3A0D06BBB8B7BED8C6F4E5E1C22BF42887586028