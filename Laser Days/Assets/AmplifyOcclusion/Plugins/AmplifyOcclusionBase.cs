// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu( "" )]
public class AmplifyOcclusionBase : MonoBehaviour
{
	public enum ApplicationMethod
	{
		PostEffect = 0,
		Deferred,
		Debug
	}

	public enum PerPixelNormalSource
	{
		None = 0,
		Camera,
		GBuffer,
		GBufferOctaEncoded,
	}

	public enum SampleCountLevel
	{
		Low = 0,
		Medium,
		High,
		VeryHigh
	}

	[Header( "Ambient Occlusion" )]
	[Tooltip( "How to inject the occlusion: Post Effect = Overlay, Deferred = Deferred Injection, Debug - Vizualize." )]
	public ApplicationMethod ApplyMethod = ApplicationMethod.PostEffect;

	[Tooltip( "Number of samples per pass." )]
	public SampleCountLevel SampleCount = SampleCountLevel.High;

	[Tooltip( "Source of per-pixel normals: None = All, Camera = Forward, GBuffer = Deferred." )]
	public PerPixelNormalSource PerPixelNormals = PerPixelNormalSource.Camera;

	[Tooltip( "Final applied intensity of the occlusion effect." )]
	[Range( 0, 1 )]
	public float Intensity = 1.0f;

	[Tooltip( "Color tint for occlusion." )]
	public Color Tint = Color.black;

	[Tooltip( "Radius spread of the occlusion." )]
	[Range( 0, 32 )]
	public float Radius = 2.0f;

	[Tooltip( "Power exponent attenuation of the occlusion." )]
	[Range( 0, 16 )]
	public float PowerExponent = 1.8f;

	[Tooltip( "Controls the initial occlusion contribution offset." )]
	[Range( 0, 0.99f )]
	public float Bias = 0.05f;

	[Tooltip( "Controls the thickness occlusion contribution." )]
	[Range( 0, 1.0f )]
	public float Thickness = 1.0f;

	[Tooltip( "Compute the Occlusion and Blur at half of the resolution." )]
	public bool Downsample = true;

	[Header( "Distance Fade" )]
	[Tooltip( "Control parameters at faraway." )]
	public bool FadeEnabled = false;

	[Tooltip( "Distance in Unity unities that start to fade." )]
	public float FadeStart = 100.0f;

	[Tooltip( "Length distance to performe the transition." )]
	public float FadeLength = 50.0f;

	[Tooltip( "Final Intensity parameter." )]
	[Range( 0, 1 )]
	public float FadeToIntensity = 0.0f;
	public Color FadeToTint = Color.black;

	[Tooltip( "Final Radius parameter." )]
	[Range( 0, 32 )]
	public float FadeToRadius = 2.0f;

	[Tooltip( "Final PowerExponent parameter." )]
	[Range( 0, 16 )]
	public float FadeToPowerExponent = 1.0f;

	[Tooltip( "Final Thickness parameter." )]
	[Range( 0, 1.0f )]
	public float FadeToThickness = 1.0f;

	[Header( "Bilateral Blur" )]
	public bool BlurEnabled = true;

	[Tooltip( "Radius in screen pixels." )]
	[Range( 1, 4 )]
	public int BlurRadius = 3;

	[Tooltip( "Number of times that the Blur will repeat." )]
	[Range( 1, 4 )]
	public int BlurPasses = 2;

	[Tooltip( "Sharpness of blur edge-detection: 0 = Softer Edges, 20 = Sharper Edges." )]
	[Range( 0, 20 )]
	public float BlurSharpness = 15.0f;

	[Header( "Temporal Filter" )]
	[Tooltip( "Accumulates the effect over the time." )]
	public bool FilterEnabled = true;

	[Tooltip( "Controls the accumulation decayment: 0 = More flicker with less ghosting, 1 = Less flicker with more ghosting." )]
	[Range( 0, 1 )]
	public float FilterBlending = 0.75f;

	[Tooltip( "Controls the discard sensitivity based on the motion of the scene and objects." )]
	[Range( 0, 1 )]
	public float FilterResponse = 0.5f;

	[Tooltip( "Reduces ghosting effect near the objects's edges while moving." )]
	private bool TemporalDilation = false;

	// Current state variables
	private bool m_HDR = true;
	private bool m_MSAA = true;

	// Previous state variables
	private PerPixelNormalSource m_prevPerPixelNormals;
	private ApplicationMethod m_prevApplyMethod;
	private bool m_prevDeferredReflections = false;
	private SampleCountLevel m_prevSampleCount = SampleCountLevel.Low;
	private bool m_prevDownsample = false;
	private bool m_prevBlurEnabled = false;
	private int m_prevBlurRadius = 0;
	private int m_prevBlurPasses = 0;
	private bool m_prevFilterEnabled = true;
	private bool m_prevHDR = true;
	private bool m_prevMSAA = true;

	private Camera m_targetCamera = null;

	private RenderTargetIdentifier[] applyDebugTargetsTemporal = new RenderTargetIdentifier[2];
	private RenderTargetIdentifier[] applyDeferredTargets_Log_Temporal = new RenderTargetIdentifier[3];
	private RenderTargetIdentifier[] applyDeferredTargetsTemporal = new RenderTargetIdentifier[3];
	private RenderTargetIdentifier[] applyOcclusionTemporal = new RenderTargetIdentifier[2];
	private RenderTargetIdentifier[] applyPostEffectTargetsTemporal = new RenderTargetIdentifier[2];

	// NOTE: MotionVectors are not supported in Deferred Injection mode due to 1 frame delay
	private bool UsingTemporalFilter { get { return FilterEnabled == true && m_targetCamera.cameraType != CameraType.SceneView; } }
	private bool UsingMotionVectors { get { return UsingTemporalFilter && ( ApplyMethod != ApplicationMethod.Deferred ); } }

	private bool useMRTBlendingFallback = false;

	// Command buffer
	private struct CmdBuffer
	{
		public CommandBuffer cmdBuffer;
		public CameraEvent cmdBufferEvent;
		public string cmdBufferName;
	}

	CmdBuffer m_commandBuffer_Occlusion;
	CmdBuffer m_commandBuffer_Apply;

	private void createCommandBuffer( ref CmdBuffer aCmdBuffer, string aCmdBufferName, CameraEvent aCameraEvent )
	{
		if( aCmdBuffer.cmdBuffer != null )
		{
			cleanupCommandBuffer( ref aCmdBuffer );
		}

		aCmdBuffer.cmdBufferName = aCmdBufferName;

		aCmdBuffer.cmdBuffer = new CommandBuffer();
		aCmdBuffer.cmdBuffer.name = aCmdBufferName;

		aCmdBuffer.cmdBufferEvent = aCameraEvent;

		m_targetCamera.AddCommandBuffer( aCameraEvent, aCmdBuffer.cmdBuffer );
	}

