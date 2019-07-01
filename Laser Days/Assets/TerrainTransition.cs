using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTransition : MonoBehaviour {

    private Material terrainMat;
    private Terrain terrain;

	// Use this for initialization
	void Start () {
        terrain = GetComponent<Terrain>();
        terrainMat = terrain.materialTemplate;

        if (!Toolbox.Instance.sharedMaterials.Contains(terrainMat))
        {
            Toolbox.Instance.sharedMaterials.Add(terrainMat);
        }
                
    }


}
	

