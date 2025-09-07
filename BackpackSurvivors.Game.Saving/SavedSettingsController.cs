using BackpackSurvivors.System;
using BackpackSurvivors.System.Saving;

namespace BackpackSurvivors.Game.Saving;

internal class SavedSettingsController : SingletonController<SavedSettingsController>
{
	public SettingsSaveState SettingsSaveState { get; private set; }

	private void Start()
	{
		LoadSavedSettings();
		base.IsInitialized = true;
	}

	public void SaveSettings(SettingsSaveState settingsSaveState)
	{
		SavedSettingsFileController.Save(settingsSaveState);
		SettingsSaveState = settingsSaveState;
	}

	private void LoadSavedSettings()
	{
		SettingsSaveState settingsSaveState = SavedSettingsFileController.Load();
		if (settingsSaveState != null)
		{
			SettingsSaveState = settingsSaveState;
		}
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