	private void cleanupCommandBuffer( ref CmdBuffer aCmdBuffer )
	{
		CommandBuffer[] currentCBs = m_targetCamera.GetCommandBuffers( aCmdBuffer.cmdBufferEvent );

		for( int i = 0; i < currentCBs.Length; i++ )
		{
			if( currentCBs[ i ].name == aCmdBuffer.cmdBufferName )
			{
				m_targetCamera.RemoveCommandBuffer( aCmdBuffer.cmdBufferEvent, currentCBs[ i ] );
			}
		}

		aCmdBuffer.cmdBufferName = null;
		aCmdBuffer.cmdBufferEvent = 0;
		aCmdBuffer.cmdBuffer = null;
	}

	// Quad Mesh
	static private Mesh m_quadMesh = null;

	private void createQuadMesh()
	{
		if( m_quadMesh == null )
		{
			m_quadMesh = new Mesh();
			m_quadMesh.vertices = new Vector3[ 4 ] { new Vector3( 0, 0, 0 ), new Vector3( 0, 1, 0 ), new Vector3( 1, 1, 0 ), new Vector3( 1, 0, 0 ) };
			m_quadMesh.uv = new Vector2[ 4 ] { new Vector2( 0, 0 ), new Vector2( 0, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 0 ) };
			m_quadMesh.triangles = new int[ 6 ] { 0, 1, 2, 0, 2, 3 };

			m_quadMesh.normals = new Vector3[ 0 ];
			m_quadMesh.tangents = new Vector4[ 0 ];
			m_quadMesh.colors32 = new Color32[ 0 ];
			m_quadMesh.colors = new Color[ 0 ];
		}
	}


	void PerformBlit( CommandBuffer cb, Material mat, int pass )
	{
		cb.DrawMesh( m_quadMesh, Matrix4x4.identity, mat, 0, pass );
	}

	// Render Materials
	private Material createMaterialWithShaderName( string aShaderName, bool aThroughErrorMsg )
	{
		var shader = Shader.Find( aShaderName );

		if( shader == null )
		{
			if( aThroughErrorMsg == true )
			{
				Debug.LogErrorFormat( "[AmplifyOcclusion] Cannot find shader: \"{0}\"" +
										" Please contact support@amplify.pt", aShaderName );
			}

			return null;
		}

		return new Material( shader ) { hideFlags = HideFlags.DontSave };
	}

	static private Material m_occlusionMat = null;
	static private Material m_blurMat = null;
	static private Material m_applyOcclusionMat = null;

	private void checkMaterials( bool aThroughErrorMsg )
	{
		if( m_occlusionMat == null )
		{
			m_occlusionMat = createMaterialWithShaderName( "Hidden/Amplify Occlusion/Occlusion", aThroughErrorMsg );
		}

		if( m_blurMat == null )
		{
			m_blurMat = createMaterialWithShaderName( "Hidden/Amplify Occlusion/Blur", aThroughErrorMsg );
		}

		if( m_applyOcclusionMat == null )
		{
			m_applyOcclusionMat = createMaterialWithShaderName( "Hidden/Amplify Occlusion/Apply", aThroughErrorMsg );
		}
	}

	private RenderTextureFormat m_occlusionRTFormat = RenderTextureFormat.RGHalf;
	private RenderTextureFormat m_accumTemporalRTFormat = RenderTextureFormat.ARGB32;
	private RenderTextureFormat m_temporaryEmissionRTFormat = RenderTextureFormat.ARGB32;

	private bool checkRenderTextureFormats()
	{
		// test the two fallback formats first
		if( SystemInfo.SupportsRenderTextureFormat( RenderTextureFormat.ARGB32 ) && SystemInfo.SupportsRenderTextureFormat( RenderTextureFormat.ARGBHalf ) )
		{
			m_occlusionRTFormat = RenderTextureFormat.RGHalf;
			if( !SystemInfo.SupportsRenderTextureFormat( m_occlusionRTFormat ) )
			{
				m_occlusionRTFormat = RenderTextureFormat.RGFloat;
				if( !SystemInfo.SupportsRenderTextureFormat( m_occlusionRTFormat ) )
				{
					// already tested above
					m_occlusionRTFormat = RenderTextureFormat.ARGBHalf;
				}
			}
			m_accumTemporalRTFormat = RenderTextureFormat.ARGB32;
			return true;
		}
		return false;
	}


	void OnEnable()
	{
		if( !checkRenderTextureFormats() )
		{
			Debug.LogError( "[AmplifyOcclusion] Target platform does not meet the minimum requirements for this effect to work properly." );

			this.enabled = false;

			return;
		}

		m_targetCamera = GetComponent<Camera>();

		checkMaterials( false );
		createQuadMesh();

		// some platforms still don't support MRT-blending; provide a fallback, if necessary
		useMRTBlendingFallback = m_applyOcclusionMat.GetTag( "MRTBlending", false ).ToUpper() != "TRUE";
	}


	void Reset()
	{
		if( m_commandBuffer_Occlusion.cmdBuffer != null )
		{
			cleanupCommandBuffer( ref m_commandBuffer_Occlusion );
		}

		if( m_commandBuffer_Apply.cmdBuffer != null )
		{
			cleanupCommandBuffer( ref m_commandBuffer_Apply );
		}

		releaseRT();
	}

	void OnDisable()
	{
		Reset();
	}

	private void releaseRT()
	{
		safeReleaseRT( ref m_occlusionDepthRT );

		m_occlusionDepthRT = null;

		if( m_temporalAccumRT != null )
		{
			if( m_temporalAccumRT.Length != 0 )
			{
				safeReleaseRT( ref m_temporalAccumRT[ 0 ] );
				safeReleaseRT( ref m_temporalAccumRT[ 1 ] );
			}
		}

		m_temporalAccumRT = null;
	}

	private bool m_paramsChanged = true;

	private void ClearHistory()
	{
		if ( m_temporalAccumRT != null )
		{
			Graphics.SetRenderTarget( m_temporalAccumRT[ 0 ] );
			GL.Clear( false, true, Color.black );

			Graphics.SetRenderTarget( m_temporalAccumRT[ 1 ] );
			GL.Clear( false, true, Color.black );
		}
	}

