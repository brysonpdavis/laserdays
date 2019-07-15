using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEye : MonoBehaviour {

    private Transform player;
    private Vector3 currentTarget;
    private Vector3 lastTarget;

    [HideInInspector]
    public Vector3 hitPoint;
    [HideInInspector]
    public bool hittingPlayer;

    //[HideInInspector]
    public EyeBeam beam;

    private EyeThatSees eye;
    public float waitTime;
    public float lerpTime;
    private float timeCounter = 0;
    public float focusedScale;
    public float unfocusedScale;
    private Vector3 previousPosition;

    private bool snapViewRunning = false;
    private bool widenFocusRunning = false;

    void Start () {
        player = Toolbox.Instance.GetPlayer().transform;
        eye = GetComponent<EyeThatSees>();
        beam = GetComponentInChildren<EyeBeam>();

	}

    // Update is called once per frame
    void FixedUpdate()
    {

        if (eye.isActive && eye.WallCheck(eye.currentPlayerPoint))
            EyeActivate();
    }


    private void EyeActivate()
    {
            Vector3 currentPlayer = new Vector3(player.position.x, player.position.y + 1.5f, player.position.z);
            timeCounter += Time.deltaTime;

            beam.SetAudioVolume();

            bool playerHasMoved = false;
            if (((previousPosition - player.position).magnitude) > .1f)
            {
                playerHasMoved = true;
            }

            if (timeCounter >= waitTime)
            {

                if (playerHasMoved)
                    StartCoroutine(SnapView(lastTarget, currentPlayer, lerpTime, playerHasMoved));

                timeCounter = 0f;
                previousPosition = player.position;

            }

            if (!snapViewRunning && !playerHasMoved && !widenFocusRunning)
                StartCoroutine(Focus(.4f));
          
    }

    public void BeamReset()
    {
        StopAllCoroutines();
        StartCoroutine(ResetBeamLength(lerpTime*2f));
    }

    private IEnumerator SnapView (Vector3 old, Vector3 current, float duration, bool playerHasMoved)
    {
        snapViewRunning = true;
        float elapsedTime = 0;
        float ratio = 0;

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            Vector3 view = Vector3.Lerp(old, current, TweeningFunctions.EaseOutCubic(ratio));
            transform.LookAt(view);
            elapsedTime += Time.deltaTime;
            SetBeamLength(hitPoint);

            if (playerHasMoved)
            {
                //doing lerp for focal scale
                float start = beam.transform.localScale.x;
                float width = Mathf.Lerp(start, unfocusedScale, ratio);
                beam.SetWidth(width);
    
            }

            yield return null;
        }

        lastTarget = current;
        snapViewRunning = false;
    }

    private IEnumerator Focus(float duration)
    //for after it's looked at the player
    {
        //Debug.Log("widening focus");
        widenFocusRunning = true;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            elapsedTime += Time.deltaTime;

            //doing lerp for focal scale
            float start = beam.transform.localScale.x;
            float width = Mathf.Lerp(start, focusedScale, TweeningFunctions.EaseOutCubic(ratio));
            beam.SetWidth(width);

            yield return null;
        }

        widenFocusRunning = false;

    }


    private IEnumerator ResetBeamLength(float duration)
    //for after it's looked at the player
    {
        widenFocusRunning = true;
        float elapsedTime = 0;
        float ratio = elapsedTime / duration;

        while (ratio < 1f)
        {
            ratio = elapsedTime / duration;
            elapsedTime += Time.deltaTime;

            //doing lerp for focal scale
            float start = beam.transform.localScale.z;
            float length = Mathf.Lerp(start, 0f, TweeningFunctions.EaseOutCubic(ratio));
            beam.SetLength(length);

            yield return null;
        }

        widenFocusRunning = false;

    }


    void SetBeamLength (Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);
        if (hittingPlayer)
        {
            beam.SetLength(dist - 0.3f);
        } else 
        {
            beam.SetLength(dist);
        }

    }

}
