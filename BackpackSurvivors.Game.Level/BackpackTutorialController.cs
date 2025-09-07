using System;
using System.Collections;
using BackpackSurvivors.Game.Analytics;
using BackpackSurvivors.Game.Analytics.Events;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Level;

public class BackpackTutorialController : MonoBehaviour
{
	[SerializeField]
	private GameObject _canvas;

	[SerializeField]
	private Image _imageContainerOverlays;

	[SerializeField]
	private AudioClip _informationPointAudioclip;

	[SerializeField]
	private GameObject _continueButton;

	[Header("Overlay")]
	[SerializeField]
	private Sprite _backpackOverlay;

	[SerializeField]
	private Sprite _coinsOverlay;

	[SerializeField]
	private Sprite _continueOverlay;

	[SerializeField]
	private Sprite _rerollOverlay;

	[SerializeField]
	private Sprite _shopOverlay;

	[SerializeField]
	private Sprite _statsOverlay;

	[SerializeField]
	private Sprite _storageOverlay;

	[SerializeField]
	private Sprite _vendorOverlay;

	[Header("Position")]
	[SerializeField]
	private Transform _backpackOverlayTextPosition;

	[SerializeField]
	private Transform _coinsOverlayTextPosition;

	[SerializeField]
	private Transform _continueOverlayTextPosition;

	[SerializeField]
	private Transform _rerollOverlayTextPosition;

	[SerializeField]
	private Transform _shopOverlayTextPosition;

	[SerializeField]
	private Transform _statsOverlayTextPosition;

	[SerializeField]
	private Transform _storageOverlayTextPosition;

	[SerializeField]
	private Transform _vendorOverlayTextPosition;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _welcomeAudio;

	[SerializeField]
	private AudioClip _backpackOverlayAudio;

	[SerializeField]
	private AudioClip _coinsOverlayAudio;

	[SerializeField]
	private AudioClip _continueOverlayAudio;

	[SerializeField]
	private AudioClip _rerollOverlayAudio;

	[SerializeField]
	private AudioClip _shopOverlayAudio;

	[SerializeField]
	private AudioClip _statsOverlayAudio;

	[SerializeField]
	private AudioClip _storageOverlayAudio;

	[SerializeField]
	private AudioClip _vendorOverlayAudio;

	[SerializeField]
	private TextMeshProUGUI _explainationText;

	[SerializeField]
	private Image _explainationCircle;

	private const float _delayBetweenInformationSteps = 8f;

	private float _tutorialStartTime;

	private bool _tutorialStarted;

	private float _previousMusicVolume;

	private int _currentTutorialStep;

	private int _currentlyRunningTutorialStep = -1;

	private int _maxTutorialStep = 8;

	public void StartTutorial()
	{
		if (!_tutorialStarted)
		{
			_tutorialStarted = true;
			_tutorialStartTime = Time.realtimeSinceStartup;
			_previousMusicVolume = SingletonController<AudioController>.Instance.GetVolume(Enums.AudioType.Music);
			if (_previousMusicVolume > 0f)
			{
				float volume = _previousMusicVolume / 2f;
				SingletonController<AudioController>.Instance.SetVolume(Enums.AudioType.Music, volume);
			}
			SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
			SingletonController<InputController>.Instance.OnAcceptHandler += InputController_OnAcceptHandler;
			SingletonController<InputController>.Instance.CanSpecial1 = false;
			_canvas.SetActive(value: true);
			StartCoroutine(RunTutorialAsync());
		}
	}

	private void InputController_OnAcceptHandler(object sender, EventArgs e)
	{
		GoToNextTutorialStep();
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		ExitTutorialButtonPressed();
	}

	public void ExitTutorialButtonPressed()
	{
		LogTutorialSkipped();
		ExitTutorial();
	}

	private void LogTutorialSkipped()
	{
		float skippedTutorialAfterTime = Time.realtimeSinceStartup - _tutorialStartTime;
		BackpackTutorialSkippedEvent eventToRecord = new BackpackTutorialSkippedEvent
		{
			SkippedTutorialAfterTime = skippedTutorialAfterTime
		};
		SingletonController<AnalyticsController>.Instance.RecordEvent(eventToRecord);
	}

	private void FillExplainationCircle(float val)
	{
		_explainationCircle.fillAmount = val;
	}