	private bool checkParamsChanged()
	{
		bool HDR = m_targetCamera.allowHDR; // && tier?
		bool MSAA = m_targetCamera.allowMSAA &&
					m_targetCamera.actualRenderingPath != RenderingPath.DeferredLighting &&
					m_targetCamera.actualRenderingPath != RenderingPath.DeferredShading &&
					QualitySettings.antiAliasing >= 1;

		int antiAliasing = MSAA ? QualitySettings.antiAliasing : 1;

		if( m_occlusionDepthRT != null )
		{
			if( ( m_occlusionDepthRT.width != m_target.width ) ||
				( m_occlusionDepthRT.height != m_target.height ) ||
				( m_prevMSAA != MSAA ) ||
				( m_prevFilterEnabled != FilterEnabled ) ||
				( !m_occlusionDepthRT.IsCreated() ) ||
				( m_temporalAccumRT != null && ( !m_temporalAccumRT[ 0 ].IsCreated() || !m_temporalAccumRT[ 1 ].IsCreated() ) ) )
			{
				releaseRT();

				m_paramsChanged = true;
			}
		}

		if( m_temporalAccumRT != null )
		{
			if( m_temporalAccumRT.Length != 2 )
			{
				m_temporalAccumRT = null;
			}
		}

		if( m_occlusionDepthRT == null )
		{
			m_occlusionDepthRT = safeAllocateRT( "_AO_OcclusionDepthTexture",
												m_target.width,
												m_target.height,
												m_occlusionRTFormat,
												RenderTextureReadWrite.Linear,
												FilterMode.Point );
		}

		bool clearHistory = false;
		if( m_temporalAccumRT == null && FilterEnabled )
		{
			m_temporalAccumRT = new RenderTexture[ 2 ];

			m_temporalAccumRT[ 0 ] = safeAllocateRT( "_AO_TemporalAccum_0",
													m_target.width,
													m_target.height,
													m_accumTemporalRTFormat,
													RenderTextureReadWrite.Linear,
													FilterMode.Bilinear,
													antiAliasing );

			m_temporalAccumRT[ 1 ] = safeAllocateRT( "_AO_TemporalAccum_1",
													m_target.width,
													m_target.height,
													m_accumTemporalRTFormat,
													RenderTextureReadWrite.Linear,
													FilterMode.Bilinear,
													antiAliasing );

			clearHistory = true;
		}

		if( ( m_prevSampleCount != SampleCount ) ||
			( m_prevDownsample != Downsample ) ||
			( m_prevBlurEnabled != BlurEnabled ) ||
			( m_prevBlurPasses != BlurPasses ) ||
			( m_prevBlurRadius != BlurRadius ) ||
			( m_prevFilterEnabled != FilterEnabled ) ||
			( m_prevHDR != HDR ) ||
			( m_prevMSAA != MSAA ) )
		{
			clearHistory |= ( m_prevHDR != HDR );
			clearHistory |= ( m_prevMSAA != MSAA );

			m_HDR = HDR;
			m_MSAA = MSAA;

			m_paramsChanged = true;
		}

		if ( clearHistory && FilterEnabled )
		{
			ClearHistory();
		}

		return m_paramsChanged;
	}
	private void updateParams()
	{
		m_prevSampleCount = SampleCount;
		m_prevDownsample = Downsample;
		m_prevBlurEnabled = BlurEnabled;
		m_prevBlurPasses = BlurPasses;
		m_prevBlurRadius = BlurRadius;
		m_prevFilterEnabled = FilterEnabled;
		m_prevHDR = m_HDR;
		m_prevMSAA = m_MSAA;

		m_paramsChanged = false;
	}

	void Update()
	{
		if( m_targetCamera.actualRenderingPath != RenderingPath.DeferredShading )
		{
			if( PerPixelNormals != PerPixelNormalSource.None && PerPixelNormals != PerPixelNormalSource.Camera )
			{
				m_paramsChanged = true;
				PerPixelNormals = PerPixelNormalSource.Camera;
				UnityEngine.Debug.LogWarning( "[AmplifyOcclusion] GBuffer Normals only available in Camera Deferred Shading mode. Switched to Camera source." );
			}

			if( ApplyMethod == ApplicationMethod.Deferred )
			{
				m_paramsChanged = true;
				ApplyMethod = ApplicationMethod.PostEffect;
				UnityEngine.Debug.LogWarning( "[AmplifyOcclusion] Deferred Method requires a Deferred Shading path. Switching to Post Effect Method." );
			}
		}
		else
		{
			if( PerPixelNormals == PerPixelNormalSource.Camera )
			{
				m_paramsChanged = true;
				PerPixelNormals = PerPixelNormalSource.GBuffer;
				UnityEngine.Debug.LogWarning( "[AmplifyOcclusion] Camera Normals not supported for Deferred Method. Switching to GBuffer Normals." );
			}
		}

		if( ( m_targetCamera.depthTextureMode & DepthTextureMode.Depth ) == 0 )
		{
			m_targetCamera.depthTextureMode |= DepthTextureMode.Depth;
		}

		if( ( PerPixelNormals == PerPixelNormalSource.Camera ) &&
				( m_targetCamera.depthTextureMode & DepthTextureMode.DepthNormals ) == 0 )
		{
			m_targetCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}

		if( ( UsingMotionVectors == true ) &&
			( m_targetCamera.depthTextureMode & DepthTextureMode.MotionVectors ) == 0 )
		{
			m_targetCamera.depthTextureMode |= DepthTextureMode.MotionVectors;
		}
	}


	void OnPreRender()
	{
		Profiler.BeginSample( "AO - OnPreRender" );

		checkMaterials( true );

		if( m_targetCamera != null )
		{
			bool deferredReflections = ( GraphicsSettings.GetShaderMode( BuiltinShaderType.DeferredReflections ) != BuiltinShaderMode.Disabled );

			if( ( m_prevPerPixelNormals != PerPixelNormals ) ||
				( m_prevApplyMethod != ApplyMethod ) ||
				( m_prevDeferredReflections != deferredReflections ) ||
				( m_commandBuffer_Occlusion.cmdBuffer == null ) ||
				( m_commandBuffer_Apply.cmdBuffer == null )
				)
			{
				CameraEvent cameraStage = CameraEvent.BeforeImageEffectsOpaque;
				if( ApplyMethod == ApplicationMethod.Deferred )
				{
					cameraStage = deferredReflections ? CameraEvent.BeforeReflections : CameraEvent.BeforeLighting;
				}

				createCommandBuffer( ref m_commandBuffer_Occlusion, "AmplifyOcclusion_Compute", cameraStage );
				createCommandBuffer( ref m_commandBuffer_Apply, "AmplifyOcclusion_Apply", cameraStage );

				m_prevPerPixelNormals = PerPixelNormals;
				m_prevApplyMethod = ApplyMethod;
				m_prevDeferredReflections = deferredReflections;

				m_paramsChanged = true;
			}

			if( ( m_commandBuffer_Occlusion.cmdBuffer != null ) &&
				( m_commandBuffer_Apply.cmdBuffer != null ) )
			{
				m_curStepIdx = m_sampleStep & 1;

				UpdateGlobalShaderConstants();

				checkParamsChanged();

				UpdateGlobalShaderConstants_AmbientOcclusion();
				UpdateGlobalShaderConstants_Matrices();

				if( m_paramsChanged )
				{
					m_commandBuffer_Occlusion.cmdBuffer.Clear();

					commandBuffer_FillComputeOcclusion( m_commandBuffer_Occlusion.cmdBuffer );
				}


				m_commandBuffer_Apply.cmdBuffer.Clear();

				if( ApplyMethod == ApplicationMethod.Debug )
				{
					commandBuffer_FillApplyDebug( m_commandBuffer_Apply.cmdBuffer );
				}
				else
				{
					if( ApplyMethod == ApplicationMethod.PostEffect )
					{
						commandBuffer_FillApplyPostEffect( m_commandBuffer_Apply.cmdBuffer );
					}
					else
					{
						bool logTarget = !m_HDR;

						commandBuffer_FillApplyDeferred( m_commandBuffer_Apply.cmdBuffer, logTarget );
					}
				}

				updateParams();

				m_sampleStep++; // No clamp, free running counter
			}
		}

		Profiler.EndSample();
	}


