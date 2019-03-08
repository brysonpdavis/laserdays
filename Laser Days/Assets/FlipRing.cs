using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlipRing : MonoBehaviour {

    public float mySpeed = 2f;
    public float startScale = 0f;
    public float endScale = 10f;

    private Renderer mRenderer;
    private Material material;

	// Use this for initialization
	void Start () {
        mRenderer = GetComponent<Renderer>();
        material = mRenderer.material;
        StartCoroutine(RingGrow());
        gameObject.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 180f), 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator RingGrow ()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / mySpeed;
        //int property = Shader.PropertyToID("_D7A8CF01");
        Vector3 startpoint;
        Vector3 endpoint;

        startpoint = new Vector3(startScale, startScale, startScale);
        endpoint = new Vector3(endScale, endScale, endScale);


        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / mySpeed;
            float a = ratio * ratio;
            float b = 1 - ratio;
            b = b * b;
            b = 1 - b;
            float rr = (1 - ratio) * a + ratio * b;



            //float value = Mathf.Lerp(startpoint, endpoint, ratio);
            Vector3 value = Vector3.Lerp(startpoint, endpoint, b);

            transform.localScale = value;


            float v = Mathf.Lerp(0f, 1f, b);

            material.SetFloat("_TransitionState", v);
            //RendererExtensions.UpdateGIMaterials(mRenderer);


            yield return null;
        }

        UnityEngine.Object.Destroy(this.gameObject);
    
}




}
