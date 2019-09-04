using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUI : MonoBehaviour, IActivatable, IDeactivatable
{
	private GameObject player;

	private Camera mainCamera;

	private float playerDistance;

	public float switchDistance = 5;

	public float activeDistance = 10;

	public TextAsset textFile;

	private string[] fileText;

	private Text textComponent;
	
	private int originalFontSize;
	
	public float refreshTime = 1f;

	private GameObject textAndImage;

	private int iLetter;

	private int iParagraph;

	private string currentText;
	
	private DistanceState dState;

	private SubState sState;

	private enum DistanceState
	{
		Inactive,
		Middle,
		Active
	};

	private enum SubState
	{
		Writing,
		WaitingForReactivation,
		WaitingForNext,
	};

	void Start () {
		player = Toolbox.Instance.GetPlayer();
		mainCamera = Camera.main;
		textComponent = GetComponentInChildren<Text>();
		originalFontSize = textComponent.fontSize;
		textAndImage = transform.GetChild(0).gameObject;
		fileText = textFile.text.Split(new string[] {"****\n", "****\r\n", "****"}, StringSplitOptions.None);
		iLetter = 0;
		iParagraph = 0;
		dState = DistanceState.Inactive;
		sState = SubState.Writing;
        DisableTextAndImage();
		InvokeRepeating("SetPlayerDistance", 0, refreshTime);
	}
	
	void Update ()
	{
		UpdateStates();
		UpdateText();
		if (dState != DistanceState.Inactive) 
			LookAtCamera();
	}

	void SetFarText()
	{
		textComponent.text = ". . .";
		textComponent.fontSize = (int) (originalFontSize * 1.5);
	}

	void UnsetFarText()
	{
		textComponent.fontSize = originalFontSize;
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
	}

	void EnableTextAndImage()
	{
		if (!textAndImage.activeSelf)
			textAndImage.SetActive(true);
	}

	void UpdateText()
	{
		switch (dState)
		{
			case DistanceState.Inactive:
				
				break;
			
			case DistanceState.Middle:
				
				SetFarText();
				
				break;
			
			case DistanceState.Active:
				
				UnsetFarText();

				switch (sState)
				{
					case (SubState.WaitingForNext):

						textComponent.text = fileText[iParagraph];

						break;
					
					case (SubState.Writing):
						
						textComponent.text = fileText[iParagraph].Substring(0, iLetter);
						
						break;
					
					case (SubState.WaitingForReactivation):

						textComponent.text = "";

						break;
				}

				break;
			
			default:
				break;
		}
	}

	void RestartDialogue()
	{
		sState = SubState.Writing;
		iLetter = 0;
		iParagraph = 0;
		EnableTextAndImage();
	}

	void NextDialogue()
	{
		iLetter = 0;
		iParagraph++;
		
		if (iParagraph < fileText.Length)
		{
			sState = SubState.Writing;
		}
		else
		{
			sState = SubState.WaitingForReactivation;
			DisableTextAndImage();
		}
	}

	void UpdateStates()
	{
		
		if (playerDistance < activeDistance)
		{
			if (dState == DistanceState.Inactive && sState != SubState.WaitingForReactivation)
				EnableTextAndImage();

			if (playerDistance < switchDistance)
			{
				dState = DistanceState.Active;
			}
			else
			{
				dState = DistanceState.Middle;
			}
		}
		else
		{
			if (dState != DistanceState.Inactive)
				DisableTextAndImage();
			
			dState = DistanceState.Inactive;
		}

		if (dState == DistanceState.Active)
		{
			switch (sState)
			{
				case SubState.Writing:
					if (iLetter >= fileText[iParagraph].Length)
					{
						sState = SubState.WaitingForNext;
					}
					else if (ControlManager.Instance.GetButtonDown("Submit"))
					{
						iLetter = fileText[iParagraph].Length;
						sState = SubState.WaitingForNext;
					}
					else
						iLetter++;

					break;

				case SubState.WaitingForNext:

					if (ControlManager.Instance.GetButtonDown("Submit"))
						NextDialogue();

					break;

				case SubState.WaitingForReactivation:

					if (ControlManager.Instance.GetButtonDown("Submit"))
						RestartDialogue();

					break;
				default:
					break;
			}
		}	
	}

	public void Activate()
	{
		EnableTextAndImage();
	}

	public void Deactivate()
	{
		DisableTextAndImage();
	}
}
