using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerDrop : MonoBehaviour {

    private Material rendermat;

    public float coolDown = 0.4f;
    public float scrollSpeed = 0.01f;

    private void Start()
    {
        rendermat = GetComponent<Renderer>().material;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Toolbox.Instance.EqualToHeld(collision.gameObject) && collision.gameObject.GetComponent<BasicClickable>())
        {
            Toolbox.Instance.GetPickUp().PutDown();
        }

        //rendermat.SetVector("_Hit", GetTextureHitPoint(collision));
        rendermat.SetFloat("_Blocked", 1f);
    }

    private void OnCollisionStay(Collision collision)
    {
     
        rendermat.SetFloat("_Blocked", 1f);
    }

    private void FixedUpdate()
    {
        rendermat.SetFloat("_Elapsed", rendermat.GetFloat("_Elapsed") + scrollSpeed);
        float current = rendermat.GetFloat("_Blocked");
        if (current <= 0f)
        {
            rendermat.SetFloat("_Blocked", 0f);
            return;
        } else 
        {
            rendermat.SetFloat("_Blocked", current - (Time.fixedDeltaTime/ coolDown));
            return;
        }
    }


    Vector4 GetTextureHitPoint(Collision collision)
    {
        Vector4 vout = Vector4.zero;

        Vector3 wPosPoint = collision.contacts[0].point;
        Vector3 oPosPoint = this.transform.InverseTransformPoint(wPosPoint);

        vout.x = oPosPoint.x + 0.5f;
        vout.y = oPosPoint.y + 0.5f;
        vout.z = 0.2f;
       
        return vout;
    }


}
