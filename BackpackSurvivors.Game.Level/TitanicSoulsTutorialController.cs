using System;
using System.Collections;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Level;

public class TitanicSoulsTutorialController : MonoBehaviour
{
	[SerializeField]
	private GameObject _canvas;

	[SerializeField]
	private Image _imageContainerOverlays;

	[SerializeField]
	private AudioClip _informationPointAudioclip;

	[SerializeField]
	private Sprite _unlockListOverlay;

	[SerializeField]
	private Sprite _unlockDescriptionOverlay;

	[SerializeField]
	private Sprite _unlockButtonOverlay;

	[SerializeField]
	private Transform _unlockListTextPosition;

	[SerializeField]
	private Transform _unlockDescriptionTextPosition;

	[SerializeField]
	private Transform _unlockButtonTextPosition;

	[SerializeField]
	private TextMeshProUGUI _explainationText;

	[SerializeField]
	private Image _explainationCircle;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _welcomeAudio;

	[SerializeField]
	private AudioClip _unlockListAudio;

	[SerializeField]
	private AudioClip _unlockDescriptionAudio;

	[SerializeField]
	private AudioClip _unlockButtonAudion;

	private const float _delayBetweenInformationSteps = 6f;

	private TutorialController _tutorialController;

	private int _currentTutorialStep;

	private int _currentlyRunningTutorialStep = -1;

	private int _maxTutorialStep = 3;

	internal float TutorialDuration => (float)(_maxTutorialStep + 1) * 6f;

	public void StartTutorial(TutorialController tutorialController)
	{
		_tutorialController = tutorialController;
		SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
		_canvas.SetActive(value: true);
		StartCoroutine(RunTutorialAsync());
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
		if (_tutorialController != null)
		{
			StartCoroutine(_tutorialController.FinishTitanicSoulTutorial());
		}
		StopAllCoroutines();
	}

	private void FillExplainationCircle(float val)
	{
		_explainationCircle.fillAmount = val;
	}

	private void RunCircle(float delayOverride = 6f)
	{
		_explainationCircle.fillAmount = 0f;
		LeanTween.cancel(_explainationCircle.gameObject);
		LeanTween.value(_explainationCircle.gameObject, FillExplainationCircle, 0f, 1f, delayOverride).setIgnoreTimeScale(useUnScaledTime: true);
	}

	internal void ExitTutorial()
	{
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
		LeanTween.cancel(_explainationCircle.gameObject);
		_canvas.SetActive(value: false);
		_imageContainerOverlays.gameObject.SetActive(value: false);
	}

	private void Step0_Start()
	{
		_explainationText.SetText(Constants.Tutorial.TitanicSouls.Explanation0);
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 1f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_welcomeAudio, 1f);
		RunCircle();
	}

	private void Step1_List()
	{
		_imageContainerOverlays.sprite = _unlockListOverlay;
		_explainationText.SetText(Constants.Tutorial.TitanicSouls.Explanation1);
		RunCircle();
		_explainationText.transform.position = _unlockListTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 1f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_unlockListAudio, 1f);
	}

	private void Step2_Description()
	{
		_imageContainerOverlays.sprite = _unlockDescriptionOverlay;
		_explainationText.SetText(Constants.Tutorial.TitanicSouls.Explanation2);
		RunCircle();
		_explainationText.transform.position = _unlockDescriptionTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 1f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_unlockDescriptionAudio, 1f);
	}

	private void Step3_Button()
	{
		_imageContainerOverlays.sprite = _unlockButtonOverlay;
		_explainationText.SetText(Constants.Tutorial.TitanicSouls.Explanation3);
		RunCircle();
		_explainationText.transform.position = _unlockButtonTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 1f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_unlockButtonAudion, 1f);
	}

	public void GoToNextTutorialStep()
	{
		LeanTween.cancel(_explainationCircle.gameObject);
		StopAllCoroutines();
		_currentTutorialStep++;
		StartCoroutine(RunTutorialAsync());
	}

	private void RunCurrentTutorialStep()
	{
		if (_currentTutorialStep == 0)
		{
			Step0_Start();
		}
		if (_currentTutorialStep == 1)
		{
			Step1_List();
		}
		if (_currentTutorialStep == 2)
		{
			Step2_Description();
		}
		if (_currentTutorialStep == 3)
		{
			Step3_Button();
		}
	}

	private IEnumerator RunTutorialAsync()
	{
		while (_currentTutorialStep <= _maxTutorialStep)
		{
			if (_currentlyRunningTutorialStep != _currentTutorialStep)
			{
				_currentlyRunningTutorialStep = _currentTutorialStep;
				RunCurrentTutorialStep();
				yield return new WaitForSecondsRealtime(6f);
				_currentTutorialStep++;
			}
			else
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		StartCoroutine(_tutorialController.FinishTitanicSoulTutorial());
	}
}
