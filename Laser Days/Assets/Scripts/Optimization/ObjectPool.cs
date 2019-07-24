using UnityEngine;

public class ObjectPool
{
    private int poolSize;
    
    private int poolIndex = 0;

    private GameObject[] pool;

    private GameObject prefabToSpawn;
    
    public static ObjectPool InitializePool(GameObject prefab, int size)
    {
        ObjectPool newPool = new ObjectPool();

        newPool.poolSize = size;

        newPool.poolIndex = 0;

        newPool.pool = new GameObject[size];

        newPool.prefabToSpawn = prefab;

        return newPool;
    }
    
    public GameObject GetAndPlaceNextMutation(Vector3 position)
    {
        GameObject temp = pool[poolIndex];
        if (! temp)
        {
            pool[poolIndex] = GameObject.Instantiate(prefabToSpawn, position, Quaternion.identity);
        }
        else
        {
            temp.SetActive(false);
            temp.transform.position = position;
            temp.SetActive(true);
        }

        int retIndex = poolIndex;

        poolIndex++;

        if (poolIndex == poolSize)
        {
            poolIndex = 0;
        }
        
        return pool[retIndex];
    }

    public void DeleteAll()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (pool[i])
                GameObject.Destroy(pool[i]);
        }
    }
}