	void OnPostRender()
	{
		if( m_occlusionDepthRT != null )
		{
			m_occlusionDepthRT.MarkRestoreExpected();
		}
		if( m_temporalAccumRT != null )
		{
			foreach ( var rt in m_temporalAccumRT )
			{
				rt.MarkRestoreExpected();
			}
		}
	}

	private int safeAllocateTemporaryRT( CommandBuffer cb, string propertyName,
										int width, int height,
										RenderTextureFormat format = RenderTextureFormat.Default,
										RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default,
										FilterMode filterMode = FilterMode.Point )
	{
		int id = Shader.PropertyToID( propertyName );

		cb.GetTemporaryRT( id, width, height, 0, filterMode, format, readWrite );

		return id;
	}


	private void safeReleaseTemporaryRT( CommandBuffer cb, int id )
	{
		cb.ReleaseTemporaryRT( id );
	}


	private RenderTexture safeAllocateRT(	string name,
											int width, int height,
											RenderTextureFormat format,
											RenderTextureReadWrite readWrite,
											FilterMode filterMode = FilterMode.Point,
											int antiAliasing = 1 )
	{
		width = Mathf.Clamp( width, 1, 65536 );
		height = Mathf.Clamp( height, 1, 65536 );

		RenderTexture rt = new RenderTexture( width, height, 0, format, readWrite ) { hideFlags = HideFlags.DontSave };

		rt.name = name;
		rt.filterMode = filterMode;
		rt.wrapMode = TextureWrapMode.Clamp;
		rt.antiAliasing = Mathf.Max( antiAliasing, 1 );
		rt.Create();

		return rt;
	}


	private void safeReleaseRT( ref RenderTexture rt )
	{
		if( rt != null )
		{
			RenderTexture.active = null;

			rt.Release();
			RenderTexture.DestroyImmediate( rt );

			rt = null;
		}
	}


	private RenderTexture m_occlusionDepthRT = null;
	private RenderTexture[] m_temporalAccumRT = null;

	private uint m_sampleStep = 0;
	private uint m_curStepIdx = 0;
	private static readonly int PerPixelNormalSourceCount = 4;

	private Matrix4x4 m_prevViewProjMatrixLeft = Matrix4x4.identity;
	private Matrix4x4 m_prevInvViewProjMatrixLeft = Matrix4x4.identity;
	private Matrix4x4 m_prevViewProjMatrixRight = Matrix4x4.identity;
	private Matrix4x4 m_prevInvViewProjMatrixRight = Matrix4x4.identity;

	private static readonly float[] m_temporalRotations = { 60.0f, 300.0f, 180.0f, 240.0f, 120.0f, 0.0f };
	private static readonly float[] m_spatialOffsets = { 0.0f, 0.5f, 0.25f, 0.75f };


	private void BeginSample( CommandBuffer cb, string name )
	{
		cb.BeginSample( name );
	}

	private void EndSample( CommandBuffer cb, string name )
	{
		cb.EndSample( name );
	}

	private void commandBuffer_FillComputeOcclusion( CommandBuffer cb )
	{
		BeginSample( cb, "AO 1 - ComputeOcclusion" );

		if( ( PerPixelNormals == PerPixelNormalSource.GBuffer ) ||
			( PerPixelNormals == PerPixelNormalSource.GBufferOctaEncoded ) )
		{
			cb.SetGlobalTexture( PropertyID._AO_GBufferNormals, BuiltinRenderTextureType.GBuffer2 );
		}

		Vector4 oneOverSize_Size = new Vector4( m_target.oneOverWidth,
												m_target.oneOverHeight,
												m_target.width,
												m_target.height );

		int sampleCountPass = ( (int)SampleCount ) * PerPixelNormalSourceCount;

		int occlusionPass = ( ShaderPass.OcclusionLow_None +
								sampleCountPass +
								( (int)PerPixelNormals ) );

		if( Downsample )
		{
			int tmpSmallOcclusionRT = safeAllocateTemporaryRT( cb, "_AO_SmallOcclusionTexture",
															m_target.width / 2, m_target.height / 2,
															m_occlusionRTFormat,
															RenderTextureReadWrite.Linear,
															FilterMode.Bilinear );


			cb.SetGlobalVector( PropertyID._AO_Target_TexelSize, new Vector4( 1.0f / ( m_target.width / 2.0f ),
																		1.0f / ( m_target.height / 2.0f ),
																		m_target.width / 2.0f,
																		m_target.height / 2.0f ) );

			cb.SetRenderTarget( tmpSmallOcclusionRT );
			PerformBlit( cb, m_occlusionMat, occlusionPass );

			cb.SetRenderTarget( default( RenderTexture ) );
			EndSample( cb, "AO 1 - ComputeOcclusion" );

			commandBuffer_Blur( cb, tmpSmallOcclusionRT, m_target.width / 2, m_target.height / 2 );

			// Combine
			BeginSample( cb, "AO 2b - Combine" );

			cb.SetGlobalTexture( PropertyID._AO_Source, tmpSmallOcclusionRT );

			cb.SetGlobalVector( PropertyID._AO_Target_TexelSize, oneOverSize_Size );

			cb.SetRenderTarget( m_occlusionDepthRT );
			PerformBlit( cb, m_occlusionMat, ShaderPass.CombineDownsampledOcclusionDepth );

			safeReleaseTemporaryRT( cb, tmpSmallOcclusionRT );

			cb.SetRenderTarget( default( RenderTexture ) );
			EndSample( cb, "AO 2b - Combine" );
		}
		else
		{
			cb.SetGlobalVector( PropertyID._AO_Source_TexelSize, oneOverSize_Size );
			cb.SetGlobalVector( PropertyID._AO_Target_TexelSize, oneOverSize_Size );

			cb.SetRenderTarget( m_occlusionDepthRT );
			PerformBlit( cb, m_occlusionMat, occlusionPass );

			cb.SetRenderTarget( default( RenderTexture ) );
			EndSample( cb, "AO 1 - ComputeOcclusion" );

			if( BlurEnabled == true )
			{
				commandBuffer_Blur( cb, m_occlusionDepthRT, m_target.width, m_target.height );
			}
		}
	}


