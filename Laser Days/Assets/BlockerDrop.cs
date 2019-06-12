using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerDrop : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        if (Toolbox.Instance.EqualToHeld(collision.gameObject))
            Toolbox.Instance.GetPickUp().PutDown();
    }

}
