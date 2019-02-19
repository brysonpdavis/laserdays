using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCompletionDup : MonoBehaviour {

    public Spawner[] mySpawners;
    public int counter = 0;
    private float rotX;
    private float rotY;
    private float rotZ;

    private float innerRotX;
    private float innerRotY;
    private float innerRotZ;
    public Transform innerRing;
    public Transform core;
    Material innerMaterial;
    Renderer innerRenderer;

    Material material;
    Renderer mRenderer;
    float duration = 3f;

    // Use this for initialization
	void Start () {
        mRenderer = GetComponent<MeshRenderer>();
        if (mRenderer)
            material = mRenderer.material;

        mySpawners = transform.parent.transform.parent.GetComponentsInChildren<Spawner>();

        rotX = Random.Range(5f, 10f);
        rotY = Random.Range(30f, 90f);
        rotZ = Random.Range(30f, 40f);

        innerRotX = Random.Range(5f, 10f);
        innerRotY = Random.Range(30f, 90f);
        innerRotZ = Random.Range(40f, 30f);

        innerRenderer = innerRing.GetComponent<MeshRenderer>();
        innerMaterial = innerRenderer.material;
	}


    public void CompletionInteract()
    {

        counter++;
        foreach (Spawner button in mySpawners)
        {
            button.OnPuzzleCompletion();
        }

        if (counter == 1)
        {
            AudioClip clip = SoundBox.Instance.completionZone;
            AudioSource audio = SoundBox.Instance.thisSource;
            audio.clip = clip;
            audio.volume = Toolbox.Instance.soundEffectsVolume;
            audio.Play();
        }

        StartCoroutine(TransitionLaserOff());
        core.GetComponent<Collider>().enabled = false;

    }

    private void Update()
    {
        transform.Rotate(rotX * Time.deltaTime, rotY * Time.deltaTime, rotZ * Time.deltaTime);
        if (core)
            core.Rotate(rotX * Time.deltaTime, rotY * Time.deltaTime, rotZ * Time.deltaTime);
        if (innerRing)
            innerRing.Rotate(innerRotX * Time.deltaTime, innerRotY * Time.deltaTime, innerRotZ * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("what");
        if (other.CompareTag("Player"))
            CompletionInteract();
    }

    private IEnumerator TransitionLaserOff()
    {
        GetComponent<Collider>().enabled = false;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        //int property = Shader.PropertyToID("_D7A8CF01");

        Vector3 start = new Vector3(1f, 1f, 1f);
        Vector3 end = new Vector3(7f, 7f, 7f);

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(1f, 0f, ratio);

            Vector3 current = Vector3.Lerp(start, end, ratio);
            transform.localScale = current;
            innerRing.localScale = current;

            material.SetFloat("_TransitionState", value);
            innerMaterial.SetFloat("_TransitionState", value);
            RendererExtensions.UpdateGIMaterials(mRenderer);
            RendererExtensions.UpdateGIMaterials(innerRenderer);

            yield return null;
        }

        mRenderer.enabled = false;
    }

}
