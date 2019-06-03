using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeThatSees : MonoBehaviour {

    private bool isActive = false;
    [SerializeField] private Transform player;
    LineRenderer lineRenderer;
    flipScript flip;
    public bool blockingFlip;

    void Start () {
        player = Toolbox.Instance.GetPlayer().transform;
        flip = player.gameObject.GetComponent<flipScript>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
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
            blockingFlip = false;
    }

    private bool CheckForPlayer()
    {
        RaycastHit hit;

        Vector3 eyeLevel = new Vector3 (player.position.x, player.position.y + 1.5f, player.position.z);

        if (Physics.Linecast(transform.position, eyeLevel, out hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, .1f);

            Vector3 debug = new Vector3(hit.point.x, hit.point.y - .1f, hit.point.z);
            Vector3[] points = new Vector3[]{transform.position, debug};
            lineRenderer.SetPositions(points);
            //Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Player"))
            {
             return true;
            }
                
            else
                return false;
        }

        return false;
    }


}
