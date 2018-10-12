using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public Scene scene;
    public GameObject button;
    public GameObject myButton;
    // Use this for initialization
    void Start()
    {
        scene = gameObject.scene;

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
                myButton.transform.SetParent(parent);
          //  }
        }
    }

    public void OnPuzzleCompletion()
    {
        ColorBlock cb = myButton.GetComponent<Button>().colors;
        cb.normalColor = Color.green;
        myButton.GetComponent<Button>().colors = cb;


        Color newBackground = myButton.GetComponent<Image>().color;
        newBackground.a = .7f;
        myButton.GetComponent<Image>().color = newBackground;
    }
}
