using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public enum Difficulty { Easy, Medium, Hard };
    public Difficulty difficulty;
    public Scene scene;
    public GameObject button;
    public GameObject myButton;
    public bool completed = false;
    // Use this for initialization



    void Awake()
    {
        scene = gameObject.scene;
        SetupResets();
        //adding to button list here
        button = GameObject.Find("DefaultButton");
        if (button)
        {
            Transform parent = button.transform.parent;
            //if (!parent.transform.Find(scene.name))
            //{

            myButton = Instantiate(button);
            myButton.GetComponent<Image>().enabled = true;
            myButton.GetComponent<Button>().enabled = true;



            myButton.transform.GetChild(0).gameObject.SetActive(true);
            Text text = myButton.GetComponentInChildren<Text>();

            text.text = gameObject.name;
            myButton.name = scene.name;

            Debug.Log("created button gameobject with name: " + myButton.name);

            switch (difficulty)
            {
                case (Difficulty.Easy):
                    myButton.transform.SetParent(LevelLoadingMenu.easyButtons.transform.GetChild(0).transform.GetChild(0));
                    break;
                case (Difficulty.Medium):
                    myButton.transform.SetParent(LevelLoadingMenu.mediumButtons.transform.GetChild(0).transform.GetChild(0));
                    break;
                case (Difficulty.Hard):
                    myButton.transform.SetParent(LevelLoadingMenu.hardButtons.transform.GetChild(0).transform.GetChild(0));
                    break;
            }


            //myButton.transform.SetParent(parent);
            //  }
        }

        //scene is being loaded, button has already been set. now linking existing button to the completion zone to do color change, etc.
        else
        {
            Debug.Log("scene is loaded");
            myButton = GameObject.Find(scene.name);
        }
    }

    public void OnPuzzleCompletion()
    {
        ColorBlock cb = myButton.GetComponent<Button>().colors;
        cb.normalColor = LevelLoadingMenu.completedColor;
        myButton.GetComponent<Button>().colors = cb;


        Color newBackground = myButton.GetComponent<Image>().color;
        newBackground.a = .7f;
        myButton.GetComponent<Image>().color = newBackground;
        completed = true;
    }


    void SetupResets()
    {
        ResetScene[] resets = GetComponentsInChildren<ResetScene>();
        foreach (ResetScene r in resets)
        {
            r.spawnName = gameObject.name;
            r.sceneName = scene.name;
        }
    }
}
