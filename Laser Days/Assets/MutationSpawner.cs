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
        GameObject newGuy;
        
        
        
        for (int i = 0; i < numSpawnAttempts; i++)
        {
            Vector3 vec;
            
            if (i < 2)
            {
                vec = (transform.forward / 2) - (transform.up / 2) + (0.5f * Random.insideUnitSphere);
            }
            else
            {
                vec = Random.insideUnitSphere;
            }
            RaycastHit hit;

            if(Physics.Raycast(playerTransform.position + eyeLevel, vec, out hit, maxSpawnDistance, LayerMaskController.SharedOnly))
            {
                if(hit.collider.CompareTag("SpawnableSurface"))
                {
                    newGuy = Instantiate(mutation, hit.point, Quaternion.identity);
                    newGuy.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    newGuy.transform.eulerAngles = new Vector3(newGuy.transform.eulerAngles.x, Random.Range(0f, 180f), newGuy.transform.eulerAngles.z);
                }
            }
        }
    }
}
