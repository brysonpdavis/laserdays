using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEye : MonoBehaviour {

    private Transform player;
    private Vector3 currentTarget;
    private Vector3 lastTarget;

    [HideInInspector]
    public Vector3 hitPoint;

    //[HideInInspector]
    public EyeBeam beam;

    private EyeThatSees eye;
    public float waitTime;
    public float lerpTime;
    private float timeCounter = 0;

    // Use this for initialization

    void Start () {
        player = Toolbox.Instance.GetPlayer().transform;
        eye = GetComponent<EyeThatSees>();
        beam = GetComponentInChildren<EyeBeam>();

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //if(eye.isActive)
        //{
            Vector3 currentPlayer = new Vector3(player.position.x, player.position.y + 1.5f, player.position.z);
            timeCounter += Time.deltaTime;

            Debug.Log("time counter:" + timeCounter + ", wait time: " + waitTime);
            
            if (timeCounter >= waitTime)
            {
                StopAllCoroutines();
                Debug.Log("tring to look");
                StartCoroutine(SnapView(lastTarget, currentPlayer, lerpTime));
                timeCounter = 0f;

            }

            

        //}

	}

    private IEnumerator SnapView (Vector3 old, Vector3 current, float duration)
    {
        float elapsedTime = 0;
        float ratio = 0;
        //int property = Shader.PropertyToID("_D7A8CF01");

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            Vector3 view = Vector3.Lerp(old, current, TweeningFunctions.EaseOutCubic(ratio));

            transform.LookAt(view);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lastTarget = current;
        SetBeamLength(hitPoint);
    }


    void SetBeamLength (Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);
        beam.SetLength(dist);
    }

}
