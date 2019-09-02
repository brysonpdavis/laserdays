using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionActivationCrystal : MonoBehaviour {

    Material material;
    Renderer mRenderer;
    public float duration = 3f;

    private CompletionBurst[] myBursts;
    private GameObject player;
    private flipScript flip;
    private int counter;


    // Use this for initialization
	void Start () {
        mRenderer = GetComponent<MeshRenderer>();
        if (mRenderer)
            material = mRenderer.material;
            
        myBursts = GetComponentsInChildren<CompletionBurst>();
        player = Toolbox.Instance.GetPlayer();
        flip = Toolbox.Instance.GetFlip();
	}


    public void CompletionInteract()
    {

        counter++;

        if (counter == 1)
        {
            AudioClip clip = SoundBox.Instance.completionZone;
            AudioSource audio = SoundBox.Instance.thisSource;
            audio.clip = clip;
            audio.volume = Toolbox.Instance.soundEffectsVolume;
            audio.Play();

            flip.FlipAttempt();
            flip.ForceFlip();
            flip.canFlip = true;

            foreach (CompletionBurst b in myBursts){
                b.DoBusrt();

            }

        }

        StartCoroutine(TransitionLaserOff());


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
            float value = Mathf.Lerp(0f, 1f, TweeningFunctions.EaseOut(ratio));
            material.SetFloat("_TransitionState", value);
         
            yield return new WaitForFixedUpdate();
        }

        //mRenderer.enabled = false;
    }

}
