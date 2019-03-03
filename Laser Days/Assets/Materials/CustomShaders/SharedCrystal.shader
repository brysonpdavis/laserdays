// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/CrystalCore"
{
	Properties
	{
		[Header(Refraction)]
		_Color("Color", Color) = (0,0,0,0)
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		[HDR]_ShimmerColor("Shimmer Color", Color) = (8,8,8,0)
		_EffectMap("Effect Map", 2D) = "gray" {}
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_Refraction("Refraction", Range( 0 , 1)) = 0
		_TransitionState("Transition State", Range( 0 , 1)) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_OffsetMag("OffsetMag", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		ZTest LEqual
		GrabPass{ }
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Standard keepalpha finalcolor:RefractionF noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _TransitionState;
		uniform float _OffsetMag;
		uniform float4 _Color;
		uniform float4 _ShimmerColor;
		uniform float _Opacity;
		uniform sampler2D _EffectMap;
		uniform float4 _EffectMap_ST;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform float _Refraction;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float TRANS192 = _TransitionState;
			v.vertex.xyz += ( ( ase_vertexNormal * sin( ( TRANS192 * UNITY_PI ) ) * _OffsetMag ) + float3( 0,0,0 ) );
		}

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			color.rgb = color.rgb + Refraction( i, o, _Refraction, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			o.Albedo = _Color.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV288 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode288 = ( 0.0 + 1.3 * pow( 1.0 - fresnelNdotV288, 1.0 ) );
			o.Emission = ( fresnelNode288 * _ShimmerColor ).rgb;
			o.Alpha = _Opacity;
			float2 uv_EffectMap = i.uv_texcoord * _EffectMap_ST.xy + _EffectMap_ST.zw;
			float4 tex2DNode2 = tex2D( _EffectMap, uv_EffectMap );
			float DISSOLVE199 = tex2DNode2.r;
			float TRANS192 = _TransitionState;
			clip( step( 0.0 , ( DISSOLVE199 + -TRANS192 ) ) - _Cutoff );
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15500
0;169;1179;828;2379.282;130.058;1.799042;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-3039.057,-877.1799;Float;False;Property;_TransitionState;Transition State;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;192;-2734.309,-883.9003;Float;False;TRANS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-1663.509,1011.556;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;338;-1474.364,1086.094;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2610.575,57.191;Float;True;Property;_EffectMap;Effect Map;3;0;Create;True;0;0;False;0;None;c6c970f44e66b4f48ada591594eec846;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;339;-1258.48,882.8026;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;332;-1620.514,508.7204;Float;False;192;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-2305.516,27.5541;Float;False;DISSOLVE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;340;-1053.389,850.4199;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;322;-1631.539,638.4651;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;335;-1371.433,513.9378;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;331;-1449.606,290.4051;Float;True;199;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;330;-1714.044,857.4006;Float;False;Property;_OffsetMag;OffsetMag;12;0;Create;True;0;0;False;0;0;0.2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;333;-1175.936,346.9879;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;323;-1037.871,585.0972;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;288;-1952.6,-191.8394;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1.3;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;209;-1913.037,4.109119;Float;False;Property;_ShimmerColor;Shimmer Color;2;1;[HDR];Create;True;0;0;False;0;8,8,8,0;1.924429,1.924429,1.924429,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;300;-3074.791,-522.4196;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;265;-1806.319,279.1498;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;290;-2782.945,-727.8611;Float;True;Property;_colorful;colorful;6;0;Create;True;0;0;False;0;None;89258654212924302bc125dd9c07a179;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;295;-3312.845,-622.7229;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;319;-3024.614,-661.8746;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;334;-938.3779,334.4986;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;326;-857.5228,544.8295;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;294;-1004.522,171.7467;Float;False;Property;_Refraction;Refraction;7;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;273;-2171.52,377.9096;Float;False;Property;_Shimmer;Shimmer;11;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;259;-2869.866,300.5688;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.13;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;308;-2930.56,-502.0296;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;283;-990.2864,245.6078;Float;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;False;0;0.5;0.49;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;321;-1838.922,180.749;Float;False;320;0;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;311;-3289.593,-468.0421;Float;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;262;-2267.803,220.0709;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;258;-3129.511,261.0578;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;320;-2301.641,-779.7651;Float;True;PAL;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;314;-1395.986,-2.126298;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;337;-1531.934,276.5255;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;249;-935.019,-50.071;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;0,0,0,0;0.8679245,0.8679245,0.8679245,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-2246.354,-454.8446;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;260;-2607.368,259.5178;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;306;-2565.801,-399.9937;Float;False;Property;_ShineBlend;Shine Blend;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;317;-2561.696,-507.1378;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;287;-3168.407,466.8228;Float;False;1;0;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;309;-2594.062,465.3455;Float;False;Property;_Boost;Boost;5;0;Create;True;0;0;False;0;0;0.81;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;301;-2779.241,-511.3371;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-621.3309,110.8753;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/CrystalCore;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;2;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;10;-1;0;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;4;0
WireConnection;339;0;197;0
WireConnection;339;1;338;0
WireConnection;199;0;2;1
WireConnection;340;0;339;0
WireConnection;335;0;332;0
WireConnection;333;0;331;0
WireConnection;333;1;335;0
WireConnection;323;0;322;0
WireConnection;323;1;340;0
WireConnection;323;2;330;0
WireConnection;300;0;295;0
WireConnection;300;1;311;0
WireConnection;265;0;262;0
WireConnection;265;1;273;0
WireConnection;290;1;319;0
WireConnection;319;0;295;0
WireConnection;334;1;333;0
WireConnection;326;0;323;0
WireConnection;259;0;258;0
WireConnection;259;1;287;0
WireConnection;308;0;300;0
WireConnection;262;0;2;1
WireConnection;262;1;260;2
WireConnection;262;2;309;0
WireConnection;320;0;290;0
WireConnection;314;0;288;0
WireConnection;314;1;209;0
WireConnection;337;0;332;0
WireConnection;302;0;290;0
WireConnection;302;1;317;0
WireConnection;302;2;306;0
WireConnection;260;1;259;0
WireConnection;317;0;301;0
WireConnection;301;0;308;0
WireConnection;0;0;249;0
WireConnection;0;2;314;0
WireConnection;0;8;294;0
WireConnection;0;9;283;0
WireConnection;0;10;334;0
WireConnection;0;11;326;0
ASEEND*/
//CHKSM=3F551B355302CB86B47867132C02193F56FE41E4