	void commandBuffer_Blur( CommandBuffer cb, RenderTargetIdentifier aSourceRT, int aSourceWidth, int aSourceHeight )
	{
		BeginSample( cb, "AO 2 - Blur" );

		int tmpBlurRT = safeAllocateTemporaryRT( cb, "_AO_BlurTmp",
													aSourceWidth, aSourceHeight,
													m_occlusionRTFormat,
													RenderTextureReadWrite.Linear,
													FilterMode.Point );

		// Apply Cross Bilateral Blur
		for( int i = 0; i < BlurPasses; i++ )
		{
			// Horizontal
			cb.SetGlobalTexture( PropertyID._AO_Source, aSourceRT );

			int blurHorizontalPass = ShaderPass.BlurHorizontal1 + ( BlurRadius - 1 ) * 2;

			cb.SetRenderTarget( tmpBlurRT );

			PerformBlit( cb, m_blurMat, blurHorizontalPass );


			// Vertical
			cb.SetGlobalTexture( PropertyID._AO_Source, tmpBlurRT );

			int blurVerticalPass = ShaderPass.BlurVertical1 + ( BlurRadius - 1 ) * 2;

			cb.SetRenderTarget( aSourceRT );

			PerformBlit( cb, m_blurMat, blurVerticalPass );
		}

		safeReleaseTemporaryRT( cb, tmpBlurRT );

		cb.SetRenderTarget( default( RenderTexture ) );
		EndSample( cb, "AO 2 - Blur" );
	}

	int getTemporalPass()
	{
		return  ( ( UsingMotionVectors == true ) ? ( 1 << 1 ) : 0 ) |
				( ( TemporalDilation == true ) ? ( 1 << 0 ) : 0 );
	}

	void commandBuffer_TemporalFilter( CommandBuffer cb )
	{
		// Temporal Filter
		float temporalAdj = Mathf.Lerp( 0.01f, 0.99f, FilterBlending );
		float temporalRotation = m_temporalRotations[ m_sampleStep % 6 ];
		float temporalOffset = m_spatialOffsets[ ( m_sampleStep / 6 ) % 4 ];

		cb.SetGlobalFloat( PropertyID._AO_TemporalCurveAdj, temporalAdj );
		cb.SetGlobalFloat( PropertyID._AO_TemporalMotionSensibility, FilterResponse * FilterResponse + 0.01f );

		cb.SetGlobalTexture( PropertyID._AO_CurrOcclusionDepth, m_occlusionDepthRT );
		cb.SetGlobalTexture( PropertyID._AO_TemporalAccumm, m_temporalAccumRT[ 1 - m_curStepIdx ] );

		cb.SetGlobalFloat( PropertyID._AO_TemporalDirections, FilterEnabled ? temporalRotation / 360.0f : 0 );
		cb.SetGlobalFloat( PropertyID._AO_TemporalOffsets, FilterEnabled ? temporalOffset : 0 );
	}

	private readonly RenderTargetIdentifier[] m_applyDeferredTargets =
	{
		BuiltinRenderTextureType.GBuffer0,		// RGB: Albedo, A: Occ
		BuiltinRenderTextureType.CameraTarget,	// RGB: Emission, A: None
	};

	private readonly RenderTargetIdentifier[] m_applyDeferredTargets_Log =
	{
		BuiltinRenderTextureType.GBuffer0,		// RGB: Albedo, A: Occ
		BuiltinRenderTextureType.GBuffer3		// RGB: Emission, A: None
	};

