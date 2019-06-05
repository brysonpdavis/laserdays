using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeThatSees : MonoBehaviour {

    public bool isActive = false;
    [SerializeField] private Transform player;
    //LineRenderer lineRenderer;
    flipScript flip;
    public bool onWall;
    public bool blockingFlip;
    private float startingAngleY;
    private enum EyeDirection{Forward, Backward, Right, Left};
    private EyeDirection myDirection;

    [HideInInspector]
    public SimpleEye eyeParent;

    private LayerMask currentLayerMask;

    void Start () {
        player = Toolbox.Instance.GetPlayer().transform;
        flip = player.gameObject.GetComponent<flipScript>();
        flipScript.OnFailedFlip += FailedFlip;
        eyeParent = GetComponent<SimpleEye>();

        InitializeWallCheck();
        Debug.Log(transform.rotation.eulerAngles.y);
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

    private void FailedFlip()
    {
        if (isActive)
            Debug.Log("FLIPFAILED: called from eye");
    }

    private void Update()
    {
        if (isActive)
        {

            if (CheckForPlayer())
            {
                blockingFlip = true;
                //Debug.Log("woohoo!");
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


            Debug.Log(transform.position - hit.point);

            if (hit.collider.CompareTag("Player") && (WallCheck(hit.point)))
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

    bool WallCheck(Vector3 hit)
    {
        if (!onWall)
            return true;

        else 
        {
            Vector3 relativePos = (transform.position - hit);


            switch (myDirection)
            {
                case EyeDirection.Forward:
                    {
                        if (relativePos.z <= 0)
                            return true;

                        else return false;
                    }

                case EyeDirection.Backward:
                    {
                        if (relativePos.z >= 0)
                            return true;
                        else return false;
                                            }

                case EyeDirection.Right:
                    {
                        if (relativePos.x >= 0)
                            return true;
                        else return false;

                    }

                case EyeDirection.Left:
                    {
                        if (relativePos.x <= 0)
                            return true;
                        else return false;
                    }

                default:
                    return false;
            }

        }

    }

    private void InitializeWallCheck()
    {
        Debug.Log(transform.rotation.eulerAngles.y);
        //facing forward: looking for 0
        if (transform.rotation.eulerAngles.y >= 0f && transform.rotation.eulerAngles.y <= 10f)
            myDirection = EyeDirection.Forward;
        //facing left: looking for 90
        if (transform.rotation.eulerAngles.y >= 80f && transform.rotation.eulerAngles.y <= 100f)
            myDirection = EyeDirection.Left;
        
        //facing backward: looking for 180
        if (transform.rotation.eulerAngles.y >= 170f && transform.rotation.eulerAngles.y <= 190f)
            myDirection = EyeDirection.Backward;

        //facing right: looking for 270
        if (transform.rotation.eulerAngles.y >= 260f && transform.rotation.eulerAngles.y <= 280f)
            myDirection = EyeDirection.Right;

    }


}
