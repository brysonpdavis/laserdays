using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ReloadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.UnloadSceneAsync(index);
        //SceneManager.LoadSceneAsync(index);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
