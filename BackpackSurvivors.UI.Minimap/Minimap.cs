using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.System.Settings.Events;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

public class Minimap : MonoBehaviour
{
	[SerializeField]
	private MinimapCamera _minimapCamera;

	[SerializeField]
	private GameObject _minimap;

	[SerializeField]
	private bool enableOnStartup = true;

	public void SetCameraToFollow(Transform player)
	{
		_minimapCamera.Player = player;
		base.gameObject.SetActive(enableOnStartup);
	}

	private void Start()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SettingsController>.Instance, RegisterSettingsControllerInitialized);
	}

	private void RegisterSettingsControllerInitialized()
	{
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnMinimapSettingChanged += GameplaySettingsController_OnMinimapSettingChanged;
	}

	private void GameplaySettingsController_OnMinimapSettingChanged(object sender, MinimapSettingsChangedEventArgs e)
	{
		_minimap.SetActive(e.VisibleMinimap == Enums.MinimapVisual.Visible);
	}

	private void OnDestroy()
	{
		SingletonController<SettingsController>.Instance.GameplaySettingsController.OnMinimapSettingChanged -= GameplaySettingsController_OnMinimapSettingChanged;
	}
}
