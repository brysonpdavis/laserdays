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

        //set to correct transition state based on player
        if (Toolbox.Instance.GetPlayer().layer == 16)
            terrainMat.SetFloat("_TransitionState", 0f);
        else
            terrainMat.SetFloat("_TransitionState", 1f);

                
    }


}
	

