using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SmoothOutline : MonoBehaviour
{
    private static RenderTexture PrePass;
    private static RenderTexture Line;
    public Shader RP;

    //private Material _blurMat;

    void OnEnable()
    {
        // = new RenderTexture(Screen.width, Screen.height, 24);
        //PrePass.antiAliasing = QualitySettings.antiAliasing;
       // Line = new RenderTexture(Screen.width >> 1, Screen.height >> 1, 0);

        var cam = GetComponent<Camera>();
       // var colorShader = Shader.Find("Custom/SolidColorPass");
        //camera.targetTexture = PrePass;
        cam.SetReplacementShader(RP, "Sobel");
        //Shader.SetGlobalTexture("_ColorPassTex", PrePass);

        //Shader.SetGlobalTexture("_GlowBlurredTex", Line);

        //_blurMat = new Material(Shader.Find("Hidden/Blur"));
        //_blurMat.SetVector("_BlurSize", new Vector2(Blurred.texelSize.x * 1.5f, Blurred.texelSize.y * 1.5f));
    }

    void OnDisable(){
        GetComponent<Camera>().ResetReplacementShader();

    }

    //void OnRenderImage(RenderTexture src, RenderTexture dst)
    //{
        //Graphics.Blit(src, dst);

        //Graphics.SetRenderTarget(Blurred);
        //GL.Clear(false, true, Color.clear);

        //Graphics.Blit(src, Blurred);
        
        //for (int i = 0; i < 4; i++)
        //{
            //var temp = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
            //Graphics.Blit(Blurred, temp, _blurMat, 0);
            //Graphics.Blit(temp, Blurred, _blurMat, 1);
            //RenderTexture.ReleaseTemporary(temp);
        //}
    //}


}