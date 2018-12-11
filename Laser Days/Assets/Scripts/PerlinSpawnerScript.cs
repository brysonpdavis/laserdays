using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinSpawnerScript : MonoBehaviour {

    public Texture2D texture;
    public int sampleDensity;
    public IList<Vector3> samplePoints;
    public int numberToSpawn;
    public GameObject[] prefabList;
    public int spawnAreaSize;
    public Transform parentTransform;
    public bool randomRot;

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
                if (newColor.b > Random.Range(0f, 1f))
                    samplePoints.Add(new Vector3(i / spawnScale, 0, t / spawnScale));
                //int number = Mathf.RoundToInt(newColor.b * 10f);
                //Debug.Log(number + ", " + i + ", "+ t);
                //for (int z = 0; z < number; z++)
                //{
                //    samplePoints.Add(new Vector3(i/spawnScale, 0, t/spawnScale));
                //}
            }
        }

        Debug.Log(samplePoints.Count);


        //randomize 
        for (int i = 0; i < samplePoints.Count; i++)
        {
            Vector3 temp = samplePoints[i];
            int randomIndex = Random.Range(i, samplePoints.Count);
            samplePoints[i] = samplePoints[randomIndex];
            samplePoints[randomIndex] = temp;
        }

        for (int s = 0; s < numberToSpawn; s++)
        {
            //GameObject spawnedObj = Instantiate(prefab);
            GameObject spawnedObj = Instantiate(prefabList[Random.Range(0, prefabList.Length)]);
            if (parentTransform)
                spawnedObj.transform.parent = parentTransform;
            //spawnedObj.transform.position = samplePoints[Random.Range(0, samplePoints.Count)];
            spawnedObj.transform.localPosition = samplePoints[s];
            if (randomRot)
                spawnedObj.transform.Rotate(0f, Random.Range(0.0f, 360.0f), 0f);
            spawnedObj.transform.localScale = new Vector3(2f, Random.Range(2f, 12f), 2f);
            //spawnedObj.GetComponent<Renderer>().material.SetFloat("_colShiftX", Random.Range(0f, 1f));
            //spawnedObj.GetComponent<Renderer>().material.SetFloat("_colShiftX", Random.Range(0f, 1f));
        }

    }
}
