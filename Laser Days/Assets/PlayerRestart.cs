using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerRestart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("ayo");
            int index = SceneManager.GetActiveScene().buildIndex;
            //SceneManager.UnloadSceneAsync();
            SceneManager.LoadSceneAsync(index);
            SceneManager.LoadSceneAsync(SceneManager.GetSceneByName("Default_Main_Player").buildIndex);

        }
	}
}
