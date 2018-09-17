using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessing_Sobel : MonoBehaviour {

    private RenderTexture PrePass;
    private RenderTexture Sobel;

    private Material sobelMat;
    public Shader sob;

    [Range(0, 3)]
    public float SobelResolution = 1;

    public Color outlineColor;

    private Color w = new Color(1, 1, 1, 1);

    //[SerializeField]
    //private var ReplacementShader;

    //private var cam;


    void Start () {

        var cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;

        var ReplacementShader = Shader.Find("Custom/SolidColorPass");

        //PrePass = new RenderTexture(Screen.width, Screen.height, 24);
        //PrePass.antiAliasing = QualitySettings.antiAliasing;

        //Sobel = new RenderTexture(Screen.width, Screen.height, 0);


        //cam.targetTexture = PrePass;
        cam.SetReplacementShader(ReplacementShader, "RenderType");

        //Shader.SetGlobalTexture("_OutlineTex", Sobel);

        //sobelMat = new Material(Shader.Find("Nasty-Screen/SobelOutline"));
        sobelMat = new Material(sob);
        outlineColor = w;

    }
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
    {

        //Graphics.Blit(source, destination);
        //RenderTexture temp = new RenderTexture(Screen.width, Screen.height, 24);
        //Graphics.Blit(source, temp);

        //Graphics.SetRenderTarget(Sobel);
        //GL.Clear(false, true, Color.clear);
        //Graphics.Blit(source, Sobel);



        sobelMat.SetFloat("_ResX", Screen.width * SobelResolution);
        sobelMat.SetFloat("_ResY", Screen.height * SobelResolution);
        sobelMat.SetColor("_Outline", w);
        Graphics.Blit(source, destination, sobelMat);
    }
}
