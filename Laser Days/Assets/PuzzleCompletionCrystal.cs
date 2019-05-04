using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCompletionCrystal : MonoBehaviour {

    public Spawner[] mySpawners;
    public int counter = 0;
  
    Material material;
    Renderer mRenderer;
    public float duration = 3f;

    private CompletionBurst[] myBursts;


    // Use this for initialization
	void Start () {
        mRenderer = GetComponent<MeshRenderer>();
        if (mRenderer)
            material = mRenderer.material;

        mySpawners = transform.parent.GetComponentsInChildren<Spawner>();

        myBursts = GetComponentsInChildren<CompletionBurst>();

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

            foreach (CompletionBurst b in myBursts){
                b.DoBusrt();

            }

        }

        StartCoroutine(TransitionLaserOff());


    }

    private void Update()
    {
       
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

       
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(0f, 1f, ratio);
            material.SetFloat("_TransitionState", value);
         
            yield return new WaitForFixedUpdate();
        }

        //mRenderer.enabled = false;
    }

}
