using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActivation : MonoBehaviour {
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask newLayerMask;
    [SerializeField] private float rayLength = 0.5f;

    [Header("Key Settings")]
    [SerializeField]public GameObject key ;
    [SerializeField]public GameObject door;

    

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, newLayerMask.value))
        {
            if (hit.collider == key)
            { door.SetActive(false); }

        }
    }
}
