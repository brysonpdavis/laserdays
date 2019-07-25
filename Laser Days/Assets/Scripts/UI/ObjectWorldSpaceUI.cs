using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectWorldSpaceUI : MonoBehaviour
{
	private GameObject player;

	private Camera mainCamera;

	private float playerDistance;

	public float activeDistance = 10;

	public Sprite joystickSprite;

	public Sprite keyboardSprite;
	
	public float refreshTime = 1f;

	// private GameObject textAndImage;

	private GameObject joystickImage;

	private GameObject keyboardImage;

	private State dState;
	
	private enum State
	{
		Waiting,
		Activating,
		Active,
		Done
	};
	
	void Start ()
	{
		player = Toolbox.Instance.GetPlayer();
		mainCamera = Camera.main;
		//textAndImage = transform.GetChild(0).gameObject;
		dState = State.Waiting;
		keyboardImage = transform.Find("Keyboard").gameObject;
		joystickImage = transform.Find("Controller").gameObject;

		if (keyboardSprite && keyboardImage.GetComponent<Image>())
			keyboardImage.GetComponent<Image>().sprite = keyboardSprite;

		if (joystickSprite && joystickImage.GetComponent<Image>())
			joystickImage.GetComponent<Image>().sprite = joystickSprite;
		
		
		DisableTextAndImage();
		InvokeRepeating("SetPlayerDistance", 0, refreshTime);
	}
	
	void Update ()
	{
		UpdateStates();
		UpdateContent();
		if (dState == State.Active) 
			LookAtCamera();
	}
	
	void LookAtCamera()
	{
		if (player)
		{
			transform.LookAt(mainCamera.transform);
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x < 180 ? transform.eulerAngles.x / 3 : (360 - ((360 - transform.eulerAngles.x) / 3)), 
													transform.eulerAngles.y, 
													transform.eulerAngles.z);
		}
	}
	
	void SetPlayerDistance()
	{
		playerDistance = Vector3.Distance(player.transform.position, transform.position);
	}

	void DisableTextAndImage()
	{
		//if (textAndImage.activeSelf)
			//textAndImage.SetActive(false);
		
		if (keyboardImage)
			keyboardImage.SetActive(false);
		
		if (joystickImage)
			joystickImage.SetActive(false);
	}

	void EnableImageAndIcon()
	{
		//if (!textAndImage.activeSelf)
		//{
		//	textAndImage.SetActive(true);
		//}
		
		if (ControlManager.GetControllerState() == ControlManager.ControllerState.JoystickPS4)
		{
			ActivateControllerImage();
		}

		if (ControlManager.GetControllerState() == ControlManager.ControllerState.KeyboardAndMouse)
		{
			ActivateKeyboardImage();
		}
		
	}

	void UpdateContent()
	{
		switch (dState)
		{
			case State.Done:
				
				gameObject.SetActive(false);

				break;
			
			case State.Activating:

				dState = State.Active;
				
				EnableImageAndIcon();

				break;
			
			case State.Active:
				
				if (ControlManager.GetControllerState() == ControlManager.ControllerState.JoystickPS4)
				{
					ActivateControllerImage();
				}

				if (ControlManager.GetControllerState() == ControlManager.ControllerState.KeyboardAndMouse)
				{
					ActivateKeyboardImage();
				}

				break;
			
			case State.Waiting:
				
				break;
			
			default:
				
				break;
		}
	}
	
	void UpdateStates()
	{

		switch (dState)
		{
			case State.Waiting:
				
				if (playerDistance < activeDistance)
				{
					dState = State.Activating;
				}

				break;
			
			case State.Activating:

				break;
			
			case State.Active:
				
				break;
			
			case State.Done:
				
				gameObject.SetActive(false);

				break;
			
			default:

				break;
		}	
	}

	// call this from another script to disable text box
	// call from TakeActionOnAction script
	public void TurnOff()
	{
		dState = State.Done;
	}

	private void ActivateControllerImage()
	{
		joystickImage.SetActive(true);
		keyboardImage.SetActive(false);
	}

	private void ActivateKeyboardImage()
	{
		joystickImage.SetActive(false);
		keyboardImage.SetActive(true);
    }

    private void ActiveImageSetTransition(GameObject image)
    {
        Transition trans = image.GetComponent<Transition>();
        if (trans)
        {
            trans.StopAllCoroutines();
            if(Toolbox.Instance.PlayerInReal())
            {
                trans.SetStart(0f);
            }
            else
            {
                trans.SetStart(1f);
            }

        }
    }
}