	private void ShowExplanationText(float delayOverride = 8f)
	{
		_explainationCircle.fillAmount = 0f;
		LeanTween.cancel(_explainationCircle.gameObject);
		LeanTween.value(_explainationCircle.gameObject, FillExplainationCircle, 0f, 1f, delayOverride).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void ExitTutorial()
	{
		SingletonController<InputController>.Instance.CanSpecial1 = true;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.SetVolume(Enums.AudioType.Music, _previousMusicVolume);
		SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownBackpackTutorial = true;
		SingletonController<SaveGameController>.Instance.SaveProgression();
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnAcceptHandler -= InputController_OnAcceptHandler;
		LeanTween.cancel(_explainationCircle.gameObject);
		StopAllCoroutines();
		_canvas.SetActive(value: false);
		_imageContainerOverlays.gameObject.SetActive(value: false);
		StartCoroutine(DelayedTutorialRunningStop());
	}

	private IEnumerator DelayedTutorialRunningStop()
	{
		yield return new WaitForSecondsRealtime(1f);
		SingletonController<TutorialController>.Instance.IsRunningTutorial = false;
	}

	private void Step0_Start()
	{
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation0);
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_welcomeAudio, 1f);
		ShowExplanationText(4f);
	}

	private void Step1_Backpack()
	{
		_imageContainerOverlays.sprite = _backpackOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation1);
		ShowExplanationText();
		_explainationText.transform.position = _backpackOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_backpackOverlayAudio, 1f);
	}

	private void Step2_Shop()
	{
		_imageContainerOverlays.sprite = _shopOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation2);
		ShowExplanationText();
		_explainationText.transform.position = _shopOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_shopOverlayAudio, 1f);
	}

	private void Step3_Coins()
	{
		_imageContainerOverlays.sprite = _coinsOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation3);
		ShowExplanationText();
		_explainationText.transform.position = _coinsOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_coinsOverlayAudio, 1f);
	}

	private void Step4_Reroll()
	{
		_imageContainerOverlays.sprite = _rerollOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation4);
		ShowExplanationText();
		_explainationText.transform.position = _rerollOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_rerollOverlayAudio, 1f);
	}

	private void Step5_Storage()
	{
		_imageContainerOverlays.sprite = _storageOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation5);
		ShowExplanationText();
		_explainationText.transform.position = _storageOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_storageOverlayAudio, 1f);
	}

	private void Step6_Vendor()
	{
		_imageContainerOverlays.sprite = _vendorOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation6);
		ShowExplanationText();
		_explainationText.transform.position = _vendorOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_vendorOverlayAudio, 1f);
	}

	private void Step7_Stats()
	{
		_imageContainerOverlays.sprite = _statsOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation7);
		ShowExplanationText();
		_explainationText.transform.position = _statsOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_statsOverlayAudio, 1f);
	}

	private void Step8_Continue()
	{
		_imageContainerOverlays.sprite = _continueOverlay;
		_explainationText.SetText(Constants.Tutorial.Backpack.Explanation8);
		ShowExplanationText();
		_explainationText.transform.position = _continueOverlayTextPosition.position;
		SingletonController<AudioController>.Instance.StopAllSFX();
		SingletonController<AudioController>.Instance.PlaySFXClip(_informationPointAudioclip, 0.5f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_continueOverlayAudio, 1f);
	}

	public void GoToNextTutorialStep()
	{
		LeanTween.cancel(_explainationCircle.gameObject);
		StopAllCoroutines();
		_currentTutorialStep++;
		StartCoroutine(RunTutorialAsync(skippedStep: true));
	}

	private void RunCurrentTutorialStep()
	{
		if (_currentTutorialStep == 0)
		{
			Step0_Start();
		}
		if (_currentTutorialStep == 1)
		{
			Step1_Backpack();
		}
		if (_currentTutorialStep == 2)
		{
			Step2_Shop();
		}
		if (_currentTutorialStep == 3)
		{
			Step3_Coins();
		}
		if (_currentTutorialStep == 4)
		{
			Step4_Reroll();
		}
		if (_currentTutorialStep == 5)
		{
			Step5_Storage();
		}
		if (_currentTutorialStep == 6)
		{
			Step6_Vendor();
		}
		if (_currentTutorialStep == 7)
		{
			Step7_Stats();
		}
		if (_currentTutorialStep == 8)
		{
			Step8_Continue();
		}
	}

	private IEnumerator RunTutorialAsync(bool skippedStep = false)
	{
		SingletonController<TutorialController>.Instance.IsRunningTutorial = true;
		if (!skippedStep)
		{
			yield return new WaitForSecondsRealtime(1.5f);
		}
		while (_currentTutorialStep <= _maxTutorialStep)
		{
			if (_currentlyRunningTutorialStep != _currentTutorialStep)
			{
				_currentlyRunningTutorialStep = _currentTutorialStep;
				RunCurrentTutorialStep();
				if (_currentlyRunningTutorialStep == 0)
				{
					yield return new WaitForSecondsRealtime(3f);
				}
				else
				{
					yield return new WaitForSecondsRealtime(8f);
				}
				_currentTutorialStep++;
			}
			else
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}
		ExitTutorial();
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnUseHandler -= InputController_OnAcceptHandler;
	}
}
