using System;
using BackpackSurvivors.Game.Analytics;
using BackpackSurvivors.Game.Analytics.Events;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Video;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.Feedback;

[RequireComponent(typeof(CameraEnabler))]
public class FeedbackUI : ModalUI
{
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private GameObject _backdrop;

	private bool _isOpen;

	private CameraEnabler _cameraEnabler;

	private void Awake()
	{
		_cameraEnabler = GetComponent<CameraEnabler>();
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		CloseModalUI();
	}

	public void ButtonCancelPressed()
	{
		CloseModalUI();
	}

	private void CloseModalUI()
	{
		_isOpen = false;
		_backdrop.SetActive(value: false);
		CloseUI();
	}

	public override void AfterCloseUI()
	{
		_cameraEnabler.SetCamerasEnabled(enabled: false);
	}

	public void ButtonSendPressed()
	{
		FeedbackEvent eventToRecord = new FeedbackEvent
		{
			FeedbackText = _text.text
		};
		SingletonController<AnalyticsController>.Instance.RecordEvent(eventToRecord);
		_text.SetText(string.Empty);
		CloseModalUI();
	}

	public void OpenFeedbackUI()
	{
		if (!_isOpen)
		{
			_isOpen = true;
			OpenUI();
		}
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		_cameraEnabler.SetCamerasEnabled(enabled: true);
		base.OpenUI(openDirection);
		_backdrop.SetActive(value: true);
		SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.CloseUI(openDirection);
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
	}
}
