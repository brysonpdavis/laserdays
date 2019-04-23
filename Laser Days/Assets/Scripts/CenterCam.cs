using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterCam : MonoBehaviour {

    bool on = false;
    public float interval = 0.1f;
    GameObject player;
    private int count = 0;
    

    // Update is called once per frame
    private void Start()
    {
        player = Toolbox.Instance.GetPlayer();
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            on = !on;

            if (on)
            {
                Debug.Log("ROTATION!");
                player.GetComponent<MFPP.Player>().enabled = false;
                //Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                Camera.main.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            }

            else
                Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>().enabled = true;

        }


        if (Input.GetKey(KeyCode.Minus))
        {
            player.transform.position += new Vector3(0f, -1f * interval, 0f);
        }

        if (Input.GetKey(KeyCode.Equals))
        {
            player.transform.position += new Vector3(0f, 1f * interval, 0f);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            ScreenCapture.CaptureScreenshot("/Users/seamusedson/Desktop/" + count + ".png", 1);
            count++;
        }




    }
}
