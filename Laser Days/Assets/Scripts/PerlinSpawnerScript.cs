using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinSpawnerScript : MonoBehaviour {

    public Texture2D texture;
    public int sampleDensity;
    public IList<Vector3> samplePoints;
    public int numberToSpawn;
    public GameObject prefab;
    public int spawnAreaSize;
    public Transform parentTransform;

    private void Start()
    {
        
    }

    public void Spawn()
    {
        samplePoints = new List<Vector3>();
        int spacingAmount = texture.width / sampleDensity;
        float spawnScale = texture.width/spawnAreaSize;

        for (int i = 0; i < texture.width; i+= spacingAmount)
        {
            for (int t = 0; t < texture.width; t += spacingAmount)
            {
                Color newColor = texture.GetPixel(i, t);
                int number = Mathf.RoundToInt(newColor.b * 10f);
                Debug.Log(number + ", " + i + ", "+ t);

                for (int z = 0; z < number; z++)
                {
                    samplePoints.Add(new Vector3(i/spawnScale, 0, t/spawnScale));
                }
            }
        }

        Debug.Log(samplePoints.Count);

        for (int s = 0; s < numberToSpawn; s++)
        {
            GameObject spawnedObj = Instantiate(prefab);
            if (parentTransform)
                spawnedObj.transform.parent = parentTransform;
            spawnedObj.transform.position = samplePoints[Random.Range(0, samplePoints.Count)];
        }

    }
}
