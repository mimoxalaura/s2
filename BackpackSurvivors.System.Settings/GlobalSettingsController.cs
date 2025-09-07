using BackpackSurvivors.Game.Saving;

namespace BackpackSurvivors.System.Settings;

public class GlobalSettingsController
{
	public int MaxNoticeNumberSeen;

	internal void FillSaveState(SettingsSaveState saveState)
	{
		saveState.MaxNoticeNumberSeen = MaxNoticeNumberSeen;
	}

	internal void LoadSettingsFromSavegame(SettingsSaveState settingsSaveState)
	{
		MaxNoticeNumberSeen = settingsSaveState.MaxNoticeNumberSeen;
	}
}
