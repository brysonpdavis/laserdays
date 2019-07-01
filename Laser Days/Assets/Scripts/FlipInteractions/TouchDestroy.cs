using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDestroy : FlipInteraction{

    public bool touched = false;
    bool activated = false;
    Material material;
    Renderer mRenderer;
    private MaterialPropertyBlock _propBlock;
    float duration;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {

            if (!touched)
            { 
                _propBlock.SetFloat("_TransitionState", .5f);
                mRenderer.SetPropertyBlock(_propBlock);//do something that lets you know you've activated it
            }

            touched = true;
        }
    }

    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        mRenderer = GetComponent<Renderer>();
        duration = Toolbox.Instance.globalFlipSpeed;


        mRenderer.GetPropertyBlock(_propBlock);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public override void Interact()
    {
        if (touched && !activated)
        {
            activated = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(InteractionRoutine());
            //Toolbox.Instance.
            //Destroy(gameObject);
        }
    }

    private IEnumerator InteractionRoutine()
    {
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(.5f, 1f, TweeningFunctions.EaseInOut(ratio));

            _propBlock.SetFloat("_TransitionState", value);
            mRenderer.SetPropertyBlock(_propBlock);


            elapsedTime += Time.fixedDeltaTime;
            yield return new  WaitForFixedUpdate();
        }

        mRenderer.enabled = false;

                 
    }


}
