using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeThatSees : MonoBehaviour {

    public bool isActive = false;
    [SerializeField] private Transform player;
    //LineRenderer lineRenderer;
    flipScript flip;
    public bool blockingFlip;

    [HideInInspector]
    public SimpleEye eyeParent;

    private LayerMask currentLayerMask;

    void Start () {
        player = Toolbox.Instance.GetPlayer().transform;
        flip = player.gameObject.GetComponent<flipScript>();
        //lineRenderer = GetComponentInChildren<LineRenderer>();
        //lineRenderer.positionCount = 2;

        eyeParent = GetComponent<SimpleEye>();

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
            Debug.Log("entered");
            flip.eyeThatSeesList.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            Debug.Log("exited");
            flip.eyeThatSeesList.Remove(this);
        }
    }

    private void Update()
    {
        if (isActive)
        {

            if (CheckForPlayer())
            {
                blockingFlip = true;
                Debug.Log("woohoo!");
            }

            else
                blockingFlip = false;

        }
        else
        {
            blockingFlip = false;
            //eyeParent.hitPoint = Vector3.zero;
        }
    }

    private bool CheckForPlayer()
    {
        RaycastHit hit;

        Vector3 eyeLevel = new Vector3 (player.position.x, player.position.y + 1.5f, player.position.z);

        currentLayerMask = LayerMaskController.GetLayerMaskForRaycast(player.gameObject.layer);

        if (Physics.Linecast(transform.position, eyeLevel, out hit, currentLayerMask))
        {

            eyeParent.hitPoint = hit.point;

            Debug.DrawLine(transform.position, hit.point, Color.red, .1f);

            if (hit.collider.CompareTag("Player"))
            {
                eyeParent.hittingPlayer = true;
                return true;


            }
                
            else
                eyeParent.hittingPlayer = false;
                return false;
        }

        return false;
    }


}
