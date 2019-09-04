using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NarrationController : MonoBehaviour
{
	private GameObject _narrationContainer;
	private HintController _hintController;
	private Text _narrationText;
	private Animator _narrationAnimator;
	private Image _narrationBackground;
	private Image _narrationContinue;
	private State _state;
	private NarrationSettings _currentSettings;
	private int _narrationIndex = 0;
	private int _narrationLettersIndex = 0;
	private Coroutine _currentRoutine;
	private INarrationActor _actor;
	private TextAsset _textAsset;

	[SerializeField] private NarrationSettings defaultSettings;

	private static NarrationController _instance;

	private enum State
	{
		Inactive,
		AnimatingOpen,
		Drawing,
		Waiting,
		AnimatingClose
	}

	// Use this for initialization
	void Start ()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Debug.LogError("There is more than one Narration Controller! Destroying this one");
			Destroy(this);
		}
		
		_narrationContainer = Toolbox.Instance.mainCanvas.transform.Find("TextNarration").gameObject;
		_narrationText = _narrationContainer.GetComponentInChildren<Text>();
		_narrationText.text = "";
		_narrationAnimator = _narrationContainer.GetComponent<Animator>();
		_narrationContinue = _narrationContainer.transform.Find("Continue").GetComponent<Image>();
		_narrationContinue.gameObject.SetActive(false);
		_narrationBackground = _narrationContainer.transform.Find("Background").GetComponent<Image>();
		_hintController = Toolbox.Instance.mainCanvas.transform.Find("ControlHint").GetComponent<HintController>();
		
		ClearNarration();
	}
	
	public static void CancelNarration()
	{
		_instance.ClearNarration();
	}

	private void ClearNarration()
	{
		_hintController.Clear();
		
		_narrationText.text = null;
		if (_narrationContinue) _narrationContinue.gameObject.SetActive(false);
		_narrationIndex = 0;
		_narrationLettersIndex = 0;
		_narrationContainer.SetActive(false);
		
/*
		Debug.LogError("narration cancelled");
*/

		_state = State.Inactive;
		
		if (_currentRoutine != null)
		{
			StopCoroutine(_currentRoutine);
			_currentRoutine = null;
		}

		if (_actor != null)
		{
			Debug.LogError("narration deactivate");
			_actor.OnNarrationDeactivate();
		}

		_actor = null;
		
		if (_currentSettings)
		{
			_currentSettings.actor = null;
			_currentSettings = null;
		}	
	}

	private IEnumerator DrawText()
	{
		string[] paragraphs = _textAsset.ToString()
			.Split(new[] {"****\n", "****\r\n", "****"}, StringSplitOptions.None);

		if (! _currentSettings.letterByLetter)
		{
			_narrationContinue.enabled = true;

			while (_narrationIndex < paragraphs.Length)
			{
				_narrationText.text = paragraphs[_narrationIndex];
				
				_state = State.Waiting;

				while (!GetButtonNextDown())
				{
					_currentSettings.OnNext();
					
					yield return null;
				}

				_narrationIndex++;
			}

		}
		else
		{
			while (_narrationIndex < paragraphs.Length)
			{
				_state = State.Drawing;

				_narrationText.text = "";

				var paragraphLetters = paragraphs[_narrationIndex].ToCharArray();
				
				_narrationLettersIndex = 0;

				while (_narrationLettersIndex < paragraphLetters.Length)
				{
					yield return null;

					if (GetButtonNextDown())
					{
						_narrationText.text = paragraphs[_narrationIndex];
						
						yield return null;

						break;
					}
						
					_narrationText.text += paragraphLetters[_narrationLettersIndex];

					_narrationLettersIndex++;
				}

				_narrationContinue.gameObject.SetActive(true);

				_state = State.Waiting;
				
				while (!GetButtonNextDown())
				{	
					yield return null;
				}
				
                _currentSettings.OnNext();

				_narrationContinue.gameObject.SetActive(false);

				_narrationIndex++;
			}
			
		}

		_currentSettings.OnDeactivate();

		if (_currentSettings.animatedClose)
		{
			//implement later
			ClearNarration();
		}
		else
		{
			ClearNarration();
		}

	}

	private void ApplyNewSettings()
	{
		var decoration = _narrationBackground.transform.GetChild(0).GetComponent<Image>();

		decoration.color = _currentSettings.decorationColor;
		
		if (_currentSettings.background)
        {
            _narrationBackground.sprite = _currentSettings.background;
        }
			
        else
        {
            _narrationBackground.sprite = null;
        }


        _narrationBackground.color = _currentSettings.backgroundRectangleColor;

		if (_currentSettings.textFace)
			_narrationText.font = _currentSettings.textFace;

		_narrationText.color = _currentSettings.textColor;
	}

	private void StartNarration(NarrationSettings newSettings, INarrationActor actor, TextAsset textAsset)
	{
		_currentSettings = newSettings == null ? defaultSettings : newSettings;
		
		ApplyNewSettings();

		_actor = actor;

		_textAsset = textAsset;

		_currentSettings.actor = _actor;

		_currentSettings.OnActivate();
		
		_narrationContainer.SetActive(true);

		_narrationText.text = null;
		
		_narrationBackground.gameObject.SetActive(true);

		if (_currentSettings.animatedOpen)
		{
			_state = State.AnimatingOpen;
			
			_narrationAnimator.Play(_currentSettings.openAnimation);
		}
		else
		{
			_currentRoutine = StartCoroutine(DrawText());
		}
	}

	private bool GetButtonNextDown()
	{
		return ControlManager.Instance.GetButtonDown("Select") || 
		       ControlManager.Instance.GetButtonDown("Submit") || 
		       ControlManager.Instance.GetButtonDown("Switch");
	}
	
	public static void TriggerNarration(NarrationSettings newSettings, INarrationActor actor, TextAsset textAsset)
	{
		if (_instance._state == State.Inactive)
			_instance.StartNarration(newSettings, actor, textAsset);
	}

	public static void OpenAnimationDone()
	{
		_instance.StartCoroutine(_instance.DrawText());
	}

	public static void TriggerHint(TextAsset hintText, Sprite hintImage)
	{
		_instance._hintController.TriggerHint(hintText, hintImage);
	}

	public static void ClearHint()
	{
		_instance._hintController.Clear();
	}
}
