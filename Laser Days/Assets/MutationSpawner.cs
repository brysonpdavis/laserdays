using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationSpawner : MonoBehaviour {

    public GameObject mutation;

    public float maxSpawnDistance;
    public int numSpawnAttempts;

    private Transform playerTransform;

    private Vector3 eyeLevel = new Vector3(0f, 1.5f, 0f);

    private void Start()
    {
        playerTransform = gameObject.transform;
    }


    public void SpawnMutations()
    {   
        for (int i = 0; i < numSpawnAttempts; i++)
        {
            Vector3 vec = Random.insideUnitSphere;

            RaycastHit hit;

            if(Physics.Raycast(playerTransform.position + eyeLevel, vec, out hit, maxSpawnDistance, LayerMaskController.SharedOnly))
            {
                if(hit.collider.CompareTag("SpawnableSurface"))
                {
                    Instantiate(mutation, hit.point, Quaternion.identity);
                }
            }

        }
    }


	
}
