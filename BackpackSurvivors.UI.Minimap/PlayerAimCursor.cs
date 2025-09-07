using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Settings.Events;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

public class PlayerAimCursor : MonoBehaviour
{
	[SerializeField]
	private GameObject _aimCursor;

	private bool _shouldShow;

	private bool _usingKeyboard;

	private Vector2 _controllerAimVector;

	private bool _inTown;

	private void Start()
	{
		_inTown = SingletonController<SceneChangeController>.Instance.CurrentSceneName.Contains("4. Town");
		_shouldShow = SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting == Enums.Targeting.Manual;
		_aimCursor.SetActive(_shouldShow);
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnTargetingSettingChanged += GameplaySettingsController_OnTargetingSettingChanged;
		SingletonController<InputController>.Instance.OnShiftHandler += Instance_OnShiftHandler;
		SingletonController<InputController>.Instance.OnControlSchemeChanged += Instance_OnControlSchemeChanged;
		SingletonController<InputController>.Instance.OnPlayerAimHandler += Instance_OnPlayerAimHandler;
		_usingKeyboard = SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard;
	}

	private void Instance_OnPlayerAimHandler(object sender, PlayerAimEventArgs e)
	{
		if (!_inTown)
		{
			_controllerAimVector = e.PlayerAim;
		}
	}

	private void Instance_OnControlSchemeChanged(object sender, ControlSchemeChangedEventArgs e)
	{
		if (!_inTown)
		{
			_usingKeyboard = SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard;
		}
	}

	private void Instance_OnShiftHandler(object sender, ShiftEventArgs e)
	{
		if (!_inTown && SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting != Enums.Targeting.Manual && SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting != Enums.Targeting.Automatic)
		{
			_shouldShow = e.Pressed;
			_aimCursor.SetActive(_shouldShow);
		}
	}

	private void GameplaySettingsController_OnTargetingSettingChanged(object sender, TargetingSettingsChangedEventArgs e)
	{
		if (!_inTown)
		{
			_shouldShow = e.Targeting == Enums.Targeting.Manual;
			_aimCursor.SetActive(_shouldShow);
		}
	}

	private void OnDestroy()
	{
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnTargetingSettingChanged -= GameplaySettingsController_OnTargetingSettingChanged;
		SingletonController<InputController>.Instance.OnShiftHandler -= Instance_OnShiftHandler;
		SingletonController<InputController>.Instance.OnControlSchemeChanged -= Instance_OnControlSchemeChanged;
	}

	private void Update()
	{
		if (!_inTown && _shouldShow)
		{
			if (_usingKeyboard)
			{
				Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - base.transform.position;
				float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				base.transform.rotation = Quaternion.Euler(0f, 0f, z);
			}
			else if (_controllerAimVector.sqrMagnitude > 0.01f)
			{
				float z2 = Mathf.Atan2(_controllerAimVector.y, _controllerAimVector.x) * 57.29578f;
				base.transform.rotation = Quaternion.Euler(0f, 0f, z2);
			}
		}
	}
}
