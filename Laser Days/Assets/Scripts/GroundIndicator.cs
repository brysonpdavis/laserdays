using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundIndicator : MonoBehaviour {


    private Material mat;
    public float CastLength;
    private bool isSafeGround;

    private LayerMask CurrentLayerMask; 


    public Color safeColor;
    public Color dangerColor;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().sharedMaterial;
	}
	
	// Update is called once per frame
	void Update () {
  
          mat.SetColor("_Color", checkBelow());
	}

    Color checkBelow()
    {
        if (transform.parent.gameObject.layer == 15) { CurrentLayerMask = LayerMaskController.Laser; } // newLayerMask.value = 1024; } //layermask value of layer 10 is 1024 (2^10)  
        else if (transform.parent.gameObject.layer == 16) { CurrentLayerMask = LayerMaskController.Real; }

        RaycastHit firsthit;
        RaycastHit sharedHit;

        Vector3 down = transform.TransformDirection(Vector3.down);



        if (Physics.Raycast(transform.position, down, out firsthit, CastLength, CurrentLayerMask))
        {
            if (firsthit.collider.gameObject.layer == 10 || firsthit.collider.gameObject.layer == 11)
            {
                if (Physics.Raycast(transform.position, down, out sharedHit, 500f, LayerMaskController.SharedOnly))
                {
                    float fallHeight = Mathf.Abs(firsthit.point.y - sharedHit.point.y);

                    if (fallHeight > 1)
                    {
                        return dangerColor;
                    } else 
                    {
                        return safeColor;
                    }
                } else 
                {
                    return safeColor;
                }
            }
            else
            {
                return safeColor;
            }

        }

        return safeColor;

    }
}
