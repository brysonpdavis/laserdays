using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SobelComposite : MonoBehaviour {


    public Color Line = new Color(0, 0, 0, 1);
    public Shader compositeShader;
    private Material comp;


	private void OnEnable()
	{
        comp = new Material(compositeShader);
	}
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
        comp.SetColor("_Line", Line);
        Graphics.Blit(source, destination, comp, 0);
	}
}
