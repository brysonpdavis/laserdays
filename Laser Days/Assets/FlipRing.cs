using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipRing : MonoBehaviour {

    public float mySpeed = 2f;
    public float startScale = 0f;
    public float endScale = 10f;

    private Material material;
    private float elapsedTime;
    private Vector3 startVec, endVec;

    private readonly ContactPoint[] contactPoints;

    void Start () 
    {
        material = GetComponent<Renderer>().material;

        startVec = new Vector3(startScale, startScale, startScale);
        endVec = new Vector3(endScale, endScale, endScale);

        elapsedTime = 0f;

        gameObject.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 180f), 0);
        gameObject.transform.localScale = startVec;
    }

	void FixedUpdate () 
    {
        if (elapsedTime < mySpeed)
        {
            var ratio = elapsedTime / mySpeed;

            float currentScale = Mathf.Lerp(startScale, endScale, TweeningFunctions.EaseOutCubic(ratio));

            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            material.SetFloat("_TransitionState", TweeningFunctions.EaseOutCubic(ratio));

            elapsedTime += Time.fixedDeltaTime;
        } 
        else 
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }

	}
}

