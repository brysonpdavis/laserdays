using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIconSetter : MonoBehaviour {

    public Texture IconTex;
    private Renderer render;
    private MaterialPropertyBlock propblock;
	
	void Start () 
    {
        render = GetComponent<Renderer>();
        propblock = new MaterialPropertyBlock();
        propblock.SetTexture("_MainTex", IconTex);
        render.SetPropertyBlock(propblock);
	}

    [ExecuteInEditMode]
    private void OnEnable()
    {
        render = GetComponent<Renderer>();
        propblock = new MaterialPropertyBlock();
        propblock.SetTexture("_MainTex", IconTex);
        render.SetPropertyBlock(propblock);
    }
}
