using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMenuScript : MonoBehaviour {
    
    [SerializeField] Transform menuPanel;

    Event keyEvent;

    Text buttonText;

    KeyCode newKey;

    bool waitingForKey;



    void Start ()

    {

        //Assign menuPanel to the Panel object in our Canvas

        //Make sure it's not active when the game starts

/*
        menuPanel = transform.Find("Panel");

        menuPanel.gameObject.SetActive(false);
*/

        waitingForKey = false;

 

        /*
         
         * iterate through each child of the panel and check

         * the names of each one. Each if statement will

         * set each button's text component to display

         * the name of the key that is associated

         * with each command. Example: the ForwardKey

         * button will display "W" in the middle of it

         */

        for(int i = 0; i < menuPanel.childCount; i++)

        {


            if (menuPanel.GetChild(i).name == "ForwardKey")

            {
                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.forward.ToString(); 
            }
            else if(menuPanel.GetChild(i).name == "BackwardKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.backward.ToString();

            else if(menuPanel.GetChild(i).name == "LeftKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.left.ToString();

            else if(menuPanel.GetChild(i).name == "RightKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.right.ToString();

            else if(menuPanel.GetChild(i).name == "JumpKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.jump.ToString();

            else if (menuPanel.GetChild(i).name == "PickupKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.pickup.ToString();
            
            else if (menuPanel.GetChild(i).name == "FlipKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.flip.ToString();
            
            else if (menuPanel.GetChild(i).name == "SelectKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.select.ToString();

            else if (menuPanel.GetChild(i).name == "PauseKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.pause.ToString();

            else if (menuPanel.GetChild(i).name == "SubmitKey")

                menuPanel.GetChild(i).Find("PlayerInput").GetComponent<Text>().text = ControlManager.Instance.submit.ToString();

        }

    }

    void OnGUI()
    {
        /* keyEvent dictates what key our user presses
         * by using event.current to detect the current
         * event
         * */
        
        keyEvent = Event.current;

        //Executes if a button gets pressed and

        //the user presses a key

        if(waitingForKey)
        {
            if (keyEvent.isKey)
            {
                newKey = keyEvent.keyCode; //Assigns newKey to the key user presses

                waitingForKey = false;
            }
            else if (keyEvent.isMouse)
            {
                if (Input.GetMouseButtonDown(0))
                    newKey = KeyCode.Mouse0;
                
                else if (Input.GetMouseButtonDown(1))
                    newKey = KeyCode.Mouse1;

                else if (Input.GetMouseButtonDown(2))
                    newKey = KeyCode.Mouse2;

                waitingForKey = false;

            }
            else if (keyEvent.keyCode != KeyCode.None)
            {
                Debug.LogError(keyEvent.keyCode);
            }
        }
    }
    
    /*Buttons cannot call on Coroutines via OnClick().

     * Instead, we have it call StartAssignment, which will

     * call a coroutine in this script instead, only if we

     * are not already waiting for a key to be pressed.

     */


    public void StartAssignment(string keyName)

    {

        if(!waitingForKey)

            StartCoroutine(AssignKey(keyName));

    }
    
    //Assigns buttonText to the text component of

    //the button that was pressed

    public void SendText(Text text)

    {

        buttonText = text;

    }

    //Used for controlling the flow of our below Coroutine

    IEnumerator WaitForKey()

    {

        while(!keyEvent.isKey && !(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))

            yield return null;

    }

 

    /*AssignKey takes a keyName as a parameter. The

     * keyName is checked in a switch statement. Each

     * case assigns the command that keyName represents

     * to the new key that the user presses, which is grabbed

     * in the OnGUI() function, above.

     */

    public IEnumerator AssignKey(string keyName)

    {

        waitingForKey = true;

 

        yield return WaitForKey(); //Executes endlessly until user presses a key

 

        switch(keyName)

        {

        case "forward":

            ControlManager.Instance.forward = newKey; //Set forward to new keycode

            buttonText.text = ControlManager.Instance.forward.ToString(); //Set button text to new key

            PlayerPrefs.SetString("forwardKey", ControlManager.Instance.forward.ToString()); //save new key to PlayerPrefs

            break;

        case "backward":

            ControlManager.Instance.backward = newKey; //set backward to new keycode

            buttonText.text = ControlManager.Instance.backward.ToString(); //set button text to new key

            PlayerPrefs.SetString("backwardKey", ControlManager.Instance.backward.ToString()); //save new key to PlayerPrefs

            break;

        case "left":

            ControlManager.Instance.left = newKey; //set left to new keycode

            buttonText.text = ControlManager.Instance.left.ToString(); //set button text to new key

            PlayerPrefs.SetString("leftKey", ControlManager.Instance.left.ToString()); //save new key to playerprefs

            break;

        case "right":

            ControlManager.Instance.right = newKey; //set right to new keycode

            buttonText.text = ControlManager.Instance.right.ToString(); //set button text to new key

            PlayerPrefs.SetString("rightKey", ControlManager.Instance.right.ToString()); //save new key to playerprefs

            break;

        case "jump":

            ControlManager.Instance.jump = newKey; //set jump to new keycode

            buttonText.text = ControlManager.Instance.jump.ToString(); //set button text to new key

            PlayerPrefs.SetString("jumpKey", ControlManager.Instance.jump.ToString()); //save new key to playerprefs

            break;
        
        case "pickup":

            ControlManager.Instance.pickup = newKey; //Set pickup to new keycode

            buttonText.text = ControlManager.Instance.pickup.ToString(); //Set button text to new key

            PlayerPrefs.SetString("pickupKey", ControlManager.Instance.pickup.ToString()); //save new key to PlayerPrefs

            break;

        case "flip":

            ControlManager.Instance.flip = newKey; //Set flip to new keycode

            buttonText.text = ControlManager.Instance.flip.ToString(); //Set button text to new key

            PlayerPrefs.SetString("flipKey", ControlManager.Instance.flip.ToString()); //save new key to PlayerPrefs

            break;

        case "select":

            ControlManager.Instance.select = newKey; //Set select to new keycode

            buttonText.text = ControlManager.Instance.select.ToString(); //Set button text to new key

            PlayerPrefs.SetString("selectKey", ControlManager.Instance.select.ToString()); //save new key to PlayerPrefs

            break;

        case "pause":

            ControlManager.Instance.pause = newKey; //Set pause to new keycode

            buttonText.text = ControlManager.Instance.pause.ToString(); //Set button text to new key

            PlayerPrefs.SetString("pauseKey", ControlManager.Instance.pause.ToString()); //save new key to PlayerPrefs

            break;

        case "submit":

            ControlManager.Instance.submit = newKey; //Set submit to new keycode

            buttonText.text = ControlManager.Instance.submit.ToString(); //Set button text to new key

            PlayerPrefs.SetString("submitKey", ControlManager.Instance.submit.ToString()); //save new key to PlayerPrefs

            break;



        }

 

        yield return null;

    }


    


}
