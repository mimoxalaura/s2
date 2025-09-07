using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Settings.Events;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class DashFeedbackUINew : MonoBehaviour
{
	[SerializeField]
	private DashElementUINew _dashElement;

	[SerializeField]
	private Transform _dashContainer;

	[SerializeField]
	private GameObject _keyboardTip;

	[SerializeField]
	private GameObject _controllerTip;

	private bool _dashTipRemoved;

	private List<DashElementUINew> _activeDashElements;

	private Player _player;

	private void Start()
	{
		_player = SingletonController<GameController>.Instance.Player;
		_player.OnDashesCountSet += Player_OnDashesCountSet;
		_player.OnDashed += Player_OnDashed;
		_player.OnDashCooldownUpdated += Player_OnDashCooldownUpdated;
		_player.OnDashCountChanged += Player_OnDashCountChanged;
		_player.OnDashReady += Player_OnDashReady;
		SingletonController<InputController>.Instance.OnControlSchemeChanged += InputController_OnControlSchemeChanged;
		_dashContainer.gameObject.SetActive(SingletonController<SettingsController>.Instance.GameplaySettingsController.DashVisual == Enums.DashVisual.InUI || SingletonController<SettingsController>.Instance.GameplaySettingsController.DashVisual == Enums.DashVisual.Both);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnDashVisualSettingChanged += GameplaySettingsController_OnDashVisualSettingChanged;
		ShowDashTip();
	}

	private void GameplaySettingsController_OnDashVisualSettingChanged(object sender, DashVisualSettingsChangedEventArgs e)
	{
		_dashContainer.gameObject.SetActive(e.DashVisual == Enums.DashVisual.InUI || e.DashVisual == Enums.DashVisual.Both);
	}

	private void Player_OnDashCountChanged(object sender, DashCooldownEventArgs e)
	{
		RefreshUI();
	}

	private void Player_OnDashCooldownUpdated(object sender, DashCooldownEventArgs e)
	{
		if (_activeDashElements.FirstOrDefault((DashElementUINew x) => x.Index == e.RemainingDashes) != null)
		{
			_activeDashElements[e.RemainingDashes].SetProgress(e.CurrentCooldown, e.TotalCooldown);
		}
	}

	private void Player_OnDashReady(object sender, DashCooldownEventArgs e)
	{
		for (int i = 0; i < e.TotalDashes; i++)
		{
			bool isReady = i < e.RemainingDashes;
			_activeDashElements[i].ToggleReadyState(isReady);
		}
	}

	private void Player_OnDashed(object sender, DashCooldownEventArgs e)
	{
		RemoveDashTipIfNeeded();
		UpdateReadyDashCount(e);
	}

	private void UpdateReadyDashCount(DashCooldownEventArgs e)
	{
		for (int i = 0; i < e.TotalDashes; i++)
		{
			bool flag = i < e.RemainingDashes;
			_activeDashElements[i].ToggleReadyState(flag);
			if (!flag)
			{
				_activeDashElements[i].SetProgress(0f);
			}
		}
	}

	private void RemoveDashTipIfNeeded()
	{
		if (!_dashTipRemoved)
		{
			_dashTipRemoved = true;
			UnbindInputControllerEvent();
			_keyboardTip.SetActive(value: false);
			_controllerTip.SetActive(value: false);
		}
	}

	private void Player_OnDashesCountSet(object sender, EventArgs e)
	{
		RefreshUI();
	}

	private void RefreshUI()
	{
		_activeDashElements = new List<DashElementUINew>();
		int totalDashCount = SingletonController<GameController>.Instance.Player.TotalDashCount;
		for (int num = _dashContainer.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_dashContainer.transform.GetChild(num).gameObject);
		}
		for (int i = 0; i < totalDashCount; i++)
		{
			DashElementUINew dashElementUINew = UnityEngine.Object.Instantiate(_dashElement, _dashContainer);
			dashElementUINew.Index = i;
			dashElementUINew.IsReady = true;
			dashElementUINew.ToggleReadyState(isReady: true);
			_activeDashElements.Add(dashElementUINew);
		}
	}

	private void InputController_OnControlSchemeChanged(object sender, ControlSchemeChangedEventArgs e)
	{
		SetControlTipsActive(e.CurrentControlSchemeName);
	}

	private void ShowDashTip()
	{
		if (!_dashTipRemoved)
		{
			if (SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard)
			{
				ShowKeyboardTips();
			}
			else
			{
				ShowControllerTips();
			}
		}
	}

	private void SetControlTipsActive(string currentControlSchemeName)
	{
		if (!_dashTipRemoved)
		{
			if (currentControlSchemeName.Equals("Keyboard&Mouse"))
			{
				ShowKeyboardTips();
			}
			else if (currentControlSchemeName.Equals("Gamepad"))
			{
				ShowControllerTips();
			}
		}
	}

	private void ShowControllerTips()
	{
		_keyboardTip.SetActive(value: false);
		_controllerTip.SetActive(value: true);
	}

	private void ShowKeyboardTips()
	{
		if (!(this == null))
		{
			if (_keyboardTip != null)
			{
				_keyboardTip.SetActive(value: true);
			}
			if (_controllerTip != null)
			{
				_controllerTip.SetActive(value: false);
			}
		}
	}

	private void UnbindInputControllerEvent()
	{
		SingletonController<InputController>.Instance.OnControlSchemeChanged -= InputController_OnControlSchemeChanged;
	}

	private void OnDestroy()
	{
		UnbindInputControllerEvent();
		_player.OnDashesCountSet -= Player_OnDashesCountSet;
		_player.OnDashed -= Player_OnDashed;
		_player.OnDashCooldownUpdated -= Player_OnDashCooldownUpdated;
		_player.OnDashCountChanged -= Player_OnDashCountChanged;
		_player.OnDashReady -= Player_OnDashReady;
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnDashVisualSettingChanged -= GameplaySettingsController_OnDashVisualSettingChanged;
	}
}
