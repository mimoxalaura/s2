using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.UI.MainMenu;
using UnityEngine;

namespace BackpackSurvivors.Game.MainMenu;

internal class NoticeController : MonoBehaviour
{
	[SerializeField]
	private int _currentNoticeNumber;

	[SerializeField]
	private WorkInProgressMessageUI _workInProgressMessageUI;

	private void Start()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SettingsController>.Instance, SetNoticeVisibility);
	}

	private void SetNoticeVisibility()
	{
		if (_currentNoticeNumber > SingletonController<SettingsController>.Instance.GlobalSettingsController.MaxNoticeNumberSeen)
		{
			_workInProgressMessageUI.ShowUI();
			SingletonController<SettingsController>.Instance.GlobalSettingsController.MaxNoticeNumberSeen = _currentNoticeNumber;
			SingletonController<SettingsController>.Instance.SaveSettings();
		}
	}
}
