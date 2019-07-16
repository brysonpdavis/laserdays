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

	public Image joystickSprite;

	public Image keyboardSprite;
	
	public float refreshTime = 1f;

	private GameObject textAndImage;

	private GameObject joystickImage;

	private GameObject keyboardImage;

	private State dState;

	private static ControlState cState;
	
	private enum State
	{
		Waiting,
		Activating,
		Active,
		Done
	};

	private enum ControlState
	{
		Keyboard,
		Controller
	}
	
	void Start ()
	{
		cState = ControlState.Controller;
		
		player = Toolbox.Instance.GetPlayer();
		mainCamera = Camera.main;
		textAndImage = transform.GetChild(0).gameObject;
		dState = State.Waiting;
		keyboardImage = textAndImage.transform.Find("Keyboard").gameObject;
		joystickImage = textAndImage.transform.Find("Controller").gameObject;
		DisableTextAndImage();
		InvokeRepeating("SetPlayerDistance", 0, refreshTime);
	}
	
	void Update ()
	{
		UpdateStates();
		UpdateText();
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
		if (textAndImage.activeSelf)
			textAndImage.SetActive(false);
		
		if (keyboardImage)
			keyboardImage.SetActive(false);
		
		if (joystickImage)
			joystickImage.SetActive(false);
	}

	void EnableImageAndIcon()
	{
		if (!textAndImage.activeSelf)
		{
			textAndImage.SetActive(true);
		}
		
		if (cState == ControlState.Controller)
		{
			joystickImage.SetActive(true);
		}

		if (cState == ControlState.Keyboard)
		{
			keyboardImage.SetActive(true);
		}

	}

	void UpdateText()
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
}