	void commandBuffer_FillApplyDeferred( CommandBuffer cb, bool logTarget )
	{
		BeginSample( cb, "AO 3 - ApplyDeferred" );

		if( !logTarget )
		{
			if( UsingTemporalFilter )
			{
				commandBuffer_TemporalFilter( cb );

				int applyOcclusionRT = 0;
				if ( useMRTBlendingFallback )
				{
					applyOcclusionRT = safeAllocateTemporaryRT( cb, "_AO_ApplyOcclusionTexture", m_target.fullWidth, m_target.fullHeight, RenderTextureFormat.ARGB32 );

					applyOcclusionTemporal[0] = applyOcclusionRT;
					applyOcclusionTemporal[1] = new RenderTargetIdentifier( m_temporalAccumRT[ m_curStepIdx ] );

					cb.SetRenderTarget( applyOcclusionTemporal, applyOcclusionTemporal[ 0 ] /* Not used, just to make Unity happy */ );
					PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyPostEffectTemporal + getTemporalPass() ); // re-use ApplyPostEffectTemporal pass to apply without Blend to the RT.
				}
				else
				{
					applyDeferredTargetsTemporal[0] = m_applyDeferredTargets[0];
					applyDeferredTargetsTemporal[1] = m_applyDeferredTargets[1];
					applyDeferredTargetsTemporal[2] = new RenderTargetIdentifier( m_temporalAccumRT[ m_curStepIdx ] );

					cb.SetRenderTarget( applyDeferredTargetsTemporal, applyDeferredTargetsTemporal[ 0 ] /* Not used, just to make Unity happy */ );
					PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyDeferredTemporal + getTemporalPass() );
				}

				if ( useMRTBlendingFallback )
				{
					cb.SetGlobalTexture( "_AO_ApplyOcclusionTexture", applyOcclusionRT );

					applyOcclusionTemporal[0] = m_applyDeferredTargets[0];
					applyOcclusionTemporal[1] = m_applyDeferredTargets[1];

					cb.SetRenderTarget( applyOcclusionTemporal, applyOcclusionTemporal[ 0 ] /* Not used, just to make Unity happy */ );
					PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyDeferredTemporalMultiply );

					safeReleaseTemporaryRT( cb, applyOcclusionRT );
				}
			}
			else
			{
				cb.SetGlobalTexture( PropertyID._AO_OcclusionTexture, m_occlusionDepthRT );

				// Multiply Occlusion
				cb.SetRenderTarget( m_applyDeferredTargets, m_applyDeferredTargets[ 0 ] /* Not used, just to make Unity happy */ );
				PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyDeferred );
			}
		}
		else
		{
			// Copy Albedo and Emission to temporary buffers
			int gbufferAlbedoRT = safeAllocateTemporaryRT( cb, "_AO_tmpAlbedo",
																m_target.fullWidth, m_target.fullHeight,
																RenderTextureFormat.ARGB32 );

			int gbufferEmissionRT = safeAllocateTemporaryRT( cb, "_AO_tmpEmission",
																m_target.fullWidth, m_target.fullHeight,
																m_temporaryEmissionRTFormat );

			cb.Blit( BuiltinRenderTextureType.GBuffer0, gbufferAlbedoRT );
			cb.Blit( BuiltinRenderTextureType.GBuffer3, gbufferEmissionRT );

			cb.SetGlobalTexture( PropertyID._AO_GBufferAlbedo, gbufferAlbedoRT );
			cb.SetGlobalTexture( PropertyID._AO_GBufferEmission, gbufferEmissionRT );

			if( UsingTemporalFilter )
			{
				commandBuffer_TemporalFilter( cb );

				applyDeferredTargets_Log_Temporal[0] = m_applyDeferredTargets_Log[0];
				applyDeferredTargets_Log_Temporal[1] = m_applyDeferredTargets_Log[1];
				applyDeferredTargets_Log_Temporal[2] = new RenderTargetIdentifier( m_temporalAccumRT[ m_curStepIdx ] );

				cb.SetRenderTarget( applyDeferredTargets_Log_Temporal, applyDeferredTargets_Log_Temporal[ 0 ] /* Not used, just to make Unity happy */ );
				PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyDeferredLogTemporal + getTemporalPass() );
			}
			else
			{
				cb.SetGlobalTexture( PropertyID._AO_OcclusionTexture, m_occlusionDepthRT );

				cb.SetRenderTarget( m_applyDeferredTargets_Log, m_applyDeferredTargets_Log[ 0 ] /* Not used, just to make Unity happy */ );
				PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyDeferredLog );
			}

			safeReleaseTemporaryRT( cb, gbufferAlbedoRT );
			safeReleaseTemporaryRT( cb, gbufferEmissionRT );
		}

		cb.SetRenderTarget( default( RenderTexture ) );
		EndSample( cb, "AO 3 - ApplyDeferred" );
	}


	void commandBuffer_FillApplyPostEffect( CommandBuffer cb )
	{
		BeginSample( cb, "AO 3 - ApplyPostEffect" );

		if( UsingTemporalFilter )
		{
			commandBuffer_TemporalFilter( cb );

			int applyOcclusionRT = 0;
			if ( useMRTBlendingFallback )
			{
				applyOcclusionRT = safeAllocateTemporaryRT( cb, "_AO_ApplyOcclusionTexture", m_target.fullWidth, m_target.fullHeight, RenderTextureFormat.ARGB32 );
				applyPostEffectTargetsTemporal[ 0 ] = applyOcclusionRT;
			}
			else
			{
				applyPostEffectTargetsTemporal[ 0 ] = BuiltinRenderTextureType.CameraTarget;
			}

			applyPostEffectTargetsTemporal[1] = new RenderTargetIdentifier( m_temporalAccumRT[ m_curStepIdx ] );

			cb.SetRenderTarget( applyPostEffectTargetsTemporal, applyPostEffectTargetsTemporal[ 0 ] /* Not used, just to make Unity happy */ );
			PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyPostEffectTemporal + getTemporalPass() );

			if ( useMRTBlendingFallback )
			{
				cb.SetGlobalTexture( "_AO_ApplyOcclusionTexture", applyOcclusionRT );

				cb.SetRenderTarget( BuiltinRenderTextureType.CameraTarget );
				PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyPostEffectTemporalMultiply );

				safeReleaseTemporaryRT( cb, applyOcclusionRT );
			}
		}
		else
		{
			cb.SetGlobalTexture( PropertyID._AO_OcclusionTexture, m_occlusionDepthRT );

			cb.SetRenderTarget( BuiltinRenderTextureType.CameraTarget );
			PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyPostEffect );
		}

		cb.SetRenderTarget( default( RenderTexture ) );
		EndSample( cb, "AO 3 - ApplyPostEffect" );
	}


	void commandBuffer_FillApplyDebug( CommandBuffer cb )
	{
		BeginSample( cb, "AO 3 - ApplyDebug" );

		if( UsingTemporalFilter )
		{
			commandBuffer_TemporalFilter( cb );

			applyDebugTargetsTemporal[0] = BuiltinRenderTextureType.CameraTarget;
			applyDebugTargetsTemporal[1] = new RenderTargetIdentifier( m_temporalAccumRT[ m_curStepIdx ] );

			cb.SetRenderTarget( applyDebugTargetsTemporal, applyDebugTargetsTemporal[ 0 ] /* Not used, just to make Unity happy */ );
			PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyDebugTemporal + getTemporalPass() );
		}
		else
		{
			cb.SetGlobalTexture( PropertyID._AO_OcclusionTexture, m_occlusionDepthRT );

			cb.SetRenderTarget( BuiltinRenderTextureType.CameraTarget );
			PerformBlit( cb, m_applyOcclusionMat, ShaderPass.ApplyDebug );
		}

		cb.SetRenderTarget( default( RenderTexture ) );
		EndSample( cb, "AO 3 - ApplyDebug" );
	}

	private TargetDesc m_target = new TargetDesc();

	private struct TargetDesc
	{
		public int fullWidth;
		public int fullHeight;
		public int width;
		public int height;
		public float oneOverWidth;
		public float oneOverHeight;
	}

	private bool isStereoSinglePassEnabled()
	{
	#if UNITY_EDITOR
		return m_targetCamera.stereoEnabled && ( PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass );
	#else

		#if UNITY_2017_2_OR_NEWER && !UNITY_SWITCH && !UNITY_XBOXONE && !UNITY_PS4
			return	m_targetCamera.stereoEnabled && ( UnityEngine.XR.XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes );
		#else
			return	false;
		#endif

	#endif
	}

	void UpdateGlobalShaderConstants()
	{
	#if UNITY_2017_2_OR_NEWER && !UNITY_SWITCH && !UNITY_XBOXONE && !UNITY_PS4
		if( UnityEngine.XR.XRSettings.enabled == true )
		{
			m_target.fullWidth = (int)( UnityEngine.XR.XRSettings.eyeTextureDesc.width * UnityEngine.XR.XRSettings.eyeTextureResolutionScale );
			m_target.fullHeight = (int)( UnityEngine.XR.XRSettings.eyeTextureDesc.height * UnityEngine.XR.XRSettings.eyeTextureResolutionScale );
		}
		else
		{
			m_target.fullWidth = m_targetCamera.pixelWidth;
			m_target.fullHeight = m_targetCamera.pixelHeight;
		}
	#else
		m_target.fullWidth = m_targetCamera.pixelWidth;
		m_target.fullHeight = m_targetCamera.pixelHeight;
	#endif

		m_target.width = m_target.fullWidth;
		m_target.height = m_target.fullHeight;

		m_target.oneOverWidth = 1.0f / (float)m_target.width;
		m_target.oneOverHeight = 1.0f / (float)m_target.height;

		float fovRad = m_targetCamera.fieldOfView * Mathf.Deg2Rad;

		float invHalfTanFov = 1.0f / Mathf.Tan( fovRad * 0.5f );

		Vector2 focalLen = new Vector2( invHalfTanFov * ( m_target.height / (float)m_target.width ),
										invHalfTanFov );

		Vector2 invFocalLen = new Vector2( 1.0f / focalLen.x, 1.0f / focalLen.y );

		// Aspect Ratio
		Shader.SetGlobalVector( PropertyID._AO_UVToView, new Vector4( +2.0f * invFocalLen.x,
																+2.0f * invFocalLen.y,
																-1.0f * invFocalLen.x,
																-1.0f * invFocalLen.y ) );

		float projScale;

		if( m_targetCamera.orthographic )
			projScale = ( (float)m_target.height ) / m_targetCamera.orthographicSize;
		else
			projScale = ( (float)m_target.height ) / ( Mathf.Tan( fovRad * 0.5f ) * 2.0f );

		if( Downsample == true )
		{
			projScale = projScale * 0.5f * 0.5f;
		}
		else
		{
			projScale = projScale * 0.5f;
		}

		Shader.SetGlobalFloat( PropertyID._AO_HalfProjScale, projScale );

		// Distance Fade
		if( FadeEnabled == true )
		{
			FadeStart = Mathf.Max( 0.0f, FadeStart );
			FadeLength = Mathf.Max( 0.01f, FadeLength );

			float rcpFadeLength = 1.0f / FadeLength;

			Shader.SetGlobalVector( PropertyID._AO_FadeParams, new Vector2( FadeStart, rcpFadeLength ) );
			float invFadeThickness = ( 1.0f - FadeToThickness );
			Shader.SetGlobalVector( PropertyID._AO_FadeValues, new Vector4( FadeToIntensity, FadeToRadius, FadeToPowerExponent, ( 1.0f - invFadeThickness * invFadeThickness ) * 0.98f ) );
			Shader.SetGlobalColor( PropertyID._AO_FadeToTint, new Color( FadeToTint.r, FadeToTint.g, FadeToTint.b, 0.0f ) );
		}
		else
		{
			Shader.SetGlobalVector( PropertyID._AO_FadeParams, new Vector2( 0.0f, 0.0f ) );
		}
	}

	void UpdateGlobalShaderConstants_AmbientOcclusion()
	{
		// Ambient Occlusion
		Shader.SetGlobalFloat( PropertyID._AO_Radius, Radius );
		Shader.SetGlobalFloat( PropertyID._AO_PowExponent, PowerExponent );
		Shader.SetGlobalFloat( PropertyID._AO_Bias, Bias * Bias );
		Shader.SetGlobalColor( PropertyID._AO_Levels, new Color( Tint.r, Tint.g, Tint.b, Intensity ) );
		float invThickness = ( 1.0f - Thickness );
		Shader.SetGlobalFloat( PropertyID._AO_ThicknessDecay, ( 1.0f - invThickness * invThickness ) * 0.98f );

		if( BlurEnabled == true )
		{
			Shader.SetGlobalFloat( PropertyID._AO_BlurSharpness, BlurSharpness * 100 );
		}
	}

	void UpdateGlobalShaderConstants_Matrices()
	{
		// Camera matrixes
		if( isStereoSinglePassEnabled() == true )
		{
			Matrix4x4 viewLeft = m_targetCamera.GetStereoViewMatrix( Camera.StereoscopicEye.Left );
			Matrix4x4 viewRight = m_targetCamera.GetStereoViewMatrix( Camera.StereoscopicEye.Right );

			Shader.SetGlobalMatrix( PropertyID._AO_CameraViewLeft, viewLeft );
			Shader.SetGlobalMatrix( PropertyID._AO_CameraViewRight, viewRight );

			Matrix4x4 projectionMatrixLeft = m_targetCamera.GetStereoProjectionMatrix( Camera.StereoscopicEye.Left );
			Matrix4x4 projectionMatrixRight = m_targetCamera.GetStereoProjectionMatrix( Camera.StereoscopicEye.Right );

			Matrix4x4 projLeft = GL.GetGPUProjectionMatrix( projectionMatrixLeft, false );
			Matrix4x4 projRight = GL.GetGPUProjectionMatrix( projectionMatrixRight, false );

			Shader.SetGlobalMatrix( PropertyID._AO_ProjMatrixLeft, projLeft );
			Shader.SetGlobalMatrix( PropertyID._AO_ProjMatrixRight, projRight );

			if( UsingTemporalFilter )
			{
				Matrix4x4 ViewProjMatrixLeft = projLeft * viewLeft;
				Matrix4x4 ViewProjMatrixRight = projRight * viewRight;

				Matrix4x4 InvViewProjMatrixLeft = Matrix4x4.Inverse( ViewProjMatrixLeft );
				Matrix4x4 InvViewProjMatrixRight = Matrix4x4.Inverse( ViewProjMatrixRight );

				Shader.SetGlobalMatrix( PropertyID._AO_InvViewProjMatrixLeft, InvViewProjMatrixLeft );
				Shader.SetGlobalMatrix( PropertyID._AO_PrevViewProjMatrixLeft, m_prevViewProjMatrixLeft );
				Shader.SetGlobalMatrix( PropertyID._AO_PrevInvViewProjMatrixLeft, m_prevInvViewProjMatrixLeft );

				Shader.SetGlobalMatrix( PropertyID._AO_InvViewProjMatrixRight, InvViewProjMatrixRight );
				Shader.SetGlobalMatrix( PropertyID._AO_PrevViewProjMatrixRight, m_prevViewProjMatrixRight );
				Shader.SetGlobalMatrix( PropertyID._AO_PrevInvViewProjMatrixRight, m_prevInvViewProjMatrixRight );

				m_prevViewProjMatrixLeft = ViewProjMatrixLeft;
				m_prevInvViewProjMatrixLeft = InvViewProjMatrixLeft;

				m_prevViewProjMatrixRight = ViewProjMatrixRight;
				m_prevInvViewProjMatrixRight = InvViewProjMatrixRight;
			}
		}
		else
		{
			Matrix4x4 view = m_targetCamera.worldToCameraMatrix;

			Shader.SetGlobalMatrix( PropertyID._AO_CameraViewLeft, view );

			if( UsingTemporalFilter )
			{
				Matrix4x4 proj = GL.GetGPUProjectionMatrix( m_targetCamera.projectionMatrix, false );

				Matrix4x4 ViewProjMatrix = proj * view;
				Matrix4x4 InvViewProjMatrix = Matrix4x4.Inverse( ViewProjMatrix );

				Shader.SetGlobalMatrix( PropertyID._AO_InvViewProjMatrixLeft, InvViewProjMatrix );
				Shader.SetGlobalMatrix( PropertyID._AO_PrevViewProjMatrixLeft, m_prevViewProjMatrixLeft );
				Shader.SetGlobalMatrix( PropertyID._AO_PrevInvViewProjMatrixLeft, m_prevInvViewProjMatrixLeft );

				m_prevViewProjMatrixLeft = ViewProjMatrix;
				m_prevInvViewProjMatrixLeft = InvViewProjMatrix;
			}
		}
	}

	private static class ShaderPass
	{
		public const int CombineDownsampledOcclusionDepth = 16;

		// Blur
		public const int BlurHorizontal1 = 0;
		public const int BlurVertical1 = 1;
		public const int BlurHorizontal2 = 2;
		public const int BlurVertical2 = 3;
		public const int BlurHorizontal3 = 4;
		public const int BlurVertical3 = 5;
		public const int BlurHorizontal4 = 6;
		public const int BlurVertical4 = 7;

		// Apply Occlusion
		public const int ApplyDebug = 0;
		public const int ApplyDebugTemporal = 1;
		public const int ApplyDeferred = 5;
		public const int ApplyDeferredTemporal = 6;
		public const int ApplyDeferredLog = 10;
		public const int ApplyDeferredLogTemporal = 11;
		public const int ApplyPostEffect = 15;
		public const int ApplyPostEffectTemporal = 16;
		public const int ApplyPostEffectTemporalMultiply = 20;
		public const int ApplyDeferredTemporalMultiply = 21;

		// Occlusion Normal Targets
		public const int OcclusionLow_None = 0;
		public const int OcclusionLow_Camera = 1;
		public const int OcclusionLow_GBuffer = 2;
		public const int OcclusionLow_GBufferOctaEncoded = 3;
	}

	private static class PropertyID
	{
		public static readonly int _AO_Radius = Shader.PropertyToID( "_AO_Radius" );
		public static readonly int _AO_PowExponent = Shader.PropertyToID( "_AO_PowExponent" );
		public static readonly int _AO_Bias = Shader.PropertyToID( "_AO_Bias" );
		public static readonly int _AO_Levels = Shader.PropertyToID( "_AO_Levels" );
		public static readonly int _AO_ThicknessDecay = Shader.PropertyToID( "_AO_ThicknessDecay" );
		public static readonly int _AO_BlurSharpness = Shader.PropertyToID( "_AO_BlurSharpness" );
		public static readonly int _AO_CameraViewLeft = Shader.PropertyToID( "_AO_CameraViewLeft" );
		public static readonly int _AO_CameraViewRight = Shader.PropertyToID( "_AO_CameraViewRight" );
		public static readonly int _AO_ProjMatrixLeft = Shader.PropertyToID( "_AO_ProjMatrixLeft" );
		public static readonly int _AO_ProjMatrixRight = Shader.PropertyToID( "_AO_ProjMatrixRight" );
		public static readonly int _AO_InvViewProjMatrixLeft = Shader.PropertyToID( "_AO_InvViewProjMatrixLeft" );
		public static readonly int _AO_PrevViewProjMatrixLeft = Shader.PropertyToID( "_AO_PrevViewProjMatrixLeft" );
		public static readonly int _AO_PrevInvViewProjMatrixLeft = Shader.PropertyToID( "_AO_PrevInvViewProjMatrixLeft" );
		public static readonly int _AO_InvViewProjMatrixRight = Shader.PropertyToID( "_AO_InvViewProjMatrixRight" );
		public static readonly int _AO_PrevViewProjMatrixRight = Shader.PropertyToID( "_AO_PrevViewProjMatrixRight" );
		public static readonly int _AO_PrevInvViewProjMatrixRight = Shader.PropertyToID( "_AO_PrevInvViewProjMatrixRight" );
		public static readonly int _AO_GBufferNormals = Shader.PropertyToID( "_AO_GBufferNormals" );
		public static readonly int _AO_Target_TexelSize = Shader.PropertyToID( "_AO_Target_TexelSize" );
		public static readonly int _AO_TemporalCurveAdj = Shader.PropertyToID( "_AO_TemporalCurveAdj" );
		public static readonly int _AO_TemporalMotionSensibility = Shader.PropertyToID( "_AO_TemporalMotionSensibility" );
		public static readonly int _AO_CurrOcclusionDepth = Shader.PropertyToID( "_AO_CurrOcclusionDepth" );
		public static readonly int _AO_TemporalAccumm = Shader.PropertyToID( "_AO_TemporalAccumm" );
		public static readonly int _AO_TemporalDirections = Shader.PropertyToID( "_AO_TemporalDirections" );
		public static readonly int _AO_TemporalOffsets = Shader.PropertyToID( "_AO_TemporalOffsets" );
		public static readonly int _AO_OcclusionTexture = Shader.PropertyToID( "_AO_OcclusionTexture" );
		public static readonly int _AO_GBufferAlbedo = Shader.PropertyToID( "_AO_GBufferAlbedo" );
		public static readonly int _AO_GBufferEmission = Shader.PropertyToID( "_AO_GBufferEmission" );
		public static readonly int _AO_UVToView = Shader.PropertyToID( "_AO_UVToView" );
		public static readonly int _AO_HalfProjScale = Shader.PropertyToID( "_AO_HalfProjScale" );
		public static readonly int _AO_FadeParams = Shader.PropertyToID( "_AO_FadeParams" );
		public static readonly int _AO_FadeValues = Shader.PropertyToID( "_AO_FadeValues" );
		public static readonly int _AO_FadeToTint = Shader.PropertyToID( "_AO_FadeToTint" );
		public static readonly int _AO_Source_TexelSize = Shader.PropertyToID( "_AO_Source_TexelSize" );
		public static readonly int _AO_Source = Shader.PropertyToID( "_AO_Source" );
	}
}
