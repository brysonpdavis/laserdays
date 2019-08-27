using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenuTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitMenu.Instance.Activate();
        }
    }


}
