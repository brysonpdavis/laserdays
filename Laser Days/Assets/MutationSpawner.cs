using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationSpawner : MonoBehaviour {

    public GameObject mutation;

    public float maxSpawnDistance;
    public int numSpawnAttempts;

    private Transform playerTransform;

    private Vector3 eyeLevel = new Vector3(0f, 1.5f, 0f);

    private GameObject[] pool;

    [SerializeField] 
    private int numberToPool = 255;

    private int poolIndex;

    private void Start()
    {
        playerTransform = gameObject.transform;
        InitializeMutationPool();
    }


    public void SpawnMutations()
    {
        GameObject newGuy;
        
        for (int i = 0; i < numSpawnAttempts; i++)
        {
            Vector3 vec;
            
            if (i < 2)
            {
                vec = (transform.forward) - (transform.up / 2) + (0.5f * Random.insideUnitSphere);
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
                    newGuy = GetAndPlaceNextMutation(hit.point);
                    newGuy.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    newGuy.transform.eulerAngles = new Vector3(newGuy.transform.eulerAngles.x, Random.Range(0f, 180f), newGuy.transform.eulerAngles.z);
                }
            }
        }
    }

    private void InitializeMutationPool()
    {
        pool = new GameObject[255];
        poolIndex = 0;
    }

    private GameObject GetAndPlaceNextMutation(Vector3 position)
    {
        GameObject temp = pool[poolIndex];
        if (! temp)
        {
            pool[poolIndex] = (Instantiate(mutation, position, Quaternion.identity));
        }
        else
        {
            temp.SetActive(false);
            temp.transform.position = position;
            temp.SetActive(true);
        }

        int retIndex = poolIndex;

            poolIndex++;

        if (poolIndex == numberToPool)
        {
            poolIndex = 0;
        }
        
        return pool[retIndex];
    }
}
