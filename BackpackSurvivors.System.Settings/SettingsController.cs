using System;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System.Settings.Events;
using BackpackSurvivors.UI.Settings;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using UnityEngine;

namespace BackpackSurvivors.System.Settings;

public class SettingsController : BaseSingletonModalUIController<SettingsController>
{
	[SerializeField]
	private SettingsUI _settingsUI;

	private AudioSettingsController _audioSettingsController;

	private VideoSettingsController _videoSettingsController;

	private GameplaySettingsController _gameplaySettingsController;

	private GlobalSettingsController _globalSettingsController;

	public AudioSettingsController AudioSettingsController => _audioSettingsController;

	public VideoSettingsController VideoSettingsController => _videoSettingsController;

	public GameplaySettingsController GameplaySettingsController => _gameplaySettingsController;

	public GlobalSettingsController GlobalSettingsController => _globalSettingsController;

	private void Start()
	{
		RegisterEvents();
	}

	public void SaveSettings()
	{
		SingletonController<SavedSettingsController>.Instance.SaveSettings(GetSaveState());
	}

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	public override void AfterBaseAwake()
	{
		Init();
	}

	public override void OpenUI()
	{
		base.OpenUI();
		_settingsUI.OpenUI();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		_settingsUI.CloseUI();
		SingletonController<SavedSettingsController>.Instance.SaveSettings(GetSaveState());
	}

	private SettingsSaveState GetSaveState()
	{
		SettingsSaveState settingsSaveState = new SettingsSaveState();
		_audioSettingsController.FillSaveState(settingsSaveState);
		_videoSettingsController.FillSaveState(settingsSaveState);
		_gameplaySettingsController.FillSaveState(settingsSaveState);
		_globalSettingsController.FillSaveState(settingsSaveState);
		return settingsSaveState;
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SavedSettingsController>.Instance, LoadSavedSettings, parameter: true);
	}

	public void LoadSavedSettings(bool applyVideoSettings = false)
	{
		if (SingletonController<SavedSettingsController>.Instance.SettingsSaveState == null)
		{
			base.IsInitialized = true;
			return;
		}
		_audioSettingsController.LoadSettingsFromSavegame(SingletonController<SavedSettingsController>.Instance.SettingsSaveState);
		_settingsUI.SetMasterAudioFromSave(_audioSettingsController.MasterVolume);
		_settingsUI.SetMusicAudioFromSave(_audioSettingsController.MusicVolume);
		_settingsUI.SetSFXAudioFromSave(_audioSettingsController.SfxVolume);
		_settingsUI.SetAmbienceAudioFromSave(_audioSettingsController.AmbienceVolume);
		_videoSettingsController.LoadSettingsFromSavegame(SingletonController<SavedSettingsController>.Instance.SettingsSaveState, applyVideoSettings);
		_settingsUI.SetResolutionFromSave((int)_videoSettingsController.Resolution);
		_settingsUI.SetWindowedModeFromSave((int)_videoSettingsController.Windowmodes);
		_settingsUI.SetMaxFPSFromSave(_videoSettingsController.MaxFPS);
		_settingsUI.SetCameraShakeFromSave((int)_videoSettingsController.CameraShake);
		_gameplaySettingsController.LoadSettingsFromSavegame(SingletonController<SavedSettingsController>.Instance.SettingsSaveState);
		_settingsUI.SetShowDamageNumbersFromSave((int)_gameplaySettingsController.ShowDamageNumbers);
		_settingsUI.SetHealthBarsFromSave((int)_gameplaySettingsController.ShowHealthBars);
		_settingsUI.SetCooldownVisualsFromSave((int)_gameplaySettingsController.CooldownVisuals);
		_settingsUI.SetMinimapVisualsFromSave((int)_gameplaySettingsController.MinimapVisual);
		_settingsUI.SetTooltipComplexityFromSave((int)_gameplaySettingsController.TooltipComplexity);
		_settingsUI.SetFlashOnDamageTakenFromSave((int)_gameplaySettingsController.FlashOnDamageTaken);
		_settingsUI.SetDashVisualFromSave((int)_gameplaySettingsController.DashVisual);
		_settingsUI.SetTargetingFromSave((int)_gameplaySettingsController.Targeting);
		_globalSettingsController.LoadSettingsFromSavegame(SingletonController<SavedSettingsController>.Instance.SettingsSaveState);
		base.IsInitialized = true;
	}

	private void SettingsController_OnCloseButtonClicked(object sender, EventArgs e)
	{
		UnityEngine.Object.FindObjectOfType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.Settings);
	}

	private void Init()
	{
		_audioSettingsController = new AudioSettingsController();
		_audioSettingsController.OnVolumeChanged += AudioSettingsController_OnVolumeChanged;
		_videoSettingsController = new VideoSettingsController();
		_videoSettingsController.UpdateMaxFPS(60);
		_gameplaySettingsController = new GameplaySettingsController();
		_globalSettingsController = new GlobalSettingsController();
	}

	private void AudioSettingsController_OnVolumeChanged(object sender, VolumeChangedEventArgs e)
	{
		SingletonController<AudioController>.Instance.SetVolume(e.AudioType, e.NewValue);
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Settings;
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}
}
